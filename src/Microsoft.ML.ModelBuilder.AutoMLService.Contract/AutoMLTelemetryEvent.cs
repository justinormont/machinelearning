// <copyright file="AutoMLTelemetryEvent.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.ML.ModelBuilder.AutoMLService.Contract
{
    public class AutoMLTelemetryEvent
    {
        public string EventName { get; set; }

        public IDictionary<string, string> Properties { get; set; }
    }
}
