// <copyright file="IExperimentResult.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

using Microsoft.ML.AutoML;

namespace Microsoft.ML.ModelBuilder.AutoMLService.Experiments
{
    internal interface IExperimentResult
    {
        ITransformer BestModel { get; set; }

        Pipeline BestPipeline { get; set; }

        TrainResult TrainResult { get; set; }
    }
}