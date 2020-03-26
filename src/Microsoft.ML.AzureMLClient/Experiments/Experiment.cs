// <copyright file = "Experiment.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

using Azure.MachineLearning.Services.GeneratedOld;
using Azure.MachineLearning.Services.GeneratedOld.Models;
using Azure.MachineLearning.Services.Metrics;
using Azure.MachineLearning.Services.Runs;
using Microsoft.Rest;

namespace Azure.MachineLearning.Services.Experiments
{
    public class Experiment : IRefreshableFromDto<ExperimentDto>
    {
        public Experiment(ServiceContext serviceContext, ExperimentDto experimentDto)
        {
            Throw.IfNull(serviceContext, nameof(serviceContext));
            Throw.IfNull(experimentDto, nameof(experimentDto));
            this.ServiceContext = serviceContext;
            this.RefreshFromDto(experimentDto);
            this.RestClient = new RestClient(this.ServiceContext.Credentials);
            this.RestClient.BaseUri = this.ServiceContext.ExperimentationEndpoint;
        }

        public ServiceContext ServiceContext { get; private set; }

        public Guid? Id { get; private set; }

        public string Name { get; set; }

        public string Description { get; private set; }

        public DateTime? CreatedTime { get; private set; }

        public RunOperations Runs
        {
            get
            {
                Throw.IfNullOrEmpty(this.Name, nameof(this.Name));
                return new RunOperations(this.ServiceContext, this.Name);
            }
        }

        public MetricOperations Metrics
        {
            get
            {
                Throw.IfNullOrEmpty(this.Name, nameof(this.Name));
                return new MetricOperations(this.ServiceContext, this.Name);
            }
        }

        public DateTime LastRefreshFromDto { get; private set; }

        protected GeneratedOld.RestClient RestClient { get; set; }

        public void RefreshFromDto(ExperimentDto experimentDto)
        {
            Throw.IfNull(experimentDto, nameof(experimentDto));

            this.Id = experimentDto.ExperimentId;
            this.Name = experimentDto.Name;
            this.CreatedTime = experimentDto.CreatedUtc;
            this.Description = experimentDto.Description;
            this.LastRefreshFromDto = DateTime.UtcNow;
        }

        public async Task<bool> ObserveAsync(
            Dictionary<string, List<string>> customHeaders = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            bool exists = false;
            try
            {
                ExperimentDto response = await RestCallWrapper.WrapAsync(
                    () => this.RestClient.Experiment.GetWithHttpMessagesAsync(
                        this.ServiceContext.SubscriptionId,
                        this.ServiceContext.ResourceGroupName,
                        this.ServiceContext.WorkspaceName,
                        this.Name,
                        customHeaders: customHeaders,
                        cancellationToken: cancellationToken)).ConfigureAwait(false);
                this.RefreshFromDto(response);
                exists = true;
            }
            catch (MachineLearningServiceException exception)
            {
                if (exception.Response.StatusCode != HttpStatusCode.NotFound)
                {
                    throw;
                }
            }

            return exists;
        }
    }
}