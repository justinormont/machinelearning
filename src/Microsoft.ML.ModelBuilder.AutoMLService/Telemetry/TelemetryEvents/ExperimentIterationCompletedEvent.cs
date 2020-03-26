// <copyright file="ExperimentIterationCompletedEvent.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.ML.AutoML;
using Newtonsoft.Json;

namespace Microsoft.ML.ModelBuilder.AutoMLService.Telemetry
{
    /// <summary>
    /// Telemetry event for completion of experiment iteration.
    /// </summary>
    internal static class ExperimentIterationCompletedEvent
    {
        public static void TrackEvent<TMetrics>(
            int iterationNum,
            RunDetail<TMetrics> runDetail,
            double score,
            TaskKind machineLearningTask)
        {
            AutoMLTelemetry.Instance.TrackEvent(
                "ExperimentIterationCompleted",
                new Dictionary<string, string>()
                {
                    { "IterationNum", iterationNum.ToString() },
                    { "MachineLearningTask", machineLearningTask.ToString() },
                    { "Metrics", GetMetricsStr(runDetail.ValidationMetrics) },
                    { "PeakMemory", Process.GetCurrentProcess().PeakWorkingSet64.ToString() },
                    { "Pipeline", AutoMLTelemetry.GetSanitizedPipelineStr(runDetail.Pipeline) },
                    { "PipelineInferenceTimeInSeconds", runDetail.PipelineInferenceTimeInSeconds.ToString() },
                    { "Score", score.ToString() },
                    { "TrainerName", runDetail.TrainerName },
                },
                TimeSpan.FromSeconds(runDetail.RuntimeInSeconds),
                runDetail.Exception);
        }

        private static string GetMetricsStr<TMetrics>(TMetrics metrics)
        {
            if (metrics == null)
            {
                return null;
            }

            return JsonConvert.SerializeObject(metrics);
        }
    }
}
