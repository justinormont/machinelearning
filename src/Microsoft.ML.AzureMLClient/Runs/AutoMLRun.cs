// <copyright file="AutoMLRun.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Azure.MachineLearning.Services.Artifacts;
using Azure.MachineLearning.Services.GeneratedOld.Models;
using Azure.MachineLearning.Services.Metrics;
using Newtonsoft.Json.Linq;

namespace Azure.MachineLearning.Services.Runs
{
    public class AutoMLRun : Run
    {
        private const string RunOrigin = "ExperimentRun";
        private const string DefaultModelName = "model.onnx";

        public AutoMLRun(
            ServiceContext serviceContext,
            string experimentName,
            FactoryManager<IRunFactory> factories,
            RunDto runDto)
            : base(serviceContext, experimentName, factories, runDto)
        {
        }

        public async Task<Run> GetBestRunAsync(
            string metric = null,
            Dictionary<string, List<string>> customHeaders = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            // If the user does not specify a metric, then use the primary metric from the
            // AutoML runs.
            metric = metric ?? GetPrimaryMetricFromProperties();

            IPageFetcher<Run> childRunPageFetcher = GetPagedListOfChildren();
            Run bestRun = await GetBestRunAsync(
                childRunPageFetcher,
                metric,
                customHeaders,
                cancellationToken).ConfigureAwait(false);

            if (bestRun == null)
            {
                throw new InvalidOperationException($"Could not find a model with a valid score for metric {metric}");
            }

            return bestRun;
        }

        private string GetPrimaryMetricFromProperties()
        {
            var parsedJson = JObject.Parse(Properties["AMLSettingsJsonString"]);
            return parsedJson["primary_metric"].ToString();
        }

        private async Task<Run> GetBestRunAsync(
            IPageFetcher<Run> childRunPageFetcher,
            string queryMetric,
            Dictionary<string, List<string>> customHeaders,
            CancellationToken cancellationToken)
        {
            var bestScore = 0.0;
            Run bestRun = null;
            do
            {
                IEnumerable<Run> childRuns = await childRunPageFetcher.FetchNextPageAsync(
                    customHeaders,
                    cancellationToken).ConfigureAwait(false);

                foreach (var child in childRuns)
                {
                    IPageFetcher<RunMetric> childMetricPagedList = child.GetMetricsPagedList();

                    double bestMetricScore = await GetBestMetricAsync(
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
            }
            while (!childRunPageFetcher.OnLastPage);

            return bestRun;
        }

        private async Task<double> GetBestMetricAsync(
            IPageFetcher<RunMetric> childRunMetricPageFetcher,
            string queryMetric,
            Dictionary<string, List<string>> customHeaders,
            CancellationToken cancellationToken)
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
                            double metricValue = (double)cellValue;
                            bestScore = Math.Max(bestScore, metricValue);
                        }
                    }
                }
            }
            while (!childRunMetricPageFetcher.OnLastPage);

            return bestScore;
        }

        public async Task DownloadRunArtifactAsync(
            Run bestRun,
            string artifactPath,
            FileInfo downloadFilePath,
            Dictionary<string, List<string>> customHeaders = null,
            CancellationToken cancellationToken = default)
        {
            string origin = RunOrigin;
            string container = bestRun.DataContainerId;

            downloadFilePath = downloadFilePath ??
                new FileInfo(Path.Combine(Directory.GetCurrentDirectory(), DefaultModelName));

            var artifactOperations = new ArtifactOperations(ServiceContext);

            await artifactOperations.DownloadArtifactAsync(
                origin: origin,
                container: container,
                path: artifactPath,
                outputFileLocation: downloadFilePath,
                overwrite: true,
                customHeaders: customHeaders,
                cancellationToken: cancellationToken).ConfigureAwait(false);
        }
    }
}