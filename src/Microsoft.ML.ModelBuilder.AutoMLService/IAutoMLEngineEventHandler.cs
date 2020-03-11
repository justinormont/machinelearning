// <copyright file="IAutoMLEngineEventHandler.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

using System;

namespace Microsoft.ML.ModelBuilder.AutoMLService
{
    public interface IAutoMLEngineEventHandler<TMetrics>
        where TMetrics : class
    {
        event EventHandler<AutoMLIterationInfo<TMetrics>> AlgorithmIterationHandler;

        event EventHandler<AutoMLIterationInfo<TMetrics>> BestAlgorithmHandler;

        event EventHandler<DataReceivedEventArgs> OutputHandler;
    }
}
