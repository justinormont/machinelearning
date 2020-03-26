// <copyright file="ComputeTargetProvisionSettings.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

using System;

namespace Azure.MachineLearning.Services.Compute
{
    public abstract class ComputeTargetProvisionSettings
    {
        public abstract GeneratedOld.Models.ComputeResource BuildDTO(Guid subscriptionid);
    }
}
