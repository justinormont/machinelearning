// <copyright file="ExperimentResult.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

using Microsoft.ML.AutoML;

namespace Microsoft.ML.ModelBuilder.AutoMLService.Experiments
{
    internal class ExperimentResult : IExperimentResult
    {
        public ITransformer BestModel { get; set; }

        public Pipeline BestPipeline { get; set; }

        public TrainResult TrainResult { get; set; }
    }
}
