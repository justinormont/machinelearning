// <copyright file="MultiClassificationHandler.cs" company="Microsoft">
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
    internal sealed class MultiClassificationHandler : ModelBuilderProgressHandler<MulticlassClassificationMetrics, MulticlassClassificationMetric, MulticlassExperimentSettings>
    {
        public MultiClassificationHandler(MLContext context, MulticlassExperimentSettings settings, CancellationToken? ct)
            : base(context, settings, ct)
        {
            this.MetricsAgent = new MultiMetricsAgent(context, settings.OptimizingMetric);
            this.IsMaximizing = new OptimizingMetricInfo(settings.OptimizingMetric).IsMaximizing;
        }

        public override TaskKind TaskKind => TaskKind.MulticlassClassification;

        internal override AutoMLIterationInfo<MulticlassClassificationMetrics> GenerateAlgorithmIterationEventArg(RunDetail<MulticlassClassificationMetrics> value)
        {
            return new AutoMLIterationInfo<MulticlassClassificationMetrics>()
            {
                RunDetails = value,
                Score = Math.Round(this.GetScore(value), 4),
                TrainerName = value.TrainerName,
                RuntimeInSeconds = Math.Round(value.RuntimeInSeconds, 1),
                Metrics = new Dictionary<string, double>()
                {
                    { "MicroAccuracy", Math.Round(value.ValidationMetrics.MicroAccuracy, 4) },
                    { "MacroAccuracy", Math.Round(value.ValidationMetrics.MacroAccuracy, 4) },
                },
            };
        }

        internal override string Iteration(RunDetail<MulticlassClassificationMetrics> value, int iterationIndex)
        {
            return ConsolePrinter.PrintMetrics(iterationIndex, value.TrainerName, value.ValidationMetrics, this.BestResultAlgorithmsInfo.Score, value.RuntimeInSeconds);
        }

        internal override string IterationSummary(IEnumerable<RunDetail<MulticlassClassificationMetrics>> results, int count)
        {
            return ConsolePrinter.PrintIterationSummary(results, this.Settings.OptimizingMetric, count);
        }

        internal override string MetricsHeader()
        {
            return ConsolePrinter.PrintMulticlassClassificationMetricsHeader();
        }
    }
}
