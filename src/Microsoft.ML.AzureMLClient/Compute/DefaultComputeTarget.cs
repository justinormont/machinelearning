// <copyright file="DefaultComputeTarget.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

using System;

namespace Azure.MachineLearning.Services.Compute
{
    public class DefaultComputeTarget : ComputeTarget
    {
        public DefaultComputeTarget(
            ServiceContext serviceContext,
            GeneratedOld.Models.ComputeResource computeResourceDto = null)
        {
            Throw.IfNull(serviceContext, nameof(serviceContext));
            Throw.IfNull(computeResourceDto, nameof(computeResourceDto));

            this.ServiceContext = serviceContext;

            this.RefreshFromDto(computeResourceDto);
        }

        public object Properties { get; private set; }

        public override void RefreshFromDto(GeneratedOld.Models.ComputeResource computeResourceDto)
        {
            Throw.IfNull(computeResourceDto, nameof(computeResourceDto));

            this.RefreshCommonFields(computeResourceDto);
            this.Properties = ((dynamic)computeResourceDto).Properties.Properties;
            this.LastRefreshFromDto = DateTime.UtcNow;
        }
    }
}
