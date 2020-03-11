// <copyright file = "ArtifactOperations.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

using Azure.MachineLearning.Services.Generated;
using Azure.MachineLearning.Services.Generated.Models;

using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace Azure.MachineLearning.Services.Artifacts
{
    public class ArtifactOperations
    {
        // Taken from the .NET implementation. Largest multiple of 4096 that is still smaller
        // than the large object heap threshold. (85K).
        private const int DefaultCopyBufferSize = 81920;

        public ArtifactOperations(ServiceContext serviceContext)
        {
            Throw.IfNull(serviceContext, nameof(serviceContext));
            this.ServiceContext = serviceContext;
        }

        public ServiceContext ServiceContext { get; private set; }

        public IPageFetcher<ArtifactContent> GetPagedList(string artifactPrefix)
        {
            Throw.IfNullOrEmpty(artifactPrefix, nameof(artifactPrefix));

            string[] components = artifactPrefix.Split("/".ToCharArray(), 3);
            if (components.Length != 3)
            {
                throw new FormatException($"Invalid Prefix for artifact: {artifactPrefix}.");
            }

            var fetcher =
                new ArtifactContentPageFetcher(
                    this.ServiceContext,
                    origin: components[0],
                    container: components[1],
                    path: components[2]);

            return fetcher;
        }

        public IEnumerable<ArtifactContent> List(string artifactPrefix)
        {
            Throw.IfNullOrEmpty(artifactPrefix, nameof(artifactPrefix));
            LazyEnumerator<ArtifactContent> lister = new LazyEnumerator<ArtifactContent>();
            lister.Fetcher = this.GetPagedList(artifactPrefix);

            return lister;
        }

        public async Task DownloadArtifactAsync(
            Uri amlUri,
            FileInfo outputFileLocation,
            bool overwrite = false,
            Dictionary<string, List<string>> customHeaders = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            Throw.IfNull(amlUri, nameof(amlUri));

            ArtifactInformation artifactInfo = AzureMLUriParser.GetArtifactInformation(amlUri);

            await DownloadArtifactAsync(
                artifactInfo.Origin,
                artifactInfo.Container,
                artifactInfo.Path,
                outputFileLocation,
                overwrite,
                customHeaders,
                cancellationToken).ConfigureAwait(false);
        }

        public async Task DownloadArtifactAsync(
            string origin,
            string container,
            string path,
            FileInfo outputFileLocation,
            bool overwrite = false,
            Dictionary<string, List<string>> customHeaders = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            Throw.IfNullOrEmpty(origin, nameof(origin));
            Throw.IfNullOrEmpty(container, nameof(container));
            Throw.IfNullOrEmpty(path, nameof(path));

            if (!overwrite && outputFileLocation.Exists)
            {
                throw new InvalidOperationException(
                    $"Input file name {outputFileLocation.FullName} already exists. Set \"overwrite\" to true to overrwrite this file.");
            }

            var restClient = new RestClient(ServiceContext.Credentials);
            restClient.BaseUri = this.ServiceContext.ExperimentationEndpoint;

            // Generated code returns a stream. This stream must be copied to a new file.
            Stream infoStream = await RestCallWrapper.WrapAsync(
                () => restClient.Artifacts.DownloadWithHttpMessagesAsync(
                    ServiceContext.SubscriptionId,
                    ServiceContext.ResourceGroupName,
                    ServiceContext.WorkspaceName,
                    origin,
                    container,
                    path,
                    customHeaders: customHeaders,
                    cancellationToken: cancellationToken)).ConfigureAwait(false);

            using (FileStream fileStream = File.Create(outputFileLocation.FullName))
            {
                await infoStream.CopyToAsync(
                    destination: fileStream,
                    bufferSize: DefaultCopyBufferSize,
                    cancellationToken: cancellationToken).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Uploads a local file into a blob. First creates an empty artifact in the workspace and then uploads the file
        /// to that blob.
        /// </summary>
        public async Task<UploadArtifactResult> UploadDependencyAsync(
            FileInfo localFilePath,
            string name = null,
            string container = null,
            Dictionary<string, List<string>> customHeaders = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            Throw.IfFileNotExists(localFilePath);

            var restClient = new RestClient(ServiceContext.Credentials);
            // restClient.Region = ServiceContext.Location;
            string origin = "LocalUpload";

            name = name ?? localFilePath.Name;
            string containerName = container ?? Guid.NewGuid().ToString("N").Substring(0, 8);

            // Create an empty Artifact and get its URI.
            BatchArtifactContentInformationResult emptyArtifactBatch =
                await CreateEmptyArtifactAsync(
                    origin,
                    containerName,
                    name,
                    customHeaders,
                    cancellationToken).ConfigureAwait(false);

            ArtifactContentInformation emptyArtifactContentDto =
                emptyArtifactBatch.ArtifactContentInformation[name];
            Uri contentUri = new Uri(emptyArtifactContentDto.ContentUri);

            await UploadBlobFromFileAsync(
                contentUri,
                localFilePath,
                cancellationToken).ConfigureAwait(false);

            // Don't expose the blob directly. Use an aml path instead.
            Generated.Models.Artifact emptyArtifact = emptyArtifactBatch.Artifacts[name];
            var simplifiedUri = new Uri(
                string.Format("aml://artifact/{0}", emptyArtifact.ArtifactId));

            return new UploadArtifactResult
            {
                DependencyUri = simplifiedUri,
                DependencyName = name
            };
        }

        private async Task UploadBlobFromFileAsync(
            Uri uploadTargetUri,
            FileInfo localFilePath,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            CloudBlockBlob cloudBlockBlob = new CloudBlockBlob(uploadTargetUri);
            await cloudBlockBlob.UploadFromFileAsync(
                path: localFilePath.FullName,
                accessCondition: AccessCondition.GenerateEmptyCondition(),
                options: null,
                operationContext: null,
                cancellationToken: cancellationToken).ConfigureAwait(false);
        }

        private async Task<BatchArtifactContentInformationResult> CreateEmptyArtifactAsync(
            string origin,
            string containerName,
            string name,
            Dictionary<string, List<string>> customHeaders = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var restClient = new RestClient(ServiceContext.Credentials);
            // restClient.Region = ServiceContext.Location;

            var batchCreateDto = new ArtifactPathList
            {
                Paths = new List<ArtifactPath>
                {
                    new ArtifactPath(name)
                },
            };

            BatchArtifactContentInformationResult response = await RestCallWrapper.WrapAsync(
                () => restClient.Artifacts.BatchCreateEmptyArtifactsWithHttpMessagesAsync(
                    ServiceContext.SubscriptionId,
                    ServiceContext.ResourceGroupName,
                    ServiceContext.WorkspaceName,
                    origin,
                    containerName,
                    batchCreateDto,
                    customHeaders: customHeaders,
                    cancellationToken: cancellationToken)).ConfigureAwait(false);
            return response;
        }
    }
}
