// <copyright file="RecommendationExperiment.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

using System.Threading;
using Microsoft.ML.AutoML;
using Microsoft.ML.Data;
using Microsoft.ML.ModelBuilder.AutoMLService.AutoMLEngineService;

namespace Microsoft.ML.ModelBuilder.AutoMLService.Experiments
{
    internal class RecommendationExperiment : AutoMLExperiment<RegressionMetrics, RegressionMetric, RecommendationExperimentSettings>
    {
        public RecommendationExperiment(MLContext context, ILocalAutoMLTrainParameters settings)
            : base(context, settings, AutoMLServiceLogger.Instance)
        {
        }

        protected override ModelBuilderProgressHandler<RegressionMetrics, RegressionMetric, RecommendationExperimentSettings> Handler { get; set; }

        protected override ExperimentBase<RegressionMetrics, RecommendationExperimentSettings> CreateExperiment(CancellationToken cancellationToken)
        {
            var experimentSetting = new RecommendationExperimentSettings()
            {
                MaxExperimentTimeInSeconds = (uint)this.Settings.TrainTime,
                CancellationToken = cancellationToken,
            };

            this.Handler = new RecommendationHandler(this.Context, experimentSetting, null);
            return this.Context.Auto().CreateRecommendationExperiment(experimentSetting);
        }
    }
}
