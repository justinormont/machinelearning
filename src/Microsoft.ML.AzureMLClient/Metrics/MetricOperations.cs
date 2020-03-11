// <copyright file="MetricOperations.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

using System.Collections.Generic;

namespace Azure.MachineLearning.Services.Metrics
{
    public class MetricOperations
    {
        public MetricOperations(ServiceContext serviceContext, string experimentName)
        {
            Throw.IfNull(serviceContext, nameof(serviceContext));
            Throw.IfNullOrEmpty(experimentName, nameof(experimentName));

            this.ServiceContext = serviceContext;
            this.ExperimentName = experimentName;
        }

        public ServiceContext ServiceContext { get; private set; }

        public string ExperimentName { get; private set; }

        public IPageFetcher<RunMetric> GetRunMetricPagedList(
            string filter = default(string),
            IList<string> orderBy = default(IList<string>),
            string sortOrder = default(string),
            int? top = default(int?),
            string mergeStrategyType = default(string),
            string mergeStrategyOptions = default(string),
            string mergeStrategySettingsVersion = default(string),
            string mergeStrategySettingsSelectMetrics = default(string))
        {
            return new RunMetricPageFetcher(
                this.ServiceContext,
                this.ExperimentName,
                filter,
                orderBy,
                sortOrder,
                top,
                mergeStrategyType,
                mergeStrategyOptions,
                mergeStrategySettingsVersion,
                mergeStrategySettingsSelectMetrics);
        }

        public IEnumerable<RunMetric> ListRunMetrics(
            string filter = default(string),
            IList<string> orderBy = default(IList<string>),
            string sortOrder = default(string),
            int? top = default(int?),
            string mergeStrategyType = default(string),
            string mergeStrategyOptions = default(string),
            string mergeStrategySettingsVersion = default(string),
            string mergeStrategySettingsSelectMetrics = default(string))
        {
            var lister = new LazyEnumerator<RunMetric>();
            lister.Fetcher = this.GetRunMetricPagedList(
                filter,
                orderBy,
                sortOrder,
                top,
                mergeStrategyType,
                mergeStrategyOptions,
                mergeStrategySettingsVersion,
                mergeStrategySettingsSelectMetrics);

            return lister;
        }
    }
}
