// <copyright file="ComputeTargetConfiguration.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

using System.Threading.Tasks;

namespace Azure.MachineLearning.Services.Compute
{
    public abstract class ComputeTargetConfiguration
    {
        public abstract Task<ComputeTarget> CreateIfNotExistAsync(ServiceContext serviceContext);
    }
}
