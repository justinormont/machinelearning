// <copyright file="SnapshotOperations.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

using Azure.MachineLearning.Services.GeneratedOld;
using Azure.MachineLearning.Services.GeneratedOld.Models;
using Microsoft.Rest;

namespace Azure.MachineLearning.Services.Snapshots
{
    public class SnapshotOperations
    {
        public SnapshotOperations(ServiceContext serviceContext)
        {
            Throw.IfNull(serviceContext, nameof(serviceContext));
            this.ServiceContext = serviceContext;
            this.RestClient = new RestClient(this.ServiceContext.Credentials);
            this.RestClient.BaseUri = this.ServiceContext.ExperimentationEndpoint;
        }

        public ServiceContext ServiceContext { get; private set; }

        protected RestClient RestClient { get; set; }

        public async Task<Snapshot> GetMetadataAsync(
            Guid snapshotId,
            Dictionary<string, List<string>> customHeaders = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            SnapshotDto response = await RestCallWrapper.WrapAsync(
                () => this.RestClient.Snapshot.GetSnapshotMetadataWithHttpMessagesAsync(
                    this.ServiceContext.SubscriptionId,
                    this.ServiceContext.ResourceGroupName,
                    this.ServiceContext.WorkspaceName,
                    snapshotId,
                    customHeaders: customHeaders,
                    cancellationToken: cancellationToken)).ConfigureAwait(false);

            return new Snapshot(this.ServiceContext, response);
        }

        public async Task<Uri> GetSASUriAsync(
            Guid snapshotId,
            Dictionary<string, List<string>> customHeaders = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            string result = await RestCallWrapper.WrapAsync(
                () => this.RestClient.Snapshot.GetSnapshotFilesZipSasWithHttpMessagesAsync(
                    this.ServiceContext.SubscriptionId,
                    this.ServiceContext.ResourceGroupName,
                    this.ServiceContext.WorkspaceName,
                    snapshotId,
                    customHeaders: customHeaders,
                    cancellationToken: cancellationToken)).ConfigureAwait(false);

            return new Uri(result);
        }

        public async Task<Guid> SnapshotDirectoryAsync(
            DirectoryInfo targetDirectory,
            Guid? parentSnapshotId = null,
            IDictionary<string, string> tags = default(IDictionary<string, string>),
            IDictionary<string, string> properties = default(IDictionary<string, string>),
            int batchSize = 200,
            Dictionary<string, List<string>> customHeaders = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            Throw.IfDirectoryNotExists(targetDirectory, nameof(targetDirectory));

            var localFiles = DirectorySnapshot.HashDirectoryTree(targetDirectory);

            IList<MerkleDiffEntry> merkleDiffs = await RestCallWrapper.WrapAsync(
                () => this.RestClient.Snapshot.SnapshotDiffConstructionWithHttpMessagesAsync(
                    this.ServiceContext.SubscriptionId,
                    this.ServiceContext.ResourceGroupName,
                    this.ServiceContext.WorkspaceName,
                    localFiles,
                    parentSnapshotId: parentSnapshotId,
                    customHeaders: customHeaders,
                    cancellationToken: cancellationToken)).ConfigureAwait(false);

            // If there are no differences whatsoever, we can just return the parent snapshot
            if (merkleDiffs.Count == 0)
            {
                return parentSnapshotId.Value;
            }

            // Use the merkleDiffs to determine the upload
            // Goal is to build a Dictionary<string, FileInfo> of the files to be pushed
            var targetFiles = new Dictionary<string, FileInfo>();
            foreach (MerkleDiffEntry diff in merkleDiffs)
            {
                if (diff.IsFile.Value)
                {
                    if (diff.OperationType.ToLowerInvariant() == "added" ||
                        diff.OperationType.ToLowerInvariant() == "modified")
                    {
                        var correctedPath = Path.Combine(targetDirectory.FullName, diff.FilePath);
                        targetFiles.Add(diff.FilePath, new FileInfo(correctedPath));
                    }
                }
            }

            Guid nextSnapshotId = Guid.Empty;
            Guid? nextParentId = parentSnapshotId;
            do
            {
                // Even if targetFiles is empty, we still need to go through this list once
                // since if we've got to here, then there must be moves or deletes which
                // need to be registered
                nextSnapshotId = Guid.NewGuid();

                // There may be more efficient ways to do this, but Take and Skip are already implemented
                // as complements which handle all integer arguments
                // https://docs.microsoft.com/en-us/dotnet/api/system.linq.enumerable.skip
                // Any increased efficiency from a handwritten implementation is going to be lost in the
                // noise of uploading at least 20MiB of files
                var nextFiles = targetFiles.Take(batchSize).ToDictionary(x => x.Key, x => x.Value);
                targetFiles = targetFiles.Skip(batchSize).ToDictionary(x => x.Key, x => x.Value);

                // Use the dictionary in a call to CreateSnapshotWithHttpMessagesAsync
                await RestCallWrapper.WrapAsync(
                    () => this.CreateSnapshotWithHttpMessagesAsync(
                        this.ServiceContext.SubscriptionId,
                        this.ServiceContext.ResourceGroupName,
                        this.ServiceContext.WorkspaceName,
                        nextSnapshotId,
                        parentSnapshotId: nextParentId,
                        flatDirTreeNodeList: localFiles,
                        files: nextFiles,
                        tags: tags,
                        properties: properties,
                        customHeaders: customHeaders,
                        cancellationToken: cancellationToken)).ConfigureAwait(false);
                nextParentId = nextSnapshotId;
            }
            while (targetFiles.Count > 0);

            return nextSnapshotId;
        }

