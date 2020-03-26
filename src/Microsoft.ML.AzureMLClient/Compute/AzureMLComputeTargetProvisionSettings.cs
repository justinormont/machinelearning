// <copyright file="AzureMLComputeTargetProvisionSettings.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;

using Azure.MachineLearning.Services.GeneratedOld.Models;

namespace Azure.MachineLearning.Services.Compute
{
    public class AzureMLComputeTargetProvisionSettings :
        ComputeTargetProvisionSettings
    {
        public AzureMLComputeTargetProvisionSettings()
        {
            this.VMPriority = VMPriority.Dedicated;
            this.MinNodes = 0;
        }

        public string VMSize { get; set; }

        public VMPriority VMPriority { get; set; }

        public int MinNodes { get; set; }

        public int MaxNodes { get; set; }

        public TimeSpan? NodeIdleBeforeScaledown { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

        public string SSHPublicKey { get; set; }

        public string VNetResourceGroupName { get; set; }

        public string VNetName { get; set; }

        public string SubNetName { get; set; }

        public string Location { get; set; }

        public Dictionary<string, string> Tags { get; set; }

        public string Description { get; set; }

        public override ComputeResource BuildDTO(Guid subscriptionId)
        {
            this.Validate();

            var scaleSettings = new GeneratedOld.Models.ScaleSettings();
            scaleSettings.MinNodeCount = this.MinNodes;
            scaleSettings.MaxNodeCount = this.MaxNodes;
            scaleSettings.NodeIdleTimeBeforeScaleDown = this.NodeIdleBeforeScaledown;

            GeneratedOld.Models.UserAccountCredentials creds = null;
            if (!string.IsNullOrEmpty(this.Username))
            {
                // Previous call to validate should ensure things are set up correctly
                creds = new GeneratedOld.Models.UserAccountCredentials();
                creds.AdminUserName = this.Username;
                creds.AdminUserPassword = this.Password;
                creds.AdminUserSshPublicKey = this.SSHPublicKey;
            }

            var computeProperties = new GeneratedOld.Models.AmlComputeProperties();
            computeProperties.VmPriority = this.VMPriority.ToString();
            computeProperties.VmSize = this.VMSize;
            if (!string.IsNullOrEmpty(this.SubNetName))
            {
                // Previous call to Validate ensures that all VNet things are set if one of them is
                var res = new GeneratedOld.Models.ResourceId();
                res.Id = this.BuildSubnetId(subscriptionId);
                computeProperties.Subnet = res;
            }
            computeProperties.UserAccountCredentials = creds;
            computeProperties.ScaleSettings = scaleSettings;

            var compute = new GeneratedOld.Models.AmlCompute();
            compute.Description = this.Description;
            compute.Properties = computeProperties;

            var patch = new GeneratedOld.Models.ComputeResource();
            patch.Tags = this.Tags;
            patch.Location = this.Location;
            patch.Properties = compute;

            return patch;
        }

        public string BuildSubnetId(Guid subscriptionId)
        {
            string fmt =
                "/subscriptions/{0}/resourceGroups/{1}/providers/Microsoft.Network/virtualNetworks/{2}/subnets/{3}";

            return string.Format(
                fmt,
                subscriptionId,
                this.VNetResourceGroupName,
                this.VNetName,
                this.SubNetName);
        }

        public void Validate()
        {
            if (!string.IsNullOrEmpty(this.Password) || !string.IsNullOrEmpty(this.SSHPublicKey))
            {
                if (string.IsNullOrEmpty(this.Username))
                {
                    throw new ArgumentException("Must specify user if password or SSH key supplied");
                }
            }

            if (!string.IsNullOrEmpty(this.Username))
            {
                if (string.IsNullOrEmpty(this.Password) && string.IsNullOrEmpty(this.SSHPublicKey))
                {
                    throw new ArgumentException("Must specify credentials if user specified");
                }
            }

            var vNetTest = new List<bool>();
            vNetTest.Add(!string.IsNullOrEmpty(this.VNetName));
            vNetTest.Add(!string.IsNullOrEmpty(this.VNetResourceGroupName));
            vNetTest.Add(!string.IsNullOrEmpty(this.SubNetName));
            if (vNetTest.Any(x => x) && !vNetTest.All(x => x))
            {
                throw new ArgumentException(
                    "The properties VNetName, ResourceGroup and Subnet must all be specified, or all left empty");
            }
        }
    }
}
