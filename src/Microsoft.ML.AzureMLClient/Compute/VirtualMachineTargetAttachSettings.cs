// <copyright file="VirtualMachineTargetAttachSettings.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

using System;
using System.IO;

namespace Azure.MachineLearning.Services.Compute
{
    public class VirtualMachineTargetAttachSettings :
        ComputeTargetAttachSettings
    {
        public VirtualMachineTargetAttachSettings()
        {
            this.SSHPort = 22;
        }

        public string Username { get; set; }

        public string Password { get; set; }

        public string Address { get; set; }

        public int SSHPort { get; set; }

        public string PrivateKeyFile { get; set; }

        public string PrivateKeyPassphrase { get; set; }

        public override GeneratedOld.Models.ComputeResource BuildDTO(ServiceContext serviceContext)
        {
            Throw.IfNull(serviceContext, nameof(serviceContext));
            this.Validate();

            var virtualMachine = new GeneratedOld.Models.VirtualMachine();
            virtualMachine.Properties = new GeneratedOld.Models.VirtualMachineProperties();
            virtualMachine.Properties.Address = this.Address;
            virtualMachine.Properties.SshPort = this.SSHPort;

            virtualMachine.Properties.AdministratorAccount =
                new GeneratedOld.Models.VirtualMachineSshCredentials();
            virtualMachine.Properties.AdministratorAccount.Username = this.Username;
            // Following if statements assume that the Validate method has ensured
            // we only have a password or a private key
            if (!string.IsNullOrEmpty(this.Password))
            {
                virtualMachine.Properties.AdministratorAccount.Password = this.Password;
            }
            if (!string.IsNullOrEmpty(this.PrivateKeyFile))
            {
                using (var key = new StreamReader(this.PrivateKeyFile))
                {
                    virtualMachine.Properties.AdministratorAccount.PrivateKeyData = key.ReadToEnd();
                    virtualMachine.Properties.AdministratorAccount.Passphrase = this.PrivateKeyPassphrase;
                }
            }

            var computeResourceDto = new GeneratedOld.Models.ComputeResource();
            computeResourceDto.Properties = virtualMachine;

            return computeResourceDto;
        }

        public void Validate()
        {
            Throw.IfNullOrEmpty(this.Username, nameof(this.Username));
            Throw.IfNullOrEmpty(this.Address, nameof(this.Address));

            if (string.IsNullOrEmpty(this.Password) && string.IsNullOrEmpty(this.PrivateKeyFile))
            {
                throw new ArgumentException(
                    "Must specify password or private key file for VirtualMachineTargetAttachSettings");
            }

            if (!(string.IsNullOrEmpty(this.Password) || string.IsNullOrEmpty(this.PrivateKeyFile)))
            {
                throw new ArgumentException(
                    "Must not specify both password and private key file for VirtualMachineTargetAttachSettings");
            }

            if (this.SSHPort <= 0 || this.SSHPort > 65535)
            {
                var msg = $"SSHPort {this.SSHPort} is not in range";
                throw new ArgumentOutOfRangeException(nameof(this.SSHPort), msg);
            }
        }
    }
}
