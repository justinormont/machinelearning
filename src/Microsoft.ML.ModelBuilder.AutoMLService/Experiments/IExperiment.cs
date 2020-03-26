// <copyright file="IExperiment.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ML.AutoML;
using Microsoft.ML.ModelBuilder.AutoMLService.Contract;

namespace Microsoft.ML.ModelBuilder.AutoMLService.Experiments
{
    internal interface IExperiment
    {
        event EventHandler<RemoteRunStartedEventArgs> RunStarted;

        event EventHandler<AlgorithmIterationEventArgs> AlgorithmIterationCompleted;

        Task<IExperimentResult> ExecuteAsync(IDataView trainData, IDataView validateData, ColumnInformation columnInformation, CancellationToken? ct, CancellationToken? timeout);
    }
}
