// <copyright file="BinaryClassificationExperiment.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

using System.Threading;
using Microsoft.ML.AutoML;
using Microsoft.ML.Data;

namespace Microsoft.ML.ModelBuilder.AutoMLService.Experiments
{
    internal class BinaryClassificationExperiment : AutoMLExperiment<BinaryClassificationMetrics, BinaryClassificationMetric, BinaryExperimentSettings>
    {
        public BinaryClassificationExperiment(MLContext context, ILocalAutoMLTrainParameters settings)
            : base(context, settings, AutoMLServiceLogger.Instance)
        {
        }

        protected override ModelBuilderProgressHandler<BinaryClassificationMetrics, BinaryClassificationMetric, BinaryExperimentSettings> Handler { get; set; }

        protected override ExperimentBase<BinaryClassificationMetrics, BinaryExperimentSettings> CreateExperiment(CancellationToken cancellationToken)
        {
            var experimentSetting = new BinaryExperimentSettings()
            {
                MaxExperimentTimeInSeconds = (uint)this.Settings.TrainTime,
                CancellationToken = cancellationToken,
            };
            this.Handler = new BinaryClassificationHandler(this.Context, experimentSetting, null);
            return this.Context.Auto().CreateBinaryClassificationExperiment(experimentSetting);
        }
    }
}
