// <copyright file="AlgorithmIterationEventArgs.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;

namespace Microsoft.ML.ModelBuilder.AutoMLService
{
    public class AlgorithmIterationEventArgs : EventArgs
    {
        public Dictionary<string, double> Metrics { get; set; }

        public double Score { get; set; }

        public string TrainerName { get; set; }

        public double RuntimeInSeconds { get; set; }

        public int IterationIndex { get; set; }

        public bool IsBest { get; set; }
    }
}
