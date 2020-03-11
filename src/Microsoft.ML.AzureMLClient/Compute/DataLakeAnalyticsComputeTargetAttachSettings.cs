// <copyright file="DataLakeAnalyticsComputeTargetAttachSettings.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace Azure.MachineLearning.Services.Compute
{
    public class DataLakeAnalyticsComputeTargetAttachSettings :
        ComputeTargetAttachSettings
    {
        public string ResourceGroupName { get; set; }

        public string DataLakeAnalyticsAccount { get; set; }

        public override GeneratedOld.Models.ComputeResource BuildDTO(ServiceContext serviceContext)
        {
            Throw.IfNull(serviceContext, nameof(serviceContext));
            this.Validate();

            var adla = new GeneratedOld.Models.DataLakeAnalytics();
            adla.Properties = new GeneratedOld.Models.DataLakeAnalyticsProperties();
            string fmt = "/subscriptions/{0}/resourceGroups/{1}/providers/Microsoft.DataLakeAnalytics/accounts/{2}";
            adla.ResourceId = string.Format(
                fmt,
                serviceContext.SubscriptionId,
                this.ResourceGroupName,
                this.DataLakeAnalyticsAccount);

            var computeResourceDto = new GeneratedOld.Models.ComputeResource();
            computeResourceDto.Properties = adla;

            return computeResourceDto;
        }

        public void Validate()
        {
            Throw.IfNullOrEmpty(this.ResourceGroupName, nameof(this.ResourceGroupName));
            Throw.IfNullOrEmpty(this.DataLakeAnalyticsAccount, nameof(this.DataLakeAnalyticsAccount));
        }
    }
}
