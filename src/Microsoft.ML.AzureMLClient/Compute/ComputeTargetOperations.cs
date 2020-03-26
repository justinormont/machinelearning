// <copyright file = "ComputeTargetOperations.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Azure.MachineLearning.Services.GeneratedOld;

namespace Azure.MachineLearning.Services.Compute
{
    public class ComputeTargetOperations
    {
        public ComputeTargetOperations(ServiceContext serviceContext)
        {
            Throw.IfNull(serviceContext, nameof(serviceContext));
            this.ServiceContext = serviceContext;
        }

        public ServiceContext ServiceContext { get; private set; }

        public IPageFetcher<ComputeTarget> GetPagedList()
        {
            var fetcher = new ComputeTargetPageFetcher(this.ServiceContext);
            return fetcher;
        }

        public IEnumerable<ComputeTarget> List()
        {
            var lister = new LazyEnumerator<ComputeTarget>();
            lister.Fetcher = this.GetPagedList();

            return lister;
        }

        public async Task<ComputeTarget> GetAsync(
            string name,
            Dictionary<string, List<string>> customHeaders = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            Throw.IfNullOrEmpty(name, nameof(name));

            var restClient = new RestClient(this.ServiceContext.Credentials);
            restClient.BaseUri = this.ServiceContext.AzureResourceManagerEndpoint;
            var mlcClient = new MachineLearningCompute(restClient);

            GeneratedOld.Models.ComputeResource computeResourceResponse =
                await RestCallWrapper.WrapAsync(
                    () => mlcClient.GetWithHttpMessagesAsync(
                        this.ServiceContext.SubscriptionId.ToString(),
                        this.ServiceContext.ResourceGroupName,
                        this.ServiceContext.WorkspaceName,
                        name,
                        customHeaders: customHeaders,
                        cancellationToken: cancellationToken)).ConfigureAwait(false);

            var f = new ComputeTargetFactory(this.ServiceContext);

            return f.ConvertFromDto(computeResourceResponse);
        }

        public async Task<ComputeTarget> AttachAsync(
            string name,
            ComputeTargetAttachSettings computeAttachSettings,
            Dictionary<string, List<string>> customHeaders = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            Throw.IfNullOrEmpty(name, nameof(name));
            Throw.IfNull(computeAttachSettings, nameof(computeAttachSettings));

            var computeAttachDto = computeAttachSettings.BuildDTO(this.ServiceContext);
            if (string.IsNullOrEmpty(computeAttachDto.Location))
            {
                computeAttachDto.Location = this.ServiceContext.Location;
            }

            var restClient = new GeneratedOld.RestClient(this.ServiceContext.Credentials);
            restClient.BaseUri = this.ServiceContext.AzureResourceManagerEndpoint;
            var mlcClient = new GeneratedOld.MachineLearningCompute(restClient);

            GeneratedOld.Models.ComputeResource computeResourceResponse =
                await RestCallWrapper.WrapAsync(
                    () => mlcClient.CreateOrUpdateWithHttpMessagesAsync(
                        this.ServiceContext.SubscriptionId.ToString(),
                        this.ServiceContext.ResourceGroupName,
                        this.ServiceContext.WorkspaceName,
                        name,
                        computeAttachDto,
                        customHeaders: customHeaders,
                        cancellationToken: cancellationToken)).ConfigureAwait(false);

            var f = new ComputeTargetFactory(this.ServiceContext);

            return f.ConvertFromDto(computeResourceResponse);
        }

        public async Task<ComputeTarget> ProvisionAsync(
            string name,
            ComputeTargetProvisionSettings provisionSettings,
            Dictionary<string, List<string>> customHeaders = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            Throw.IfNullOrEmpty(name, nameof(name));
            Throw.IfNull(provisionSettings, nameof(provisionSettings));

            if (ComputeConstants.ReservedNames.Contains(name))
            {
                throw new ArgumentException(
                    $"ComputeTarget name '{name}' is reserved. Please choose another");
            }

            var provisionSettingsDto =
                provisionSettings.BuildDTO(this.ServiceContext.SubscriptionId);
            // The Location in the DTO has to match the workspace location
            provisionSettingsDto.Location = this.ServiceContext.Location;

            var restClient = new RestClient(this.ServiceContext.Credentials);
            restClient.BaseUri = this.ServiceContext.AzureResourceManagerEndpoint;
            var mlcClient = new MachineLearningCompute(restClient);

            GeneratedOld.Models.ComputeResource computeResourceDto = await RestCallWrapper.WrapAsync(
                () => mlcClient.CreateOrUpdateWithHttpMessagesAsync(
                    this.ServiceContext.SubscriptionId.ToString(),
                    this.ServiceContext.ResourceGroupName,
                    this.ServiceContext.WorkspaceName,
                    name,
                    provisionSettingsDto,
                    customHeaders: customHeaders,
                    cancellationToken: cancellationToken)).ConfigureAwait(false);

            var f = new ComputeTargetFactory(this.ServiceContext);

            return f.ConvertFromDto(computeResourceDto);
        }
    }
}
