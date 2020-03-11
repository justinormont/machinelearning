// <copyright file="ConsolePrinter.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.ML.AutoML;
using Microsoft.ML.Data;
using Microsoft.ML.ModelBuilder.AutoMLService.AutoMLEngineService;

namespace Microsoft.ML.ModelBuilder.Utils
{
    internal class ConsolePrinter
    {
        internal static readonly string TableSeparator = "------------------------------------------------------------------------------------------------------------------";
        private const int Width = 114;

        internal static string PrintMetrics(int iteration, string trainerName, BinaryClassificationMetrics metrics, double bestMetric, double? runtimeInSeconds, int iterationNumber = -1)
        {
            return CreateRow($"{iteration,-4} {trainerName,-35} {metrics?.Accuracy ?? double.NaN,9:F4} {metrics?.AreaUnderRocCurve ?? double.NaN,8:F4} {metrics?.AreaUnderPrecisionRecallCurve ?? double.NaN,8:F4} {metrics?.F1Score ?? double.NaN,9:F4} {runtimeInSeconds.Value,9:F1} {iteration,10}", Width);
        }

        internal static string PrintMetrics(int iteration, string trainerName, MulticlassClassificationMetrics metrics, double bestMetric, double? runtimeInSeconds, int iterationNumber = -1)
        {
            return CreateRow($"{iteration,-4} {trainerName,-35} {metrics?.MicroAccuracy ?? double.NaN,14:F4} {metrics?.MacroAccuracy ?? double.NaN,14:F4} {runtimeInSeconds.Value,9:F1} {iteration,10}", Width);
        }

        internal static string PrintMetrics(int iteration, string trainerName, RegressionMetrics metrics, double bestMetric, double? runtimeInSeconds, int iterationNumber = -1)
        {
            return CreateRow($"{iteration,-4} {trainerName,-35} {metrics?.RSquared ?? double.NaN,8:F4} {metrics?.MeanAbsoluteError ?? double.NaN,13:F2} {metrics?.MeanSquaredError ?? double.NaN,12:F2} {metrics?.RootMeanSquaredError ?? double.NaN,8:F2} {runtimeInSeconds.Value,9:F1} {iteration,10}", Width);
        }

