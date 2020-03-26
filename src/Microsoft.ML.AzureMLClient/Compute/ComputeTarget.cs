// <copyright file="ComputeTarget.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Azure.MachineLearning.Services.GeneratedOld;

namespace Azure.MachineLearning.Services.Compute
{
    public abstract class ComputeTarget :
        IRefreshableFromDto<GeneratedOld.Models.ComputeResource>
    {
        public ServiceContext ServiceContext { get; protected set; }

        public string Location { get; private set; }

        public string ProvisioningState { get; private set; }

        public DateTime? Created { get; private set; }

        public DateTime? Modified { get; private set; }

        public string Description { get; private set; }

        public string Id { get; private set; }

        public string Name { get; private set; }

        public bool? IsAttachedCompute { get; private set; }

        public IList<ComputeError> ProvisioningErrors { get; private set; }

        public DateTime LastRefreshFromDto { get; protected set; }

        public async Task RefreshAsync(
            Dictionary<string, List<string>> customHeaders = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var restClient = new RestClient(this.ServiceContext.Credentials);
            restClient.BaseUri = this.ServiceContext.AzureResourceManagerEndpoint;
            var mlcClient = new MachineLearningCompute(restClient);

            GeneratedOld.Models.ComputeResource response = await RestCallWrapper.WrapAsync(
                () => mlcClient.GetWithHttpMessagesAsync(
                    this.ServiceContext.SubscriptionId.ToString(),
                    this.ServiceContext.ResourceGroupName,
                    this.ServiceContext.WorkspaceName,
                    this.Name,
                    customHeaders: customHeaders,
                    cancellationToken: cancellationToken)).ConfigureAwait(false);

            this.RefreshFromDto(response);
        }

        public async Task WaitForProvisioningAsync(
            PollingConfiguration pollingConfiguration = default(PollingConfiguration),
            bool showOutput = false,
            Dictionary<string, List<string>> customHeaders = null,
            CancellationToken cancellation = default(CancellationToken))
        {
            pollingConfiguration = pollingConfiguration ?? new PollingConfiguration();

            await pollingConfiguration.WaitForCompletion(
                async () => await this.RefreshAsync(customHeaders, cancellation).ConfigureAwait(false),
                () => TerminalStates.States.Contains(this.ProvisioningState.ToLowerInvariant()),
                showOutput,
                cancellation).ConfigureAwait(false);
        }

        public abstract void RefreshFromDto(GeneratedOld.Models.ComputeResource computeResourceDto);

        protected void RefreshCommonFields(GeneratedOld.Models.ComputeResource computeResourceDto)
        {
            Throw.IfNull(computeResourceDto, nameof(computeResourceDto));

            this.Id = computeResourceDto.Id;
            this.Name = computeResourceDto.Name;
            this.Location = computeResourceDto.Location;

            this.ProvisioningState = computeResourceDto.Properties.ProvisioningState;
            this.Created = computeResourceDto.Properties.CreatedOn;
            this.Modified = computeResourceDto.Properties.ModifiedOn;
            this.Description = computeResourceDto.Properties.Description;
            this.IsAttachedCompute = computeResourceDto.Properties.IsAttachedCompute;
            if (computeResourceDto.Properties.ProvisioningErrors != null)
            {
                // Note unwrap of embedded Error field
                this.ProvisioningErrors =
                    computeResourceDto.Properties.ProvisioningErrors.Select(
                        x => new ComputeError(x.Error)).ToList();
            }
            else
            {
                this.ProvisioningErrors = null;
            }
        }

        protected async Task DeleteOrDetachAsync(
            GeneratedOld.Models.UnderlyingResourceAction underlyingResourceAction,
            Dictionary<string, List<string>> customHeaders,
            CancellationToken cancellationToken)
        {
            var restClient = new GeneratedOld.RestClient(this.ServiceContext.Credentials);
            restClient.BaseUri = this.ServiceContext.AzureResourceManagerEndpoint;
            var mlcClient = new GeneratedOld.MachineLearningCompute(restClient);

            GeneratedOld.Models.MachineLearningComputeDeleteHeaders response = await RestCallWrapper.WrapAsync(
                () => mlcClient.DeleteWithHttpMessagesAsync(
                this.ServiceContext.SubscriptionId.ToString(),
                this.ServiceContext.ResourceGroupName,
                this.ServiceContext.WorkspaceName,
                this.Name,
                underlyingResourceAction,
                customHeaders: customHeaders,
                cancellationToken: cancellationToken)).ConfigureAwait(false);
        }
    }
}
