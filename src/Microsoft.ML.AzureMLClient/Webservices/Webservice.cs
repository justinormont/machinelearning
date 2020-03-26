// <copyright file="Webservice.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Azure.MachineLearning.Services.GeneratedOld;
using Azure.MachineLearning.Services.GeneratedOld.Models;

namespace Azure.MachineLearning.Services.Webservices
{
    public abstract class Webservice
    {
        public ServiceContext ServiceContext { get; protected set; }

        public string Name { get; protected set; } = null;

        public string ServiceId { get; protected set; } = null;

        public string OperationId { get; protected set; } = null;

        protected string Status { get; set; } = "NotStarted";

        /// <summary>
        /// Polls the WebService for deployment to complete. Throws an exception if the
        /// object reaches an invalid state.
        /// </summary>
        public async Task WaitForDeployment(
            PollingConfiguration pollConfig = default(PollingConfiguration),
            bool showOutput = false,
            Dictionary<string, List<string>> customHeaders = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (OperationId == null)
            {
                // Services do not track the OperationId, so it is not included in the webservice object unless
                // we created it in this session. For example, if a reference to a webservice is acquired through
                // "ListAsync", then the user cannot call "WaitForCreation" on it.
                throw new InvalidOperationException("WaitForDeployment can only be called on an object that was created in this session.");
            }

            RestClient restClient = GetModelManagementRestClient();

            pollConfig = pollConfig ?? new PollingConfiguration();

            await pollConfig.WaitForCompletion(
                async () => await UpdateState(customHeaders, cancellationToken).ConfigureAwait(false),
                IsInTerminalState,
                showOutput,
                cancellationToken).ConfigureAwait(false);

            if (string.Equals(Status, "Succeeded", StringComparison.InvariantCultureIgnoreCase))
            {
                return;
            }
            else
            {
                await HandleImageCreationErrorAsync(customHeaders, cancellationToken).ConfigureAwait(false);
            }
        }

        private bool IsInTerminalState()
        {
            return TerminalStates.States.Contains(this.Status.ToLowerInvariant());
        }

        private async Task UpdateState(
            Dictionary<string, List<string>> customHeaders = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            RestClient restClient = GetModelManagementRestClient();

            AsyncOperationStatus getResult = await RestCallWrapper.WrapAsync(
                () => restClient.Operations.GetWithHttpMessagesAsync(
                    this.ServiceContext.SubscriptionId,
                    this.ServiceContext.ResourceGroupName,
                    this.ServiceContext.WorkspaceName,
                    this.OperationId,
                    customHeaders: customHeaders,
                    cancellationToken: cancellationToken)).ConfigureAwait(false);

            Status = getResult.State;
        }

        private async Task HandleImageCreationErrorAsync(
            Dictionary<string, List<string>> customHeaders = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            RestClient restClient = GetModelManagementRestClient();

            AsyncOperationStatus status = await RestCallWrapper.WrapAsync(
                () => restClient.Operations.GetWithHttpMessagesAsync(
                    this.ServiceContext.SubscriptionId,
                    this.ServiceContext.ResourceGroupName,
                    this.ServiceContext.WorkspaceName,
                    this.OperationId,
                    customHeaders: customHeaders,
                    cancellationToken: cancellationToken)).ConfigureAwait(false);

            throw new InvalidOperationException(string.Format(
                "Operation failed with state {0}. Error is {1}. Image creation log can be found at: {2}",
                status.State,
                status.Error.Message,
                status.Error.Details.First().Message));
        }

        private RestClient GetModelManagementRestClient()
        {
            var restClient = new RestClient(ServiceContext.Credentials);
            restClient.BaseUri = ServiceContext.ModelManagementEndpoint;

            return restClient;
        }
    }
}
