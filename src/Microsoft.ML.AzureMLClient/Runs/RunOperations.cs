// <copyright file = "RunOperations.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

using Azure.MachineLearning.Services.GeneratedOld;
using Azure.MachineLearning.Services.GeneratedOld.Models;

using Microsoft.Rest;

using Newtonsoft.Json;

namespace Azure.MachineLearning.Services.Runs
{
    public class RunOperations
    {
        public RunOperations(ServiceContext serviceContext, string experimentName)
        {
            Throw.IfNull(serviceContext, nameof(serviceContext));
            Throw.IfNullOrEmpty(experimentName, nameof(experimentName));

            this.ServiceContext = serviceContext;
            this.ExperimentName = experimentName;
            this.Factories = new FactoryManager<IRunFactory>();
            this.Factories.DefaultFactory = new BasicRunFactory();
            this.Factories.RegisterFactory(RunConstants.AutoMLDiscriminator, new AutoMLRunFactory());

            this.RestClient = new RestClient(this.ServiceContext.Credentials);
            this.RestClient.BaseUri = this.ServiceContext.ExperimentationEndpoint;
        }

        public ServiceContext ServiceContext { get; private set; }

        public string ExperimentName { get; private set; }

        public FactoryManager<IRunFactory> Factories { get; set; }

        public RestClient RestClient { get; set; }

        public async Task<Run> GetAsync(
            string runId,
            Dictionary<string, List<string>> customHeaders = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            Throw.IfNullOrEmpty(runId, nameof(runId));

            var restClient = new RestClient(this.ServiceContext.Credentials);
            restClient.BaseUri = this.ServiceContext.ExperimentationEndpoint;

            RunDto response = await RestCallWrapper.WrapAsync(
                () => restClient.Run.GetWithHttpMessagesAsync(
                     this.ServiceContext.SubscriptionId,
                     this.ServiceContext.ResourceGroupName,
                     this.ServiceContext.WorkspaceName,
                     this.ExperimentName,
                     runId,
                     customHeaders: customHeaders,
                     cancellationToken: cancellationToken)).ConfigureAwait(false);

            return this.Factories.GetFactory(response.RunType).Create(
                this.ServiceContext,
                this.ExperimentName,
                this.Factories,
                response);
        }

        public IEnumerable<Run> List(
            string filter = default(string),
            IList<string> orderby = default(IList<string>),
            string sortorder = default(string),
            int? top = default(int?))
        {
            var lister = new LazyEnumerator<Run>();
            lister.Fetcher = this.GetPagedList(filter, orderby, sortorder, top);

            return lister;
        }

        public IPageFetcher<Run> GetPagedList(
            string filter = default(string),
            IList<string> orderby = default(IList<string>),
            string sortorder = default(string),
            int? top = default(int?))
        {
            return new RootRunPageFetcher(
                this.ServiceContext,
                this.ExperimentName,
                this.Factories,
                filter,
                orderby,
                sortorder,
                top);
        }

        public async Task<Run> CreateAsync(
            RunConfigurationBase runConfig,
            Dictionary<string, List<string>> customHeaders = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            // We need to support multiple types of Runs. For example
            // Estimators and AutoML. This is also supposed to be a user
            // extension point, so use a visitor-like pattern
            Throw.IfNull(runConfig, nameof(runConfig));
            return await runConfig.SubmitRunAsync(this, customHeaders, cancellationToken).ConfigureAwait(false);
        }

