// <copyright file="ComputeTypeResource.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace Microsoft.ML.ModelBuilder.AutoMLService
{
    public class ComputeTypeResource
    {
        public string Name { get; set; }

        public string Family { get; set; }

        public int VCPUs { get; set; }

        public int GPUs { get; set; }

        public double MemoryGB { get; set; }
    }
}
