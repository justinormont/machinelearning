// <copyright file="RegressionHandler.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.ML.AutoML;
using Microsoft.ML.Data;
using Microsoft.ML.ModelBuilder.AutoMLService;
using Microsoft.ML.ModelBuilder.Utils;

namespace Microsoft.ML.ModelBuilder
{
    internal class RegressionHandler : ModelBuilderProgressHandler<RegressionMetrics, RegressionMetric, RegressionExperimentSettings>
    {
        public RegressionHandler(MLContext context, RegressionExperimentSettings settings, CancellationToken? ct)
            : base(context, settings, ct)
        {
            this.MetricsAgent = new RegressionMetricsAgent(context, settings.OptimizingMetric);
            this.IsMaximizing = new OptimizingMetricInfo(settings.OptimizingMetric).IsMaximizing;
        }

        public override TaskKind TaskKind => TaskKind.Regression;

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
                    { "RSquared", Math.Round(value.ValidationMetrics.RSquared, 4) },
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
