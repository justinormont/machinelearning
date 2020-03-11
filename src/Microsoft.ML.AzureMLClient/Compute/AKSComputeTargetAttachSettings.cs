// <copyright file="AKSComputeTargetAttachSettings.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

using System;

namespace Azure.MachineLearning.Services.Compute
{
    public class AKSComputeTargetAttachSettings : ComputeTargetAttachSettings
    {
        public Guid SubscriptionId { get; set; }

        public string ResourceGroupName { get; set; }

        public string ClusterName { get; set; }

        public override GeneratedOld.Models.ComputeResource BuildDTO(ServiceContext serviceContext)
        {
            Throw.IfNull(serviceContext, nameof(serviceContext));
            this.Validate();

            var aks = new GeneratedOld.Models.AKS();

            string fmt = "/subscriptions/{0}/resourcegroups/{1}/providers/Microsoft.ContainerService/managedClusters/{2}";

            aks.ResourceId = string.Format(fmt, this.SubscriptionId.ToString(), this.ResourceGroupName, this.ClusterName);

            var patch = new GeneratedOld.Models.ComputeResource();
            patch.Properties = aks;

            return patch;
        }

        public void Validate()
        {
            Throw.IfNullOrEmpty(this.ResourceGroupName, nameof(this.ResourceGroupName));
            Throw.IfNullOrEmpty(this.ClusterName, nameof(this.ClusterName));
        }
    }
}
