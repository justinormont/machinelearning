// <copyright file="TrainResult.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

using System.Collections.Generic;

namespace Microsoft.ML.ModelBuilder.AutoMLService
{
    public class TrainResult
    {
        public IList<AlgorithmIterationEventArgs> Algorithms { get; set; }

        public string Task { get; set; }

        public string Dataset { get; set; }

        public string Predict { get; set; }

        public string ExperimentTime { get; set; }

        public string ModelsExplored { get; set; }
    }
}