// <copyright file="ModelBuilderProgressHandler.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.ML.AutoML;
using Microsoft.ML.ModelBuilder.AutoMLService;
using Microsoft.ML.ModelBuilder.AutoMLService.Telemetry;

namespace Microsoft.ML.ModelBuilder
{
    internal abstract class ModelBuilderProgressHandler<TMetrics, TMetric, TExperimentSettings> : AutoMLEngineProgressHandler<TMetrics, TMetric>
        where TMetric : IConvertible
        where TMetrics : class
        where TExperimentSettings : ExperimentSettings
    {
        private readonly CancellationToken ct;
        private int iterationIndex = 0;

        protected ModelBuilderProgressHandler(MLContext context, TExperimentSettings settings, CancellationToken? ct)
        {
            this.ct = ct ?? CancellationToken.None;
            this.Settings = settings;
        }

        public TExperimentSettings Settings { get; }

        public IMetricsAgent<TMetrics> MetricsAgent { get; set; }

        public bool IsMaximizing { get; protected set; }

        protected AutoMLIterationInfo<TMetrics> BestResultAlgorithmsInfo { get; private set; }

        public virtual double GetScore(RunDetail<TMetrics> value)
        {
            return this.MetricsAgent.GetScore(value.ValidationMetrics);
        }

        public override void Report(RunDetail<TMetrics> value)
        {
            if (value.Exception != null)
            {
                this.OnConsoleOutputEvent(this, new AutoMLService.DataReceivedEventArgs()
                {
                    Data = value.Exception.Message,
                });
                this.OnAlgorithmIterationEvent(this, new AutoMLIterationInfo<TMetrics>()
                {
                    RunDetails = value,
                });
                return;
            }

            if (this.ct.IsCancellationRequested)
            {
                // cancel!
                return;
            }

            var iterationEventArgs = this.GenerateAlgorithmIterationEventArg(value);
            iterationEventArgs.IterationIndex = this.iterationIndex;
            this.iterationIndex++;
            if (this.BestResultAlgorithmsInfo == null)
            {
                this.BestResultAlgorithmsInfo = iterationEventArgs;
            }

            this.OnAlgorithmIterationEvent(this, iterationEventArgs);

            // console output
            var iteration = this.Iteration(value, this.iterationIndex);
            this.OnConsoleOutputEvent(this, new DataReceivedEventArgs()
            {
                Data = iteration,
            });

            if (this.MetricComparator(this.GetScore(value), this.BestResultAlgorithmsInfo.Score, this.IsMaximizing) > 0)
            {
                this.BestResultAlgorithmsInfo = iterationEventArgs;
            }

            this.OnBestAlgorithmEvent(this, this.BestResultAlgorithmsInfo);

            // Track ExperimentIterationCompletedEvent event
            ExperimentIterationCompletedEvent.TrackEvent(this.iterationIndex, value, this.GetScore(value), this.TaskKind);
        }

        internal abstract AutoMLIterationInfo<TMetrics> GenerateAlgorithmIterationEventArg(RunDetail<TMetrics> value);

        internal abstract string IterationSummary(IEnumerable<RunDetail<TMetrics>> results, int count);

        internal abstract string Iteration(RunDetail<TMetrics> value, int iterationIndex);

        internal abstract string MetricsHeader();

        protected int MetricComparator(double a, double b, bool isMaximizing)
        {
            return isMaximizing ? a.CompareTo(b) : -a.CompareTo(b);
        }
    }
}
