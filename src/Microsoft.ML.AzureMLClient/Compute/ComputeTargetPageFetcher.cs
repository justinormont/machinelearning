// <copyright file="ComputeTargetPageFetcher.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Azure.MachineLearning.Services.GeneratedOld;

namespace Azure.MachineLearning.Services.Compute
{
    public class ComputeTargetPageFetcher : IPageFetcher<ComputeTarget>
    {
        public ComputeTargetPageFetcher(ServiceContext serviceContext)
        {
            Throw.IfNull(serviceContext, nameof(serviceContext));

            this.OnLastPage = false;
            this.ContinuationToken = default(string);
            this.ServiceContext = serviceContext;
            this.Factory = new ComputeTargetFactory(this.ServiceContext);
        }

        public ServiceContext ServiceContext { get; private set; }

        public ComputeTargetFactory Factory { get; private set; }

        public bool OnLastPage { get; private set; }

        public string ContinuationToken { get; private set; }

        public IEnumerable<ComputeTarget> FetchNextPage()
        {
            return this.FetchNextPageAsync().Result;
        }

        public async Task<IEnumerable<ComputeTarget>> FetchNextPageAsync(
            Dictionary<string, List<string>> customHeaders = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            List<ComputeTarget> result = new List<ComputeTarget>();

            if (!this.OnLastPage)
            {
                var restClient = new RestClient(this.ServiceContext.Credentials);
                restClient.BaseUri = this.ServiceContext.AzureResourceManagerEndpoint;
                var mlcClient = new MachineLearningCompute(restClient);

                GeneratedOld.Models.PaginatedComputeResourcesList computeResourceList =
                    await RestCallWrapper.WrapAsync(
                        () => mlcClient.ListByWorkspaceWithHttpMessagesAsync(
                            this.ServiceContext.SubscriptionId.ToString(),
                            this.ServiceContext.ResourceGroupName,
                            this.ServiceContext.WorkspaceName,
                            this.ContinuationToken,
                            customHeaders: customHeaders,
                            cancellationToken: cancellationToken)).ConfigureAwait(false);

                if (!string.IsNullOrEmpty(computeResourceList.NextLink))
                {
                    this.ContinuationToken = StaticHelpers.ExtractSingleQueryParamFromUrl(
                        computeResourceList.NextLink,
                        "$skipToken");
                }
                else
                {
                    this.ContinuationToken = null;
                }
                this.OnLastPage = string.IsNullOrEmpty(this.ContinuationToken);

                foreach (var computeData in computeResourceList.Value)
                {
                    result.Add(this.Factory.ConvertFromDto(computeData));
                }
            }
            return result;
        }
    }
}
