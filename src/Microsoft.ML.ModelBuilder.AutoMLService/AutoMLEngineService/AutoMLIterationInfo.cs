// <copyright file="AutoMLIterationInfo.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

using Microsoft.ML.AutoML;

namespace Microsoft.ML.ModelBuilder.AutoMLService
{
    public class AutoMLIterationInfo<TMetrics> : AlgorithmIterationEventArgs
        where TMetrics : class
    {
        internal RunDetail<TMetrics> RunDetails { get; set; }
    }
}
