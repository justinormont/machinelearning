// <copyright file="AKSComputeTargetProvisioningSettings.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

using System;

namespace Azure.MachineLearning.Services.Compute
{
    public class AKSComputeTargetProvisioningSettings : ComputeTargetProvisionSettings
    {
        public AKSComputeTargetProvisioningSettings()
        {
            // Nothing to do
        }

        public int? AgentCount { get; set; }

        public string VMSize { get; set; }

        public SSLConfiguration SSLConfiguration { get; set; }

        public AKSNetworkingConfiguration NetworkingConfiguration { get; set; }

        public string Location { get; set; }

        public override GeneratedOld.Models.ComputeResource BuildDTO(Guid subscriptionid)
        {
            var aksProperties = new GeneratedOld.Models.AKSProperties();
            aksProperties.AgentCount = this.AgentCount;
            aksProperties.AgentVMSize = this.VMSize;

            if (this.SSLConfiguration != null)
            {
                aksProperties.SslConfiguration = this.SSLConfiguration.ToDTO();
            }

            if (this.NetworkingConfiguration != null)
            {
                aksProperties.AksNetworkingConfiguration = this.NetworkingConfiguration.ToDTO();
            }

            var aks = new GeneratedOld.Models.AKS();
            aks.Properties = aksProperties;
            aks.ComputeLocation = this.Location;

            var patch = new GeneratedOld.Models.ComputeResource();
            patch.Properties = aks;
            // Don't set the patch.Location, since it needs to be the same as the
            // workspace
            // ProvisionAsync will take care of setting it

            return patch;
        }
    }
}
