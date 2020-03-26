// <copyright file="AzureMLComputeTarget.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Azure.MachineLearning.Services.Compute
{
    public class AzureMLComputeTarget : ComputeTarget
    {
        public AzureMLComputeTarget(
            ServiceContext serviceContext,
            GeneratedOld.Models.ComputeResource computeResourceDto = null)
        {
            Throw.IfNull(serviceContext, nameof(serviceContext));

            this.ServiceContext = serviceContext;
            if (computeResourceDto != null)
            {
                this.RefreshFromDto(computeResourceDto);
            }
        }

        #region Unwrapped Properties
        public string VirtualMachineSize { get; private set; }

        public VMPriority? Priority { get; private set; }

        public ScaleSettings ScaleSettings { get; private set; }

        public SSHCredentials AdminCredentials { get; private set; }

        public string SubnetResourceId { get; private set; }

        public NodeAllocationState? AllocationState { get; private set; }

        public DateTime? AllocationStateTransitionTime { get; private set; }

        public IList<ComputeError> Errors { get; private set; }

        public int? CurrentNodeCount { get; private set; }

        public int? TargetNodeCount { get; private set; }

        public NodeStateCounts NodeStateCounts { get; private set; }
        #endregion

        public async Task DeleteAsync(
            Dictionary<string, List<string>> customHeaders = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            await this.DeleteOrDetachAsync(
                GeneratedOld.Models.UnderlyingResourceAction.Delete,
                customHeaders,
                cancellationToken).ConfigureAwait(false);
        }

        public override void RefreshFromDto(
            GeneratedOld.Models.ComputeResource computeResourceDto)
        {
            Throw.IfNull(computeResourceDto, nameof(computeResourceDto));

            var amlComputeData = computeResourceDto.Properties as GeneratedOld.Models.AmlCompute;
            if (amlComputeData == null)
            {
                throw new ArgumentException(
                    $"Argument {nameof(computeResourceDto)} did not cast to DTO for AML Compute");
            }
            this.VirtualMachineSize = amlComputeData.Properties.VmSize;

            if (amlComputeData.Properties.VmPriority.HasValue)
            {
                GeneratedOld.Models.VmPriority temp = amlComputeData.Properties.VmPriority.Value;
                if (temp == "Dedicated")
                {
                    this.Priority = VMPriority.Dedicated;
                }
                else if (temp == "LowPriority")
                {
                    this.Priority = VMPriority.LowPriority;
                }
                else
                {
                    throw new ArgumentException(
                        $"Could not convert VmPriority: {temp}");
                }
            }
            else
            {
                this.Priority = null;
            }

            if (amlComputeData.Properties.ScaleSettings != null)
            {
                this.ScaleSettings = new ScaleSettings(
                    amlComputeData.Properties.ScaleSettings);
            }
            else
            {
                this.ScaleSettings = null;
            }

            if (amlComputeData.Properties.UserAccountCredentials != null)
            {
                this.AdminCredentials = new SSHCredentials(
                    amlComputeData.Properties.UserAccountCredentials);
            }
            else
            {
                this.AdminCredentials = null;
            }

            if (amlComputeData.Properties.Subnet != null)
            {
                this.SubnetResourceId = amlComputeData.Properties.Subnet.Id;
            }
            else
            {
                this.SubnetResourceId = null;
            }

            if (amlComputeData.Properties.AllocationState.HasValue)
            {
                var temp = amlComputeData.Properties.AllocationState.Value;
                if (temp == "Steady")
                {
                    this.AllocationState = NodeAllocationState.Steady;
                }
                else if (temp == "Resizing")
                {
                    this.AllocationState = NodeAllocationState.Resizing;
                }
                else
                {
                    throw new ArgumentException($"Could not convert AllocationState: {temp}");
                }
            }
            else
            {
                this.AllocationState = null;
            }

            this.AllocationStateTransitionTime =
                amlComputeData.Properties.AllocationStateTransitionTime;

            if (amlComputeData.Properties.Errors != null)
            {
                // Note unwrap of embedded Error field
                this.Errors =
                    amlComputeData.Properties.Errors.Select(x => new ComputeError(x.Error)).ToList();
            }
            else
            {
                this.Errors = null;
            }

            this.CurrentNodeCount = amlComputeData.Properties.CurrentNodeCount;
            this.TargetNodeCount = amlComputeData.Properties.TargetNodeCount;
            if (amlComputeData.Properties.NodeStateCounts != null)
            {
                this.NodeStateCounts =
                    new NodeStateCounts(amlComputeData.Properties.NodeStateCounts);
            }
            else
            {
                this.NodeStateCounts = null;
            }

            this.RefreshCommonFields(computeResourceDto);

            this.LastRefreshFromDto = DateTime.UtcNow;
        }
    }
}