        public async Task<HttpOperationResponse> CreateSnapshotWithHttpMessagesAsync(
            Guid subscriptionId,
            string resourceGroupName,
            string workspaceName,
            Guid snapshotId,
            string projectName = default(string),
            Guid? parentSnapshotId = default(Guid?),
            FlatDirTreeNodeListDto flatDirTreeNodeList = default(FlatDirTreeNodeListDto),
            IDictionary<string, FileInfo> files = default(IDictionary<string, FileInfo>),
            string accountName = default(string),
            IDictionary<string, string> tags = default(IDictionary<string, string>),
            IDictionary<string, string> properties = default(IDictionary<string, string>),
            Dictionary<string, List<string>> customHeaders = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (resourceGroupName == null)
            {
                throw new ValidationException(ValidationRules.CannotBeNull, nameof(resourceGroupName));
            }
            if (workspaceName == null)
            {
                throw new ValidationException(ValidationRules.CannotBeNull, nameof(workspaceName));
            }
            if (flatDirTreeNodeList == null)
            {
                throw new ValidationException(ValidationRules.CannotBeNull, nameof(flatDirTreeNodeList));
            }
            if (flatDirTreeNodeList.Files == null)
            {
                throw new ValidationException(ValidationRules.CannotBeNull, nameof(flatDirTreeNodeList.Files));
            }
            if (files == null)
            {
                throw new ValidationException(ValidationRules.CannotBeNull, nameof(files));
            }
            if (files.Count > flatDirTreeNodeList.Files.Count)
            {
                throw new ValidationException(ValidationRules.InclusiveMaximum, nameof(files));
            }

            // Tracing
            bool shouldTrace = ServiceClientTracing.IsEnabled;
            string invocationId = null;
            if (shouldTrace)
            {
                invocationId = ServiceClientTracing.NextInvocationId.ToString();
                Dictionary<string, object> tracingParameters = new Dictionary<string, object>();
                tracingParameters.Add("subscriptionId", subscriptionId);
                tracingParameters.Add("resourceGroupName", resourceGroupName);
                tracingParameters.Add("workspaceName", workspaceName);
                tracingParameters.Add("projectName", projectName);
                tracingParameters.Add("snapshotId", snapshotId);
                tracingParameters.Add("parentSnapshotId", parentSnapshotId);
                tracingParameters.Add("files", files);
                tracingParameters.Add("accountName", accountName);
                tracingParameters.Add("cancellationToken", cancellationToken);
                ServiceClientTracing.Enter(invocationId, this, "CreateSnapshot", tracingParameters);
            }
            // Construct URL
            var baseUri = this.RestClient.BaseUri.AbsoluteUri;
            var uri = new System.Uri(new Uri(baseUri + (baseUri.EndsWith("/") ? string.Empty : "/")), "content/v1.0/subscriptions/{subscriptionId}/resourceGroups/{resourceGroupName}/providers/Microsoft.MachineLearningServices/workspaces/{workspaceName}/snapshots/{snapshotId}").ToString();
            uri = uri.Replace("{subscriptionId}", Uri.EscapeDataString(Microsoft.Rest.Serialization.SafeJsonConvert.SerializeObject(subscriptionId, this.RestClient.SerializationSettings).Trim('"')));
            uri = uri.Replace("{resourceGroupName}", Uri.EscapeDataString(resourceGroupName));
            uri = uri.Replace("{workspaceName}", Uri.EscapeDataString(workspaceName));
            uri = uri.Replace("{snapshotId}", Uri.EscapeDataString(Microsoft.Rest.Serialization.SafeJsonConvert.SerializeObject(snapshotId, this.RestClient.SerializationSettings).Trim('"')));
            var queryParameters = new List<string>();
            if (projectName != null)
            {
                queryParameters.Add(string.Format("projectName={0}", Uri.EscapeDataString(projectName)));
            }
            if (parentSnapshotId != null)
            {
                queryParameters.Add(string.Format("parentSnapshotId={0}", Uri.EscapeDataString(Microsoft.Rest.Serialization.SafeJsonConvert.SerializeObject(parentSnapshotId, this.RestClient.SerializationSettings).Trim('"'))));
            }
            if (accountName != null)
            {
                queryParameters.Add(string.Format("accountName={0}", Uri.EscapeDataString(accountName)));
            }
            if (queryParameters.Count > 0)
            {
                uri += "?" + string.Join("&", queryParameters);
            }
            // Create HTTP transport objects
            var httpRequest = new HttpRequestMessage();
            HttpResponseMessage httpResponse = null;
            httpRequest.Method = new HttpMethod("POST");
            httpRequest.RequestUri = new Uri(uri);

            // Set Headers
            if (customHeaders != null)
            {
                foreach (var header in customHeaders)
                {
                    if (httpRequest.Headers.Contains(header.Key))
                    {
                        httpRequest.Headers.Remove(header.Key);
                    }
                    httpRequest.Headers.TryAddWithoutValidation(header.Key, header.Value);
                }
            }

            // Serialize Request
            string requestContent = null;
            using (var multipartContent = new MultipartFormDataContent("Upload--" + DateTime.UtcNow.ToString(System.Globalization.CultureInfo.InvariantCulture)))
            {
                var diffContent = new StringContent(Microsoft.Rest.Serialization.SafeJsonConvert.SerializeObject(flatDirTreeNodeList, this.RestClient.SerializationSettings));
                multipartContent.Add(diffContent, "dirTree");

                if (tags != null)
                {
                    var tagContent = new StringContent(Microsoft.Rest.Serialization.SafeJsonConvert.SerializeObject(tags, this.RestClient.SerializationSettings));
                    multipartContent.Add(tagContent, "tags");
                }

                if (properties != null)
                {
                    var propertyContent = new StringContent(Microsoft.Rest.Serialization.SafeJsonConvert.SerializeObject(properties, this.RestClient.SerializationSettings));
                    multipartContent.Add(propertyContent, "properties");
                }

                foreach (var kvPair in files)
                {
                    string nodePath = kvPair.Key;
                    FileInfo f = kvPair.Value;
                    multipartContent.Add(new StreamContent(f.OpenRead()), "files", nodePath);
                }

                httpRequest.Content = multipartContent;

                // Set Credentials
                if (this.RestClient.Credentials != null)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    await this.RestClient.Credentials.ProcessHttpRequestAsync(httpRequest, cancellationToken).ConfigureAwait(false);
                }
                // Send Request
                if (shouldTrace)
                {
                    ServiceClientTracing.SendRequest(invocationId, httpRequest);
                }
                cancellationToken.ThrowIfCancellationRequested();
                httpResponse = await this.RestClient.HttpClient.SendAsync(httpRequest, cancellationToken).ConfigureAwait(false);
                if (shouldTrace)
                {
                    ServiceClientTracing.ReceiveResponse(invocationId, httpResponse);
                }
                HttpStatusCode statusCode = httpResponse.StatusCode;
                cancellationToken.ThrowIfCancellationRequested();
                string responseContent = null;
                if ((int)statusCode != 200)
                {
                    var ex = new HttpOperationException(string.Format("Operation returned an invalid status code '{0}'", statusCode));
                    if (httpResponse.Content != null)
                    {
                        responseContent = await httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
                    }
                    else
                    {
                        responseContent = string.Empty;
                    }
                    ex.Request = new HttpRequestMessageWrapper(httpRequest, requestContent);
                    ex.Response = new HttpResponseMessageWrapper(httpResponse, responseContent);
                    if (shouldTrace)
                    {
                        ServiceClientTracing.Error(invocationId, ex);
                    }
                    httpRequest.Dispose();
                    if (httpResponse != null)
                    {
                        httpResponse.Dispose();
                    }
                    throw ex;
                }
                // Create Result
                var result = new HttpOperationResponse();
                result.Request = httpRequest;
                result.Response = httpResponse;
                if (shouldTrace)
                {
                    ServiceClientTracing.Exit(invocationId, result);
                }
                return result;
            }
        }
    }
}
