// <copyright file="RunMetricPageFetcher.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Azure.MachineLearning.Services.GeneratedOld;
using Azure.MachineLearning.Services.GeneratedOld.Models;

namespace Azure.MachineLearning.Services.Metrics
{
    public class RunMetricPageFetcher : IPageFetcher<RunMetric>
    {
        public RunMetricPageFetcher(
            ServiceContext serviceContext,
            string experimentName,
            string filter,
            IList<string> orderBy,
            string sortOrder,
            int? top,
            string mergeStrategyType,
            string mergeStrategyOptions,
            string mergeStrategySettingsVersion,
            string mergeStrategySettingsSelectMetrics)
        {
            Throw.IfNull(serviceContext, nameof(serviceContext));
            Throw.IfNullOrEmpty(experimentName, nameof(experimentName));

            this.ServiceContext = serviceContext;
            this.ExperimentName = experimentName;

            this.Filter = filter;
            this.OrderBy = orderBy;
            this.SortOrder = sortOrder;
            this.Top = top;
            this.MergeStrategyType = mergeStrategyType;
            this.MergeStrategyOptions = mergeStrategyOptions;
            this.MergeStrategySettingsVersion = mergeStrategySettingsVersion;
            this.MergeStrategySettingsSelectMetrics = mergeStrategySettingsSelectMetrics;

            this.OnLastPage = false;
            this.ContinuationToken = null;
        }

        public ServiceContext ServiceContext { get; private set; }

        public string ExperimentName { get; private set; }

        public string Filter { get; private set; }

        public IList<string> OrderBy { get; private set; }

        public string SortOrder { get; private set; }

        public int? Top { get; private set; }

        public string MergeStrategyType { get; private set; }

        public string MergeStrategyOptions { get; private set; }

        public string MergeStrategySettingsVersion { get; private set; }

        public string MergeStrategySettingsSelectMetrics { get; private set; }

        public string ContinuationToken { get; private set; }

        public bool OnLastPage { get; private set; }

        public IEnumerable<RunMetric> FetchNextPage()
        {
            return this.FetchNextPageAsync().Result;
        }

        public async Task<IEnumerable<RunMetric>> FetchNextPageAsync(
            Dictionary<string, List<string>> customHeaders = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var results = new List<RunMetric>();

            if (!this.OnLastPage)
            {
                var restClient = new RestClient(this.ServiceContext.Credentials);
                restClient.BaseUri = this.ServiceContext.MetricsEndpoint;

                PaginatedRunMetricDto response = await RestCallWrapper.WrapAsync(
                    () => restClient.RunMetric.ListWithHttpMessagesAsync(
                        this.ServiceContext.SubscriptionId,
                        this.ServiceContext.ResourceGroupName,
                        this.ServiceContext.WorkspaceName,
                        this.ExperimentName,
                        this.Filter,
                        this.ContinuationToken,
                        this.OrderBy,
                        this.SortOrder,
                        this.Top,
                        this.MergeStrategyType,
                        this.MergeStrategyOptions,
                        this.MergeStrategySettingsVersion,
                        this.MergeStrategySettingsSelectMetrics,
                        customHeaders: customHeaders,
                        cancellationToken: cancellationToken)).ConfigureAwait(false);

                this.ContinuationToken = response.ContinuationToken;
                this.OnLastPage = this.ContinuationToken == null;

                results = response.Value.Select(x => new RunMetric(x)).ToList();
            }

            return results;
        }
    }
}
