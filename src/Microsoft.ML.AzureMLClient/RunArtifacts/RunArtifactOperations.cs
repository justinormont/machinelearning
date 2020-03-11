// <copyright file = "RunArtifactOperations.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>
using System;
using System.Collections.Generic;
using System.Net;

using System.Threading;
using System.Threading.Tasks;

using Azure.MachineLearning.Services.GeneratedOld;
using Azure.MachineLearning.Services.GeneratedOld.Models;

namespace Azure.MachineLearning.Services.RunArtifacts
{
    public class RunArtifactOperations
    {
        public RunArtifactOperations(
            ServiceContext serviceContext,
            string experimentName,
            string runId)
        {
            Throw.IfNull(serviceContext, nameof(serviceContext));
            Throw.IfNullOrEmpty(experimentName, nameof(experimentName));
            Throw.IfNullOrEmpty(runId, nameof(runId));
            this.ServiceContext = serviceContext;
            this.ExperimentName = experimentName;
            this.RunId = runId;
        }

        public ServiceContext ServiceContext { get; private set; }

        public string ExperimentName { get; private set; }

        public string RunId { get; private set; }

        public RunArtifactContentInformation GetRunArtifactMetaData(string artifactPath)
        {
            return GetRunArtifactMetaDataAsync(artifactPath).Result;
        }

        public async Task<RunArtifactContentInformation> GetRunArtifactMetaDataAsync(
            string artifactPath,
            Dictionary<string, List<string>> customHeaders = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            Throw.IfNullOrEmpty(artifactPath, nameof(artifactPath));
            var restClient = new RestClient(this.ServiceContext.Credentials);
            restClient.BaseUri = this.ServiceContext.ExperimentationEndpoint;

            var runArtifact = new GeneratedOld.RunArtifact(restClient);

            ArtifactContentInformationDto response = await RestCallWrapper.WrapAsync(
                () => restClient.RunArtifact.GetContentInformationWithHttpMessagesAsync(
                    this.ServiceContext.SubscriptionId,
                    this.ServiceContext.ResourceGroupName,
                    this.ServiceContext.WorkspaceName,
                    this.ExperimentName,
                    this.RunId,
                    artifactPath,
                    customHeaders: customHeaders,
                    cancellationToken: cancellationToken)).ConfigureAwait(false);

            var contentMetaData = new RunArtifactContentInformation(this.ServiceContext, response);

            return contentMetaData;
        }

        public string DownloadRunArtifactContentAsString(string artifactPath)
        {
            Throw.IfNullOrEmpty(artifactPath, nameof(artifactPath));
            return DownloadRunArtifactContentAsStringAsync(artifactPath).Result;
        }

        public async Task<string> DownloadRunArtifactContentAsStringAsync(
            string artifactPath,
            Dictionary<string, List<string>> customHeaders = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            Throw.IfNullOrEmpty(artifactPath, nameof(artifactPath));
            RunArtifactContentInformation runArtrifactMetaData =
                await GetRunArtifactMetaDataAsync(artifactPath, customHeaders, cancellationToken).ConfigureAwait(false);

            WebClient client = new WebClient();
            return await client.DownloadStringTaskAsync(new Uri(runArtrifactMetaData.ContentUri)).ConfigureAwait(false);
        }

        public void DownloadRunArtifactContentAsFile(string artifactPath, string destinationFile)
        {
            Throw.IfNullOrEmpty(artifactPath, nameof(artifactPath));
            Throw.IfNullOrEmpty(destinationFile, nameof(destinationFile));
            DownloadRunArtifactContentAsFileAsync(artifactPath, destinationFile).Wait();
        }

        public async Task DownloadRunArtifactContentAsFileAsync(
            string artifactPath,
            string destinationFile,
            Dictionary<string, List<string>> customHeaders = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            Throw.IfNullOrEmpty(artifactPath, nameof(artifactPath));
            Throw.IfNullOrEmpty(destinationFile, nameof(destinationFile));
            RunArtifactContentInformation runArtrifactMetaData = await
                GetRunArtifactMetaDataAsync(artifactPath, customHeaders, cancellationToken).ConfigureAwait(false);

            WebClient client = new WebClient();
            await client.DownloadFileTaskAsync(new Uri(runArtrifactMetaData.ContentUri), destinationFile).ConfigureAwait(false);
        }

        public IPageFetcher<RunArtifact> GetRunOutputPagedList()
        {
            var fetcher = new RunArtifactPageFetcher(this.ServiceContext, this.ExperimentName, this.RunId);
            return fetcher;
        }

        public IEnumerable<RunArtifact> ListRunOutputs()
        {
            var lister = new LazyEnumerator<RunArtifact>();
            lister.Fetcher = this.GetRunOutputPagedList();

            return lister;
        }
    }
}
