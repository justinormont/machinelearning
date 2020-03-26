// <copyright file="RunStats.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Azure.MachineLearning.Services;
using Azure.MachineLearning.Services.Metrics;
using Azure.MachineLearning.Services.Runs;
using Newtonsoft.Json.Linq;

namespace AzureML
{
    internal class RunStats
    {
        public string GetPrimaryMetricFromProperties(Run run)
        {
            var parsedJson = JObject.Parse(run.Properties["AMLSettingsJsonString"]);
            return parsedJson["primary_metric"].ToString();
        }

        public async Task<(Run bestRun, double bestScore)> GetBestRunAsync(
            IPageFetcher<Run> childRunPageFetcher,
            string queryMetric,
            Dictionary<string, List<string>> customHeaders = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var bestScore = 0.0;
            Run bestRun = null;
            do
            {
                var childRuns = await childRunPageFetcher.FetchNextPageAsync(
                    customHeaders,
                    cancellationToken).ConfigureAwait(false);

                var innerBestRun = await this.GetBestRunAsync(childRuns, queryMetric, customHeaders, cancellationToken);

                if (innerBestRun.bestScore > bestScore)
                {
                    bestScore = innerBestRun.bestScore;
                    bestRun = innerBestRun.bestRun;
                }
            }
            while (!childRunPageFetcher.OnLastPage);

            return (bestRun, bestScore);
        }

        public async Task<(Run bestRun, double bestScore)> GetBestRunAsync(
            IEnumerable<Run> childRuns,
            string queryMetric,
            Dictionary<string, List<string>> customHeaders = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var bestScore = 0.0;
            Run bestRun = null;

            foreach (var child in childRuns)
            {
                var childMetricPagedList = child.GetMetricsPagedList();

                double bestMetricScore = await this.GetBestMetricAsync(
                    childMetricPagedList,
                    queryMetric,
                    customHeaders,
                    cancellationToken).ConfigureAwait(false);

                if (bestMetricScore > bestScore)
                {
                    bestScore = bestMetricScore;
                    bestRun = child;
                }
            }

            return (bestRun, bestScore);
        }

        public async Task<double> GetBestMetricAsync(
            IPageFetcher<RunMetric> childRunMetricPageFetcher,
            string queryMetric,
            Dictionary<string, List<string>> customHeaders = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var bestScore = 0.0;
            do
            {
                IEnumerable<RunMetric> metrics = await childRunMetricPageFetcher.FetchNextPageAsync(
                    customHeaders,
                    cancellationToken).ConfigureAwait(false);

                foreach (var metric in metrics)
                {
                    foreach (var cell in metric.Cells)
                    {
                        if (cell.TryGetValue(queryMetric, out var cellValue))
                        {
                            double metricValue = Convert.ToDouble(cellValue);
                            bestScore = Math.Max(bestScore, metricValue);
                        }
                    }
                }
            }
            while (!childRunMetricPageFetcher.OnLastPage);

            return bestScore;
        }
    }
}
