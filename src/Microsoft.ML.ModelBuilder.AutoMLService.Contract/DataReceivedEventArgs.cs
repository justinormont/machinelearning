// <copyright file="DataReceivedEventArgs.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

using Microsoft.ML.ModelBuilder.AutoMLService.Contract;
using System;

namespace Microsoft.ML.ModelBuilder.AutoMLService
{
    public enum AutoMLServiceOutputDataType
    {
        IterationHeader = 0,
        IterationInfo = 1,
        ExperimentResult = 2,
        CodeGenerator = 3,
        Other = 4,
    }

    public class DataReceivedEventArgs : EventArgs
    {
        public AutoMLServiceLogLevel LogLevel { get; set; }

        public AutoMLServiceOutputDataType Type { get; set; } = AutoMLServiceOutputDataType.Other;

        public string Data { get; set; }
    }
}
