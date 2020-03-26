// <copyright file="AKSNetworkingConfiguration.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

using Azure.MachineLearning.Services.GeneratedOld.Models;

namespace Azure.MachineLearning.Services.Compute
{
    public class AKSNetworkingConfiguration
    {
        public AKSNetworkingConfiguration(
            GeneratedOld.Models.AksNetworkingConfiguration aksNetworkingConfiguration = null)
        {
            if (aksNetworkingConfiguration != null)
            {
                this.SubnetId = aksNetworkingConfiguration.SubnetId;
                this.ServiceCIDR = aksNetworkingConfiguration.ServiceCidr;
                this.DNSServiceIP = aksNetworkingConfiguration.DnsServiceIP;
                this.DockerBridgeCIDR = aksNetworkingConfiguration.DockerBridgeCidr;
            }
        }

        public string SubnetId { get; private set; }

        public string ServiceCIDR { get; private set; }

        public string DNSServiceIP { get; private set; }

        public string DockerBridgeCIDR { get; private set; }

        public AksNetworkingConfiguration ToDTO()
        {
            var networkingConfigurationDto =
                new GeneratedOld.Models.AksNetworkingConfiguration();
            networkingConfigurationDto.SubnetId = this.SubnetId;
            networkingConfigurationDto.ServiceCidr = this.ServiceCIDR;
            networkingConfigurationDto.DnsServiceIP = this.DNSServiceIP;
            networkingConfigurationDto.DockerBridgeCidr = this.DockerBridgeCIDR;

            return networkingConfigurationDto;
        }
    }
}
