// <copyright file="DataLakeAnalyticsComputeTarget.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Azure.MachineLearning.Services.Compute
{
    public class DataLakeAnalyticsComputeTarget : ComputeTarget
    {
        public DataLakeAnalyticsComputeTarget(
            ServiceContext serviceContext = null,
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
        public string DataLakeStoreAccount { get; private set; }
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

        public override void RefreshFromDto(GeneratedOld.Models.ComputeResource computeResourceDto)
        {
            Throw.IfNull(computeResourceDto, nameof(computeResourceDto));

            var adlaData = computeResourceDto.Properties as GeneratedOld.Models.DataLakeAnalytics;
            if (adlaData == null)
            {
                throw new ArgumentException(
                    $"Argument {nameof(computeResourceDto)} did not cast to DTO for DataLakeAnalytics");
            }

            this.DataLakeStoreAccount = adlaData.Properties.DataLakeStoreAccountName;

            this.RefreshCommonFields(computeResourceDto);

            this.LastRefreshFromDto = DateTime.UtcNow;
        }
    }
}
