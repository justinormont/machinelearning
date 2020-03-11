// <copyright file="ExperimentCompletedEvent.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.ML.AutoML;

namespace Microsoft.ML.ModelBuilder.AutoMLService.Telemetry
{
    /// <summary>
    /// Telemetry event for AutoML experiment completion.
    /// </summary>
    internal static class ExperimentCompletedEvent
    {
        public static void TrackEvent<TMetrics>(
            RunDetail<TMetrics> bestRun,
            List<RunDetail<TMetrics>> allRuns,
            TaskKind machineLearningTask,
            TimeSpan duration)
        {
            AutoMLTelemetry.Instance.TrackEvent(
                "ExperimentCompleted",
                new Dictionary<string, string>()
                {
                    { "BestIterationNum", (allRuns.IndexOf(bestRun) + 1).ToString() },
                    { "BestPipeline", AutoMLTelemetry.GetSanitizedPipelineStr(bestRun.Pipeline) },
                    { "BestTrainer", bestRun.TrainerName },
                    { "MachineLearningTask", machineLearningTask.ToString() },
                    { "NumIterations", allRuns.Count().ToString() },
                    { "PeakMemory", Process.GetCurrentProcess().PeakWorkingSet64.ToString() },
                },
                duration);
        }
    }
}
