// <copyright file="ComputeTargetAttachSettings.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace Azure.MachineLearning.Services.Compute
{
    public abstract class ComputeTargetAttachSettings
    {
        public abstract GeneratedOld.Models.ComputeResource BuildDTO(ServiceContext serviceContext);
    }
}
