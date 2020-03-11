// <copyright file="ExperimentPageFetcher.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

using System.Collections.Generic;

using System.Threading;
using System.Threading.Tasks;

using Azure.MachineLearning.Services.GeneratedOld;
using Azure.MachineLearning.Services.GeneratedOld.Models;

namespace Azure.MachineLearning.Services.Experiments
{
    public class ExperimentPageFetcher : IPageFetcher<Experiment>
    {
        public ExperimentPageFetcher(
            ServiceContext serviceContext,
            string filter,
            IList<string> order,
            string sortOrder,
            int? top)
        {
            Throw.IfNull(serviceContext, nameof(serviceContext));

            this.ServiceContext = serviceContext;
            this.FilterString = filter;
            this.OrderBy = order;
            this.SortOrder = sortOrder;
            this.Top = top;

            this.OnLastPage = false;
            this.ContinuationToken = null;
        }

        public ServiceContext ServiceContext { get; private set; }

        public string FilterString { get; private set; }

        public IList<string> OrderBy { get; private set; }

        public string SortOrder { get; private set; }

        public int? Top { get; private set; }

        public bool OnLastPage { get; private set; }

        public string ContinuationToken { get; private set; }

        public IEnumerable<Experiment> FetchNextPage()
        {
            return this.FetchNextPageAsync().Result;
        }

        public async Task<IEnumerable<Experiment>> FetchNextPageAsync(
            Dictionary<string, List<string>> customHeaders = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var result = new List<Experiment>();

            if (!this.OnLastPage)
            {
                var restClient = new RestClient(this.ServiceContext.Credentials);
                restClient.BaseUri = this.ServiceContext.ExperimentationEndpoint;

                PaginatedExperimentDto response = await RestCallWrapper.WrapAsync(()
                    => restClient.Experiment.ListWithHttpMessagesAsync(
                       this.ServiceContext.SubscriptionId,
                       this.ServiceContext.ResourceGroupName,
                       this.ServiceContext.WorkspaceName,
                       this.FilterString,
                       this.ContinuationToken,
                       this.OrderBy,
                       this.SortOrder,
                       this.Top,
                       customHeaders: customHeaders,
                       cancellationToken: cancellationToken)).ConfigureAwait(false);

                this.ContinuationToken = response.ContinuationToken;
                this.OnLastPage = this.ContinuationToken == null;

                foreach (var experimentDto in response.Value)
                {
                    result.Add(new Experiment(this.ServiceContext, experimentDto));
                }
            }
            return result;
        }
    }
}
