// <copyright file="RunPageFetcher.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Azure.MachineLearning.Services.GeneratedOld.Models;

namespace Azure.MachineLearning.Services.Runs
{
    public abstract class RunPageFetcher : IPageFetcher<Run>
    {
        public RunPageFetcher(
            ServiceContext serviceContext,
            string experimentName,
            FactoryManager<IRunFactory> factories,
            string filter = default(string),
            IList<string> orderby = default(IList<string>),
            string sortorder = default(string),
            int? top = default(int?))
        {
            Throw.IfNull(serviceContext, nameof(serviceContext));
            Throw.IfNullOrEmpty(experimentName, nameof(experimentName));
            Throw.IfNull(factories, nameof(factories));

            this.ServiceContext = serviceContext;
            this.ExperimentName = experimentName;
            this.Factories = factories;
            this.OnLastPage = false;

            this.Filter = filter;
            this.OrderBy = orderby;
            this.SortOrder = sortorder;
            this.Top = top;
        }

        public ServiceContext ServiceContext { get; private set; }

        public string ExperimentName { get; private set; }

        public FactoryManager<IRunFactory> Factories { get; private set; }

        public string Filter { get; private set; }

        public IList<string> OrderBy { get; private set; }

        public string SortOrder { get; private set; }

        public int? Top { get; private set; }

        public bool OnLastPage { get; private set; }

        public string ContinuationToken { get; private set; }

        public IEnumerable<Run> FetchNextPage()
        {
            return this.FetchNextPageAsync().Result;
        }

        public async Task<IEnumerable<Run>> FetchNextPageAsync(
            Dictionary<string, List<string>> customHeaders = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var result = new List<Run>();
            if (!this.OnLastPage)
            {
                PaginatedRunDto response = await this.GetNextResultsAsync(customHeaders, cancellationToken).ConfigureAwait(false);

                this.ContinuationToken = response.ContinuationToken;
                if (string.IsNullOrEmpty(this.ContinuationToken))
                {
                    this.OnLastPage = true;
                }

                foreach (var runData in response.Value)
                {
                    IRunFactory f = this.Factories.GetFactory(runData.RunType);
                    result.Add(f.Create(this.ServiceContext, this.ExperimentName, this.Factories, runData));
                }
            }
            return result;
        }

        protected abstract Task<PaginatedRunDto> GetNextResultsAsync(Dictionary<string, List<string>> customHeaders, CancellationToken cancellationToken);
    }
}
