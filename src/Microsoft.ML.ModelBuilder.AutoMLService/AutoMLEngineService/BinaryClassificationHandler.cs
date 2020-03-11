// <copyright file="BinaryClassificationHandler.cs" company="Microsoft">
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
    internal sealed class BinaryClassificationHandler : ModelBuilderProgressHandler<BinaryClassificationMetrics, BinaryClassificationMetric, BinaryExperimentSettings>
    {
        public BinaryClassificationHandler(MLContext context, BinaryExperimentSettings settings, CancellationToken? ct)
            : base(context, settings, ct)
        {
            this.MetricsAgent = new BinaryMetricsAgent(context, settings.OptimizingMetric);
            this.IsMaximizing = new OptimizingMetricInfo(settings.OptimizingMetric).IsMaximizing;
        }

        public override TaskKind TaskKind { get => TaskKind.BinaryClassification; }

        internal override AutoMLIterationInfo<BinaryClassificationMetrics> GenerateAlgorithmIterationEventArg(RunDetail<BinaryClassificationMetrics> value)
        {
            return new AutoMLIterationInfo<BinaryClassificationMetrics>()
            {
                RunDetails = value,
                Score = Math.Round(this.GetScore(value), 4),
                TrainerName = value.TrainerName,
                RuntimeInSeconds = Math.Round(value.RuntimeInSeconds, 1),
                Metrics = new Dictionary<string, double>()
                {
                    { "Accuracy", Math.Round(value.ValidationMetrics.Accuracy, 4) },
                    { "AUC", Math.Round(value.ValidationMetrics.AreaUnderRocCurve, 4) },
                    { "AUPRC", Math.Round(value.ValidationMetrics.AreaUnderPrecisionRecallCurve, 4) },
                    { "F1-score", Math.Round(value.ValidationMetrics.F1Score, 4) },
                },
            };
        }

        internal override string Iteration(RunDetail<BinaryClassificationMetrics> value, int iterationIndex)
        {
            return ConsolePrinter.PrintMetrics(iterationIndex, value.TrainerName, value.ValidationMetrics, this.BestResultAlgorithmsInfo.Score, value.RuntimeInSeconds);
        }

        internal override string IterationSummary(IEnumerable<RunDetail<BinaryClassificationMetrics>> results, int count)
        {
            return ConsolePrinter.PrintIterationSummary(results, this.Settings.OptimizingMetric, count);
        }

        internal override string MetricsHeader()
        {
            return ConsolePrinter.PrintBinaryClassificationMetricsHeader();
        }
    }
}
