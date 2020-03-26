// <copyright file="AKSComputeTarget.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Azure.MachineLearning.Services.Compute
{
    public class AKSComputeTarget : ComputeTarget
    {
        public AKSComputeTarget(
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
        public string ClusterFQDN { get; private set; }

        public IList<SystemServiceInformation> SystemServices { get; private set; }

        public int? AgentCount { get; private set; }

        public string AgentVMSize { get; private set; }

        public SSLConfiguration SSLConfiguration { get; private set; }

        public AKSNetworkingConfiguration NetworkingConfiguration { get; private set; }
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

            var aksData = computeResourceDto.Properties as GeneratedOld.Models.AKS;
            if (aksData == null)
            {
                var fmt = "Argument {0} did not cast to DTO for AKS";

                throw new ArgumentException(string.Format(fmt, nameof(computeResourceDto)));
            }

            if (aksData.Properties != null)
            {
                // When the 'attach' call is made, aksData.Properties might not be available
                this.ClusterFQDN = aksData.Properties.ClusterFqdn;
                if (aksData.Properties.SystemServices != null)
                {
                    this.SystemServices =
                        aksData.Properties.SystemServices.Select(
                            x => new SystemServiceInformation(x)).ToList();
                }
                else
                {
                    this.SystemServices = null;
                }
                this.AgentCount = aksData.Properties.AgentCount;
                this.AgentVMSize = aksData.Properties.AgentVMSize;
                if (aksData.Properties.SslConfiguration != null)
                {
                    this.SSLConfiguration =
                        new SSLConfiguration(aksData.Properties.SslConfiguration);
                }
                else
                {
                    this.SSLConfiguration = null;
                }
                if (aksData.Properties.AksNetworkingConfiguration != null)
                {
                    this.NetworkingConfiguration =
                        new AKSNetworkingConfiguration(aksData.Properties.AksNetworkingConfiguration);
                }
                else
                {
                    this.NetworkingConfiguration = null;
                }
            }

            this.RefreshCommonFields(computeResourceDto);

            this.LastRefreshFromDto = DateTime.UtcNow;
        }
    }
}
