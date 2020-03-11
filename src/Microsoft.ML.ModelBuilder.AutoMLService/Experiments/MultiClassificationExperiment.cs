// <copyright file="MultiClassificationExperiment.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ML.AutoML;
using Microsoft.ML.Data;

namespace Microsoft.ML.ModelBuilder.AutoMLService.Experiments
{
    internal class MultiClassificationExperiment : AutoMLExperiment<MulticlassClassificationMetrics, MulticlassClassificationMetric, MulticlassExperimentSettings>
    {
        public MultiClassificationExperiment(MLContext context, ILocalAutoMLTrainParameters settings)
            : base(context, settings, AutoMLServiceLogger.Instance)
        {
        }

        protected override ModelBuilderProgressHandler<MulticlassClassificationMetrics, MulticlassClassificationMetric, MulticlassExperimentSettings> Handler { get; set; }

        protected override ExperimentBase<MulticlassClassificationMetrics, MulticlassExperimentSettings> CreateExperiment(CancellationToken cancellationToken)
        {
            var experimentSetting = new MulticlassExperimentSettings()
            {
                MaxExperimentTimeInSeconds = (uint)this.Settings.TrainTime,
                CancellationToken = cancellationToken,
            };
            this.Handler = new MultiClassificationHandler(this.Context, experimentSetting, null);
            return this.Context.Auto().CreateMulticlassClassificationExperiment(experimentSetting);
        }
    }
}
