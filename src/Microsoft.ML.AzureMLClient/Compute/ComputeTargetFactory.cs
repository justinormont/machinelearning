// <copyright file="ComputeTargetFactory.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace Azure.MachineLearning.Services.Compute
{
    public class ComputeTargetFactory
    {
        public ComputeTargetFactory(ServiceContext serviceContext)
        {
            Throw.IfNull(serviceContext, nameof(serviceContext));
            this.ServiceContext = serviceContext;
        }

        public ServiceContext ServiceContext { get; private set; }

        public ComputeTarget ConvertFromDto(
            GeneratedOld.Models.ComputeResource computeResourceDto)
        {
            Throw.IfNull(computeResourceDto, nameof(computeResourceDto));

            if (computeResourceDto.Properties is GeneratedOld.Models.VirtualMachine)
            {
                return new VirtualMachineTarget(this.ServiceContext, computeResourceDto);
            }

            if (computeResourceDto.Properties is GeneratedOld.Models.AmlCompute)
            {
                return new AzureMLComputeTarget(this.ServiceContext, computeResourceDto);
            }

            if (computeResourceDto.Properties is GeneratedOld.Models.AKS)
            {
                return new AKSComputeTarget(this.ServiceContext, computeResourceDto);
            }

            if (computeResourceDto.Properties is GeneratedOld.Models.DataLakeAnalytics)
            {
                return new DataLakeAnalyticsComputeTarget(this.ServiceContext, computeResourceDto);
            }

            return new DefaultComputeTarget(this.ServiceContext, computeResourceDto);
        }
    }
}
