// <copyright file="RegressionExperiment.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

using System.Threading;
using Microsoft.ML.AutoML;
using Microsoft.ML.Data;

namespace Microsoft.ML.ModelBuilder.AutoMLService.Experiments
{
    internal class RegressionExperiment : AutoMLExperiment<RegressionMetrics, RegressionMetric, RegressionExperimentSettings>
    {
        public RegressionExperiment(MLContext context, ILocalAutoMLTrainParameters settings)
            : base(context, settings, AutoMLServiceLogger.Instance)
        {
        }

        protected override ModelBuilderProgressHandler<RegressionMetrics, RegressionMetric, RegressionExperimentSettings> Handler { get; set; }

        protected override ExperimentBase<RegressionMetrics, RegressionExperimentSettings> CreateExperiment(CancellationToken cancellationToken)
        {
            var experimentSetting = new RegressionExperimentSettings()
            {
                MaxExperimentTimeInSeconds = (uint)this.Settings.TrainTime,
                CancellationToken = cancellationToken,
            };

            this.Handler = new RegressionHandler(this.Context, experimentSetting, null);
            return this.Context.Auto().CreateRegressionExperiment(experimentSetting);
        }
    }
}