        public async Task<HttpOperationResponse<RunStatus>> SubmitRunInternalAsync(
            RunDefinition runDefinition,
            byte[] projectZip,
            Dictionary<string, List<string>> customHeaders,
            CancellationToken cancellationToken)
        {
            Throw.IfNull(runDefinition, nameof(runDefinition));

            Guid subscriptionId = this.ServiceContext.SubscriptionId;
            string resourceGroupName = this.ServiceContext.ResourceGroupName;
            string workspaceName = this.ServiceContext.WorkspaceName;

            string parentRunId;
            if (!string.IsNullOrEmpty(runDefinition.ParentRunId))
            {
                parentRunId = runDefinition.ParentRunId;
            }
            else
            {
                parentRunId = StaticHelpers.GenerateUniqueRunId(this.ExperimentName);
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
                tracingParameters.Add("experimentName", this.ExperimentName);
                tracingParameters.Add("runDefinition", runDefinition);
                tracingParameters.Add("cancellationToken", cancellationToken);
                ServiceClientTracing.Enter(invocationId, null, "PostRemoteRun", tracingParameters);
            }
            // Construct URL
            var baseUri = this.RestClient.BaseUri.AbsoluteUri;
            var uri = new System.Uri(new System.Uri(baseUri + (baseUri.EndsWith("/") ? string.Empty : "/")), "execution/v1.0/subscriptions/{subscriptionId}/resourceGroups/{resourceGroupName}/providers/Microsoft.MachineLearningServices/workspaces/{workspaceName}/experiments/{experimentName}/run").ToString();

            uri = uri.Replace("{subscriptionId}", System.Uri.EscapeDataString(Microsoft.Rest.Serialization.SafeJsonConvert.SerializeObject(subscriptionId, this.RestClient.SerializationSettings).Trim('"')));
            uri = uri.Replace("{resourceGroupName}", System.Uri.EscapeDataString(resourceGroupName));
            uri = uri.Replace("{workspaceName}", System.Uri.EscapeDataString(workspaceName));
            uri = uri.Replace("{experimentName}", System.Uri.EscapeDataString(this.ExperimentName));
            uri = uri.Replace("{RunId}", System.Uri.EscapeDataString(parentRunId));

            // Create HTTP transport objects
            var httpRequest = new HttpRequestMessage();
            HttpResponseMessage httpResponseMessage = null;
            httpRequest.Method = new HttpMethod("POST");
            httpRequest.RequestUri = new System.Uri(uri);

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
            MultipartFormDataContent multipartContent = new MultipartFormDataContent();

            string requestContent = Microsoft.Rest.Serialization.SafeJsonConvert.SerializeObject(
                    runDefinition,
                    this.RestClient.SerializationSettings);

            var runDefinitionContent = new StringContent(requestContent);
            multipartContent.Add(runDefinitionContent, "files", "definition.json");

            var fileStr = new ByteArrayContent(projectZip);
            multipartContent.Add(fileStr, "files", "project.zip");

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
            httpResponseMessage = await this.RestClient.HttpClient.SendAsync(
                httpRequest,
                cancellationToken).ConfigureAwait(false);
            if (shouldTrace)
            {
                ServiceClientTracing.ReceiveResponse(invocationId, httpResponseMessage);
            }
            HttpStatusCode statusCode = httpResponseMessage.StatusCode;
            cancellationToken.ThrowIfCancellationRequested();
            string responseContent = null;
            if ((int)statusCode != 200)
            {
                var ex = new MachineLearningServiceException(string.Format("Operation returned an invalid status code '{0}'", statusCode));
                if (httpResponseMessage.Content != null)
                {
                    try
                    {
                        responseContent = await httpResponseMessage.Content.ReadAsStringAsync().ConfigureAwait(false);
                        var errorBody = Microsoft.Rest.Serialization.SafeJsonConvert.DeserializeObject<GeneratedOld.Models.ErrorResponse>(responseContent, this.RestClient.DeserializationSettings);
                        if (errorBody != null)
                        {
                            ex.Body = new ErrorResponse(errorBody);
                        }
                    }
                    catch (JsonException)
                    {
                        // Ignore the exception
                    }
                }
                else
                {
                    responseContent = string.Empty;
                }
                ex.Request = new HttpRequestMessageWrapper(httpRequest, requestContent);
                ex.Response = new HttpResponseMessageWrapper(httpResponseMessage, responseContent);
                if (shouldTrace)
                {
                    ServiceClientTracing.Error(invocationId, ex);
                }
                httpRequest.Dispose();
                if (httpResponseMessage != null)
                {
                    httpResponseMessage.Dispose();
                }
                throw ex;
            }
            // Create Result
            var result = new HttpOperationResponse<RunStatus>();
            result.Request = httpRequest;
            result.Response = httpResponseMessage;
            // Deserialize Response
            if ((int)statusCode == 200)
            {
                responseContent = await httpResponseMessage.Content.ReadAsStringAsync().ConfigureAwait(false);
                try
                {
                    result.Body = Microsoft.Rest.Serialization.SafeJsonConvert.DeserializeObject<RunStatus>(responseContent, this.RestClient.DeserializationSettings);
                }
                catch (JsonException jsonException)
                {
                    httpRequest.Dispose();
                    if (httpResponseMessage != null)
                    {
                        httpResponseMessage.Dispose();
                    }
                    throw new SerializationException("Unable to deserialize the response.", responseContent, jsonException);
                }
            }
            if (shouldTrace)
            {
                ServiceClientTracing.Exit(invocationId, result);
            }
            return result;
        }