        internal static string PrintBinaryClassificationMetricsHeader()
        {
            return CreateRow($"{string.Empty,-4} {"Trainer",-35} {"Accuracy",9} {"AUC",8} {"AUPRC",8} {"F1-score",9} {"Duration",9} {"#Iteration",10}", Width);
        }

        internal static string PrintMulticlassClassificationMetricsHeader()
        {
            return CreateRow($"{string.Empty,-4} {"Trainer",-35} {"MicroAccuracy",14} {"MacroAccuracy",14} {"Duration",9} {"#Iteration",10}", Width);
        }

        internal static string PrintRegressionMetricsHeader(string rMSEOrRSquared)
        {
            return CreateRow($"{string.Empty,-4} {"Trainer",-35} {rMSEOrRSquared,8} {"Absolute-loss",13} {"Squared-loss",12} {"RMS-loss",8} {"Duration",9} {"#Iteration",10}", Width);
        }

        internal static string ExperimentResultsHeader(string mltask, string datasetName, string labelName, string time, int numModelsExplored)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(string.Empty);
            sb.AppendLine($"===============================================Experiment Results=================================================");
            sb.AppendLine(TableSeparator);
            var header = "Summary";
            sb.AppendLine(CreateRow(header.PadLeft((Width / 2) + (header.Length / 2)), Width));
            sb.AppendLine(TableSeparator);
            sb.AppendLine(CreateRow($"{"ML Task",-7}: {mltask,-20}", Width));
            sb.AppendLine(CreateRow($"{"Dataset",-7}: {datasetName,-25}", Width));
            sb.AppendLine(CreateRow($"{"Label",-6}: {labelName,-25}", Width));
            sb.AppendLine(CreateRow($"{"Total experiment time",-22}: {time} Secs", Width));
            sb.AppendLine(CreateRow($"{"Total number of models explored",-30}: {numModelsExplored}", Width));
            sb.AppendLine(TableSeparator);
            return sb.ToString();
        }

        internal static string CreateRow(string message, int width)
        {
            return "|" + message.PadRight(width - 2) + "|";
        }

        internal static string PrintIterationSummary<TMetrics, TMetric>(IEnumerable<RunDetail<TMetrics>> results, TMetric optimizationMetric, int count)
            where TMetric : IConvertible
        {
            if (typeof(TMetrics) == typeof(BinaryClassificationMetrics))
            {
                return PrintIterationSummary((IEnumerable<RunDetail<BinaryClassificationMetrics>>)(object)results, (BinaryClassificationMetric)(object)optimizationMetric, count);
            }

            if (typeof(TMetrics) == typeof(MulticlassClassificationMetrics))
            {
                return PrintIterationSummary((IEnumerable<RunDetail<MulticlassClassificationMetrics>>)(object)results, (MulticlassClassificationMetric)(object)optimizationMetric, count);
            }

            if (typeof(TMetrics) == typeof(RegressionMetrics))
            {
                 return PrintIterationSummary((IEnumerable<RunDetail<RegressionMetrics>>)(object)results, (RegressionMetric)(object)optimizationMetric, count);
            }

            throw new NotImplementedException();
        }

        internal static string PrintIterationSummary(IEnumerable<RunDetail<BinaryClassificationMetrics>> results, BinaryClassificationMetric optimizationMetric, int count)
        {
            var sb = new StringBuilder();
            var metricsAgent = new BinaryMetricsAgent(null, optimizationMetric);
            var topNResults = BestResultUtil.GetTopNRunResults(results, metricsAgent, count, new OptimizingMetricInfo(optimizationMetric).IsMaximizing);
            var header = $"Top {topNResults?.Count()} models explored";
            sb.AppendLine(CreateRow(header.PadLeft((Width / 2) + (header.Length / 2)), Width));
            sb.AppendLine(TableSeparator);
            sb.AppendLine(PrintBinaryClassificationMetricsHeader());
            int i = 0;
            foreach (var pair in topNResults)
            {
                var result = pair.Item1;
                if (i == 0)
                {
                    // Print top iteration colored.
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    sb.AppendLine(PrintMetrics(++i, result?.TrainerName, result?.ValidationMetrics, metricsAgent.GetScore(result?.ValidationMetrics), result?.RuntimeInSeconds, pair.Item2));
                    Console.ResetColor();
                    continue;
                }

                sb.AppendLine(PrintMetrics(++i, result?.TrainerName, result?.ValidationMetrics, metricsAgent.GetScore(result?.ValidationMetrics), result?.RuntimeInSeconds, pair.Item2));
            }

            sb.AppendLine(TableSeparator);
            return sb.ToString();
        }

        internal static string PrintIterationSummary(IEnumerable<RunDetail<RegressionMetrics>> results, RegressionMetric optimizationMetric, int count)
        {
            var sb = new StringBuilder();
            var metricsAgent = new RegressionMetricsAgent(null, optimizationMetric);
            var topNResults = BestResultUtil.GetTopNRunResults(results, metricsAgent, count, new OptimizingMetricInfo(optimizationMetric).IsMaximizing);
            var header = $"Top {topNResults?.Count()} models explored";
            sb.AppendLine(CreateRow(header.PadLeft((Width / 2) + (header.Length / 2)), Width));
            sb.AppendLine(TableSeparator);
            var rMSEOrRSquaredHeader = "RSquared";
            sb.AppendLine(PrintRegressionMetricsHeader(rMSEOrRSquaredHeader));
            int i = 0;
            foreach (var pair in topNResults)
            {
                var result = pair.Item1;
                if (i == 0)
                {
                    // Print top iteration colored.
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    sb.AppendLine(PrintMetrics(++i, result?.TrainerName, result?.ValidationMetrics, metricsAgent.GetScore(result?.ValidationMetrics), result?.RuntimeInSeconds, pair.Item2));
                    Console.ResetColor();
                    continue;
                }

                sb.AppendLine(PrintMetrics(++i, result?.TrainerName, result?.ValidationMetrics, metricsAgent.GetScore(result?.ValidationMetrics), result?.RuntimeInSeconds, pair.Item2));
            }

            sb.AppendLine(TableSeparator);
            return sb.ToString();
        }

        internal static string PrintIterationSummary(IEnumerable<RunDetail<MulticlassClassificationMetrics>> results, MulticlassClassificationMetric optimizationMetric, int count)
        {
            var sb = new StringBuilder();
            var metricsAgent = new MultiMetricsAgent(null, optimizationMetric);
            var topNResults = BestResultUtil.GetTopNRunResults(results, metricsAgent, count, new OptimizingMetricInfo(optimizationMetric).IsMaximizing);
            var header = $"Top {topNResults?.Count()} models explored";
            sb.AppendLine(CreateRow(header.PadLeft((Width / 2) + (header.Length / 2)), Width));
            sb.AppendLine(TableSeparator);
            sb.AppendLine(PrintMulticlassClassificationMetricsHeader());
            int i = 0;
            foreach (var pair in topNResults)
            {
                var result = pair.Item1;
                if (i == 0)
                {
                    // Print top iteration colored.
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    sb.AppendLine(PrintMetrics(++i, result?.TrainerName, result?.ValidationMetrics, metricsAgent.GetScore(result?.ValidationMetrics), result?.RuntimeInSeconds, pair.Item2));
                    Console.ResetColor();
                    continue;
                }

                sb.AppendLine(PrintMetrics(++i, result?.TrainerName, result?.ValidationMetrics, metricsAgent.GetScore(result?.ValidationMetrics), result?.RuntimeInSeconds, pair.Item2));
            }

            sb.AppendLine(TableSeparator);
            return sb.ToString();
        }
    }
}