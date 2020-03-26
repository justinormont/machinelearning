// <copyright file="AutoMLEngineProgressHandler.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

using System;
using Microsoft.ML.AutoML;
using Microsoft.ML.ModelBuilder.AutoMLService;

namespace Microsoft.ML.ModelBuilder
{
    internal abstract class AutoMLEngineProgressHandler<TMetrics, TMetric> : IProgress<RunDetail<TMetrics>>, IAutoMLEngineEventHandler<TMetrics>
        where TMetric : IConvertible
        where TMetrics : class
    {
        public event EventHandler<AutoMLIterationInfo<TMetrics>> AlgorithmIterationHandler;

        public event EventHandler<AutoMLIterationInfo<TMetrics>> BestAlgorithmHandler;

        public event EventHandler<DataReceivedEventArgs> OutputHandler;

        public abstract TaskKind TaskKind { get; }

        public abstract void Report(RunDetail<TMetrics> value);

        protected virtual void OnAlgorithmIterationEvent(object sender, AutoMLIterationInfo<TMetrics> e)
        {
            this.AlgorithmIterationHandler?.Invoke(this, e);
        }

        protected virtual void OnBestAlgorithmEvent(object sender, AutoMLIterationInfo<TMetrics> e)
        {
            e.IsBest = true;
            this.BestAlgorithmHandler?.Invoke(this, e);
        }

        protected virtual void OnConsoleOutputEvent(object sender, AutoMLService.DataReceivedEventArgs e)
        {
            this.OutputHandler?.Invoke(this, e);
        }
    }
}