        public async Task<HttpOperationResponse<RunStatus>> SubmitSnapshotRunInternalAsync(
            RunDefinition runDefinition,
            Dictionary<string, List<string>> customHeaders,
            CancellationToken cancellationToken)
        {
            Throw.IfNull(runDefinition, nameof(runDefinition));

            Guid subscriptionId = this.ServiceContext.SubscriptionId;
            string resourceGroupName = this.ServiceContext.ResourceGroupName;
            string workspaceName = this.ServiceContext.WorkspaceName;

            string parentRunId;
            if (!string.IsNullOrEmpty(runDefinition.ParentRunId))
            {
                parentRunId = runDefinition.ParentRunId;
            }
            else
            {
                parentRunId = StaticHelpers.GenerateUniqueRunId(this.ExperimentName);
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
                tracingParameters.Add("experimentName", this.ExperimentName);
                tracingParameters.Add("runDefinition", runDefinition);
                tracingParameters.Add("cancellationToken", cancellationToken);
                ServiceClientTracing.Enter(invocationId, null, "PostRemoteRun", tracingParameters);
            }
            // Construct URL
            var baseUri = this.RestClient.BaseUri.AbsoluteUri;
            var uri = new System.Uri(new System.Uri(baseUri + (baseUri.EndsWith("/") ? string.Empty : "/")), "execution/v1.0/subscriptions/{subscriptionId}/resourceGroups/{resourceGroupName}/providers/Microsoft.MachineLearningServices/workspaces/{workspaceName}/experiments/{experimentName}/snapshotrun").ToString();

            uri = uri.Replace("{subscriptionId}", System.Uri.EscapeDataString(Microsoft.Rest.Serialization.SafeJsonConvert.SerializeObject(subscriptionId, this.RestClient.SerializationSettings).Trim('"')));
            uri = uri.Replace("{resourceGroupName}", System.Uri.EscapeDataString(resourceGroupName));
            uri = uri.Replace("{workspaceName}", System.Uri.EscapeDataString(workspaceName));
            uri = uri.Replace("{experimentName}", System.Uri.EscapeDataString(this.ExperimentName));
            uri = uri.Replace("{RunId}", System.Uri.EscapeDataString(parentRunId));

            // Create HTTP transport objects
            var httpRequest = new HttpRequestMessage();
            HttpResponseMessage httpResponseMessage = null;
            httpRequest.Method = new HttpMethod("POST");
            httpRequest.RequestUri = new System.Uri(uri);

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
            var runDefinitionContent = new StringContent(
                Microsoft.Rest.Serialization.SafeJsonConvert.SerializeObject(
                    runDefinition,
                    this.RestClient.SerializationSettings),
                System.Text.Encoding.UTF8);
            runDefinitionContent.Headers.ContentType = System.Net.Http.Headers.MediaTypeHeaderValue.Parse(
                "application/json; charset=utf-8");

            httpRequest.Content = runDefinitionContent;
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
            httpResponseMessage = await this.RestClient.HttpClient.SendAsync(httpRequest, cancellationToken).ConfigureAwait(false);
            if (shouldTrace)
            {
                ServiceClientTracing.ReceiveResponse(invocationId, httpResponseMessage);
            }
            HttpStatusCode statusCode = httpResponseMessage.StatusCode;
            cancellationToken.ThrowIfCancellationRequested();
            string responseContent = null;
            if ((int)statusCode != 200)
            {
                var ex = new MachineLearningServiceException(string.Format("Operation returned an invalid status code '{0}'", statusCode));
                if (httpResponseMessage.Content != null)
                {
                    try
                    {
                        responseContent = await httpResponseMessage.Content.ReadAsStringAsync().ConfigureAwait(false);
                        var errorBody = Microsoft.Rest.Serialization.SafeJsonConvert.DeserializeObject<GeneratedOld.Models.ErrorResponse>(responseContent, this.RestClient.DeserializationSettings);
                        if (errorBody != null)
                        {
                            ex.Body = new ErrorResponse(errorBody);
                        }
                    }
                    catch (JsonException)
                    {
                        // Ignore the exception
                    }
                }
                else
                {
                    responseContent = string.Empty;
                }
                ex.Request = new HttpRequestMessageWrapper(httpRequest, runDefinitionContent.AsString());
                ex.Response = new HttpResponseMessageWrapper(httpResponseMessage, responseContent);
                if (shouldTrace)
                {
                    ServiceClientTracing.Error(invocationId, ex);
                }
                httpRequest.Dispose();
                if (httpResponseMessage != null)
                {
                    httpResponseMessage.Dispose();
                }
                throw ex;
            }
            // Create Result
            var result = new HttpOperationResponse<RunStatus>();
            result.Request = httpRequest;
            result.Response = httpResponseMessage;
            // Deserialize Response
            if ((int)statusCode == 200)
            {
                responseContent = await httpResponseMessage.Content.ReadAsStringAsync().ConfigureAwait(false);
                try
                {
                    result.Body = Microsoft.Rest.Serialization.SafeJsonConvert.DeserializeObject<RunStatus>(
                        responseContent,
                        this.RestClient.DeserializationSettings);
                }
                catch (JsonException jsonException)
                {
                    httpRequest.Dispose();
                    if (httpResponseMessage != null)
                    {
                        httpResponseMessage.Dispose();
                    }
                    throw new SerializationException("Unable to deserialize the response.", responseContent, jsonException);
                }
            }
            if (shouldTrace)
            {
                ServiceClientTracing.Exit(invocationId, result);
            }
            return result;
        }
    }
}
