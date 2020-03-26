// <copyright file="RecommendationHandler.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ML.AutoML;
using Microsoft.ML.Data;
using Microsoft.ML.ModelBuilder.Utils;

namespace Microsoft.ML.ModelBuilder.AutoMLService.AutoMLEngineService
{
    internal class RecommendationHandler : ModelBuilderProgressHandler<RegressionMetrics, RegressionMetric, RecommendationExperimentSettings>
    {
        public RecommendationHandler(MLContext context, RecommendationExperimentSettings settings, CancellationToken? ct)
            : base(context, settings, ct)
        {
            this.MetricsAgent = new RegressionMetricsAgent(context, settings.OptimizingMetric);
            this.IsMaximizing = new OptimizingMetricInfo(settings.OptimizingMetric).IsMaximizing;
        }

        public override TaskKind TaskKind => TaskKind.Recommendation;

        internal override AutoMLIterationInfo<RegressionMetrics> GenerateAlgorithmIterationEventArg(RunDetail<RegressionMetrics> value)
        {
            return new AutoMLIterationInfo<RegressionMetrics>()
            {
                RunDetails = value,
                Score = Math.Round(this.GetScore(value), 4),
                TrainerName = value.TrainerName,
                RuntimeInSeconds = Math.Round(value.RuntimeInSeconds, 1),
                Metrics = new Dictionary<string, double>()
                {
                    { "RMSE", Math.Round(value.ValidationMetrics.RSquared, 4) },
                    { "Absolute-loss", Math.Round(value.ValidationMetrics.MeanAbsoluteError, 4) },
                    { "Squared-loss", Math.Round(value.ValidationMetrics.MeanSquaredError, 4) },
                    { "RMS-loss", Math.Round(value.ValidationMetrics.RootMeanSquaredError, 4) },
                },
            };
        }

        internal override string Iteration(RunDetail<RegressionMetrics> value, int iterationIndex)
        {
            return ConsolePrinter.PrintMetrics(iterationIndex, value.TrainerName, value.ValidationMetrics, this.BestResultAlgorithmsInfo.Score, value.RuntimeInSeconds);
        }

        internal override string IterationSummary(IEnumerable<RunDetail<RegressionMetrics>> results, int count)
        {
            return ConsolePrinter.PrintIterationSummary(results, this.Settings.OptimizingMetric, count);
        }

        internal override string MetricsHeader()
        {
            return ConsolePrinter.PrintRegressionMetricsHeader("RSquared");
        }
    }
}