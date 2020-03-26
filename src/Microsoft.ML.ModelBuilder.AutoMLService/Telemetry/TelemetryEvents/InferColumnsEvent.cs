// <copyright file="InferColumnsEvent.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.ML.AutoML;

namespace Microsoft.ML.ModelBuilder.AutoMLService.Telemetry
{
    /// <summary>
    /// Telemetry event for AutoML column inferencing.
    /// </summary>
    internal static class InferColumnsEvent
    {
        public static void TrackEvent(
            ColumnInformation inferredColumns,
            TimeSpan duration)
        {
            var properties = new Dictionary<string, string>();

            // Include count of each column type present as a property
            var columnsByPurpose = ColumnInformationUtil.CountColumnsByPurpose(inferredColumns);
            var totalColumnCount = 0;
            foreach (var kvp in columnsByPurpose)
            {
                totalColumnCount += kvp.Value;
                properties[kvp.Key + "ColumnCount"] = kvp.Value.ToString();
            }

            properties["ColumnCount"] = totalColumnCount.ToString();
            properties["PeakMemory"] = Process.GetCurrentProcess().PeakWorkingSet64.ToString();

            AutoMLTelemetry.Instance.TrackEvent("InferColumns", properties, duration);
        }
    }
}
