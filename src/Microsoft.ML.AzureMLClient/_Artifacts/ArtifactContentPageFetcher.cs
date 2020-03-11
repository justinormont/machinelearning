// <copyright file="ArtifactContentPageFetcher.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Azure.MachineLearning.Services.Generated;
using Azure.MachineLearning.Services.Generated.Models;

namespace Azure.MachineLearning.Services.Artifacts
{
    public class ArtifactContentPageFetcher : IPageFetcher<ArtifactContent>
    {
        public ArtifactContentPageFetcher(
            ServiceContext serviceContext,
            string origin,
            string container,
            string path)
        {
            this.ServiceContext = serviceContext;
            this.OnLastPage = false;
            this.Origin = origin;
            this.Container = container;
            this.Path = path;
        }

        public ServiceContext ServiceContext { get; private set; }

        public string Origin { get; private set; }

        public string Container { get; private set; }

        public string Path { get; private set; }

        public string ContinuationToken { get; private set; }

        public bool OnLastPage { get; private set; }

        public IEnumerable<ArtifactContent> FetchNextPage()
        {
            return this.FetchNextPageAsync().Result;
        }

        public async Task<IEnumerable<ArtifactContent>> FetchNextPageAsync(
            Dictionary<string, List<string>> customHeaders = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            List<ArtifactContent> result = null;
            if (!this.OnLastPage)
            {
                var restClient = new RestClient(this.ServiceContext.Credentials);
                // restClient.Region = this.ServiceContext.Location;

                PaginatedArtifactContentInformationList response = await RestCallWrapper.WrapAsync(
                    () => restClient.Artifacts.ListSasByPrefixWithHttpMessagesAsync(
                    this.ServiceContext.SubscriptionId,
                    this.ServiceContext.ResourceGroupName,
                    this.ServiceContext.WorkspaceName,
                    this.Origin,
                    this.Container,
                    this.Path,
                    this.ContinuationToken,
                    customHeaders: customHeaders,
                    cancellationToken: cancellationToken)).ConfigureAwait(false);

                this.ContinuationToken = response.ContinuationToken;
                this.OnLastPage = string.IsNullOrEmpty(this.ContinuationToken);

                result = response.Value.Select(x => new ArtifactContent(this.ServiceContext, x)).ToList();
            }
            return result;
        }
    }
}
