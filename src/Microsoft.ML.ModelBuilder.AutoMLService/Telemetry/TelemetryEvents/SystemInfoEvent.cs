// <copyright file="SystemInfoEvent.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;

namespace Microsoft.ML.ModelBuilder.AutoMLService.Telemetry
{
    /// <summary>
    /// System info telemetry event.
    /// </summary>
    internal class SystemInfoEvent
    {
        public static void TrackEvent()
        {
            AutoMLTelemetry.Instance.TrackEvent(
                "SystemInfo",
                new Dictionary<string, string>
                {
                    { "LogicalCores", Environment.ProcessorCount.ToString() },
                });
        }
    }
}
