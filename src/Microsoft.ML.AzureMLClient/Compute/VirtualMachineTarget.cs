// <copyright file="VirtualMachineTarget.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Azure.MachineLearning.Services.Compute
{
    public class VirtualMachineTarget : ComputeTarget
    {
        public VirtualMachineTarget(
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

        #region Unwrapped Properies
        // 'Unwrap' the following from the Properties field of the DTO
        public string VirtualMachineSize { get; private set; }

        public string Address { get; private set; }

        public int? SSHport { get; private set; }

        public SSHCredentials AdministratorAccount { get; private set; }
        #endregion

        public async Task DetachAsync(
            Dictionary<string, List<string>> customHeaders = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            await this.DeleteOrDetachAsync(
                GeneratedOld.Models.UnderlyingResourceAction.Detach,
                customHeaders,
                cancellationToken).ConfigureAwait(false);
        }

        public override void RefreshFromDto(
            GeneratedOld.Models.ComputeResource computeResourceDto)
        {
            Throw.IfNull(computeResourceDto, nameof(computeResourceDto));

            var vmData = computeResourceDto.Properties as GeneratedOld.Models.VirtualMachine;
            if (vmData == null)
            {
                throw new ArgumentException(
                    $"Argument {nameof(computeResourceDto)} did not cast to DTO for VirtualMachine");
            }

            this.VirtualMachineSize = vmData.Properties.VirtualMachineSize;
            this.Address = vmData.Properties.Address;
            this.SSHport = vmData.Properties.SshPort;
            if (vmData.Properties.AdministratorAccount != null)
            {
                this.AdministratorAccount = new SSHCredentials(vmData.Properties.AdministratorAccount);
            }
            else
            {
                this.AdministratorAccount = null;
            }

            this.RefreshCommonFields(computeResourceDto);

            this.LastRefreshFromDto = DateTime.UtcNow;
        }
    }
}
