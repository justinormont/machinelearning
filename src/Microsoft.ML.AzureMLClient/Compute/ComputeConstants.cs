// <copyright file="ComputeConstants.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

using System.Collections.Generic;

namespace Azure.MachineLearning.Services.Compute
{
    public static class ComputeConstants
    {
        public static readonly List<string> ReservedNames =
            new List<string> { "amlcompute", "local", "containerinstance" };
    }
}
