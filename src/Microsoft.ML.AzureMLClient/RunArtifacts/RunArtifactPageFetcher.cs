// <copyright file="RunArtifactPageFetcher.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Azure.MachineLearning.Services.GeneratedOld;
using Azure.MachineLearning.Services.GeneratedOld.Models;

namespace Azure.MachineLearning.Services.RunArtifacts
{
    public class RunArtifactPageFetcher : IPageFetcher<RunArtifact>
    {
        public RunArtifactPageFetcher(ServiceContext serviceContext, string experimentName, string runId)
        {
            Throw.IfNull(serviceContext, nameof(serviceContext));
            Throw.IfNullOrEmpty(experimentName, nameof(experimentName));
            Throw.IfNullOrEmpty(runId, nameof(runId));
            this.OnLastPage = false;
            this.ContinuationToken = default(string);
            this.ServiceContext = serviceContext;
            this.ExperimentName = experimentName;
            this.RunId = runId;
        }

        public ServiceContext ServiceContext { get; private set; }

        public bool OnLastPage { get; private set; }

        public string ContinuationToken { get; private set; }

        public string ExperimentName { get; private set; }

        public string RunId { get; private set; }

        public IEnumerable<RunArtifact> FetchNextPage()
        {
            return this.FetchNextPageAsync().Result;
        }

        public async Task<IEnumerable<RunArtifact>> FetchNextPageAsync(
            Dictionary<string, List<string>> customHeaders = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            List<RunArtifact> result = new List<RunArtifact>();

            if (!this.OnLastPage)
            {
                var restClient = new RestClient(this.ServiceContext.Credentials);
                restClient.BaseUri = this.ServiceContext.ExperimentationEndpoint;

                PaginatedArtifactDto response = await RestCallWrapper.WrapAsync(
                    () => restClient.RunArtifact.ListInContainerWithHttpMessagesAsync(
                        this.ServiceContext.SubscriptionId,
                        this.ServiceContext.ResourceGroupName,
                        this.ServiceContext.WorkspaceName,
                        this.ExperimentName,
                        this.RunId,
                        this.ContinuationToken,
                        customHeaders: customHeaders,
                        cancellationToken: cancellationToken)).ConfigureAwait(false);

                this.ContinuationToken = response.ContinuationToken;
                this.OnLastPage = string.IsNullOrEmpty(this.ContinuationToken);

                // Convert from paginated to a List<RunArtifact>
                foreach (var runOutput in response.Value)
                {
                    result.Add(new RunArtifact(this.ServiceContext, this.ExperimentName, this.RunId, runOutput));
                }
            }
            return result;
        }
    }
}