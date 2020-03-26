// <copyright file="Image.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Azure.MachineLearning.Services.GeneratedOld;
using Azure.MachineLearning.Services.GeneratedOld.Models;
using Azure.MachineLearning.Services.Models;

namespace Azure.MachineLearning.Services.Images
{
    public class Image : IRefreshableFromDto<ImageResponseBase>
    {
        public Image(
            ServiceContext serviceContext,
            ImageResponseBase imageDto,
            string operationId = null)
        {
            Throw.IfNull(serviceContext, nameof(serviceContext));
            Throw.IfNull(imageDto, nameof(imageDto));
            this.ServiceContext = serviceContext;
            this.RefreshFromDto(imageDto);

            this.OperationId = operationId;
        }

        public DateTime LastRefreshFromDto { get; private set; }

        public ServiceContext ServiceContext { get; set; }

        public string Id { get; set; }

        public string Name { get; set; }

        public int? Version { get; set; }

        public string Description { get; set; }

        public IList<string> Tags { get; set; }

        public DateTime? CreatedTime { get; set; }

        public string CreationState { get; set; }

        public ErrorResponse Error { get; set; }

        public IList<string> ModelIds { get; set; }

        public IList<Models.Model> ModelDetails { get; set; }

        public string DriverProgram { get; set; }

        public IList<ImageAsset> Assets { get; set; }

        public TargetRuntime TargetRuntime { get; set; }

        public string ImageBuildLogUri { get; set; }

        private string OperationId { get; set; } = null;

        public async Task WaitForCreation(
            PollingConfiguration pollConfig = default(PollingConfiguration),
            bool showOutput = false,
            Dictionary<string, List<string>> customHeaders = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (OperationId == null)
            {
                // Services do not track the OperationId, so it is not included in the Image object unless
                // we created it in this session. For example, if a reference to an Image is acquried through
                // "ListAsync", then the user cannot call "WaitForCreation" on it.
                throw new InvalidOperationException(
                    "WaitForCreation can only be called on an object that was created in this session.");
            }

            var restClient = new RestClient(this.ServiceContext.Credentials);
            restClient.BaseUri = ServiceContext.ModelManagementEndpoint;

            pollConfig = pollConfig ?? new PollingConfiguration();

            await pollConfig.WaitForCompletion(
                async () => await UpdateStateAsync(customHeaders, cancellationToken).ConfigureAwait(false),
                IsInTerminalState,
                showOutput,
                cancellationToken).ConfigureAwait(false);

            if (CreationState == "Succeeded")
            {
                return;
            }
            else
            {
                await HandleImageCreationErrorAsync(customHeaders, cancellationToken).ConfigureAwait(false);
            }
        }

        public async Task DeleteAsync(
            Dictionary<string, List<string>> customHeaders = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var restClient = new RestClient(this.ServiceContext.Credentials);
            restClient.BaseUri = ServiceContext.ModelManagementEndpoint;

            await RestCallWrapper.WrapAsync(
                () => restClient.Image.DeleteWithHttpMessagesAsync(
                    ServiceContext.SubscriptionId,
                    ServiceContext.ResourceGroupName,
                    ServiceContext.WorkspaceName,
                    Id,
                    customHeaders: customHeaders,
                    cancellationToken: cancellationToken)).ConfigureAwait(false);
        }

        public void RefreshFromDto(ImageResponseBase imageDto)
        {
            Throw.IfNull(imageDto, nameof(imageDto));

            this.Id = imageDto.Id;
            this.Name = imageDto.Name;
            this.Description = imageDto.Description;

            this.Tags = new List<string>(imageDto.Tags ?? Enumerable.Empty<string>());
            this.ModelIds = new List<string>(imageDto.ModelIds ?? Enumerable.Empty<string>());

            if (imageDto.ModelDetails != null)
            {
                this.ModelDetails = imageDto.ModelDetails.Select(
                    model => new DefaultModel(ServiceContext, model) as Models.Model).ToList();
            }
            else
            {
                this.ModelDetails = new List<Models.Model>();
            }

            this.CreatedTime = imageDto.CreatedTime;
            this.CreationState = imageDto.CreationState;
            if (imageDto.Error != null)
            {
                this.Error = new ErrorResponse(imageDto.Error);
            }

            this.ImageBuildLogUri = imageDto.ImageBuildLogUri;

            LastRefreshFromDto = DateTime.UtcNow;
        }

        private async Task UpdateStateAsync(
            Dictionary<string, List<string>> customHeaders,
            CancellationToken cancellationToken)
        {
            var restClient = new RestClient(this.ServiceContext.Credentials);
            restClient.BaseUri = ServiceContext.ModelManagementEndpoint;

            AsyncOperationStatus status = await RestCallWrapper.WrapAsync(
                () => restClient.Operations.GetWithHttpMessagesAsync(
                    this.ServiceContext.SubscriptionId,
                    this.ServiceContext.ResourceGroupName,
                    this.ServiceContext.WorkspaceName,
                    this.OperationId,
                    customHeaders: customHeaders,
                    cancellationToken: cancellationToken)).ConfigureAwait(false);

            CreationState = status.State;
        }

        private bool IsInTerminalState()
        {
            if (CreationState == null)
            {
                return false;
            }

            return TerminalStates.States.Contains(CreationState.ToLowerInvariant());
        }

        private async Task HandleImageCreationErrorAsync(
            Dictionary<string, List<string>> customHeaders,
            CancellationToken cancellationToken)
        {
            var restClient = new RestClient(ServiceContext.Credentials);
            restClient.BaseUri = ServiceContext.ModelManagementEndpoint;

            AsyncOperationStatus status = await RestCallWrapper.WrapAsync(
                () => restClient.Operations.GetWithHttpMessagesAsync(
                    this.ServiceContext.SubscriptionId,
                    this.ServiceContext.ResourceGroupName,
                    this.ServiceContext.WorkspaceName,
                    this.OperationId,
                    customHeaders: customHeaders,
                    cancellationToken: cancellationToken)).ConfigureAwait(false);

            throw new InvalidOperationException(string.Format(
                "Operation failed with state {0}. Error is {1}. Log file location: {2}",
                status.State,
                status.Error?.Message,
                this.ImageBuildLogUri));
        }
    }
}
