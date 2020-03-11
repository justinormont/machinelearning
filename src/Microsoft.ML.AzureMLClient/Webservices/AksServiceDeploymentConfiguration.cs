// <copyright file="AksServiceDeploymentConfiguration.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;

using Azure.MachineLearning.Services.Compute;
using Azure.MachineLearning.Services.GeneratedOld.Models;

namespace Azure.MachineLearning.Services.Webservices
{
    /// <summary>
    /// User-facing configuration file that is used to build an AksServiceCreateRequest.
    /// Hand-copied from the Python specification until we can figure out a better swagger story.
    /// https://docs.microsoft.com/en-us/python/api/azureml-core/azureml.core.webservice.aks.aksservicedeploymentconfiguration?view=azure-ml-py
    /// </summary>
    public class AksServiceDeploymentConfiguration : WebserviceDeploymentConfigurationBase
    {
        public AksServiceDeploymentConfiguration(
            bool? autoscaleEnabled = null,
            int? autoscaleMinReplicas = null,
            int? autoscaleMaxReplicas = null,
            int? autoscaleRefreshSeconds = null,
            int? autoscaleTargetUtilization = null,
            bool? collectModelData = null,
            bool? authEnabled = null,
            int? cpuCores = null,
            int? memoryGb = null,
            bool? enableAppInsights = null,
            int? scoringTimeoutMs = null,
            int? replicaMaxConcurrentRequests = null,
            int? maxRequestWaitTime = null,
            int? numReplicas = null,
            string primaryKey = null,
            string secondaryKey = null,
            IDictionary<string, string> tags = null,
            IDictionary<string, string> properties = null,
            string description = null,
            int? gpuCore = null,
            int? periodSeconds = null,
            int? initialDelaySeconds = null,
            int? timeoutSeconds = null,
            int? successThreshold = null,
            int? failureThreshold = null)
        {
            AutoscaleEnabled = autoscaleEnabled;
            AutoscaleMinReplicas = autoscaleMinReplicas;
            AutoscaleMaxReplicas = autoscaleMaxReplicas;
            AutoscaleRefreshSeconds = autoscaleRefreshSeconds;
            AutoscaleTargetUtilization = autoscaleTargetUtilization;
            CollectModelData = collectModelData;
            AuthEnabled = authEnabled;
            CpuCores = cpuCores;
            MemoryGb = memoryGb;
            EnableAppInsights = enableAppInsights;
            ScoringTimeoutMs = scoringTimeoutMs;
            ReplicaMaxConcurrentRequests = replicaMaxConcurrentRequests;
            MaxRequestWaitTime = maxRequestWaitTime;
            NumReplicas = numReplicas;
            PrimaryKey = primaryKey;
            SecondaryKey = secondaryKey;
            Tags = tags;
            Properties = properties;
            Description = description;
            GpuCore = gpuCore;
            PeriodSeconds = periodSeconds;
            InitialDelaySeconds = initialDelaySeconds;
            TimeoutSeconds = timeoutSeconds;
            SuccessThreshold = successThreshold;
            FailureThreshold = failureThreshold;

            ValidateConfiguration();
        }

        public bool? AutoscaleEnabled { get; private set; } = null;

        public int? AutoscaleMinReplicas { get; private set; } = null;

        public int? AutoscaleMaxReplicas { get; private set; } = null;

        public int? AutoscaleRefreshSeconds { get; private set; } = null;

        public int? AutoscaleTargetUtilization { get; private set; } = null;

        public bool? CollectModelData { get; private set; } = null;

        public bool? AuthEnabled { get; private set; } = null;

        public int? CpuCores { get; private set; } = null;

        public int? MemoryGb { get; private set; } = null;

        public bool? EnableAppInsights { get; private set; } = null;

        public int? ScoringTimeoutMs { get; private set; } = null;

        public int? ReplicaMaxConcurrentRequests { get; private set; } = null;

        public int? MaxRequestWaitTime { get; private set; } = null;

        public int? NumReplicas { get; private set; } = null;

        public string PrimaryKey { get; private set; } = null;

        public string SecondaryKey { get; private set; } = null;

        public IDictionary<string, string> Tags { get; private set; } = null;

        public IDictionary<string, string> Properties { get; private set; } = null;

        public string Description { get; private set; } = null;

        public int? GpuCore { get; private set; } = null;

        public int? PeriodSeconds { get; private set; } = null;

        public int? InitialDelaySeconds { get; private set; } = null;

        public int? TimeoutSeconds { get; private set; } = null;

        public int? SuccessThreshold { get; private set; } = null;

        public int? FailureThreshold { get; private set; } = null;

        /// <summary>
        /// Creates an AKSServiceCreateRequest based on the information in the AksServiceDeploymentConfiguration file.
        /// </summary>
        internal override ServiceCreateRequest ToServiceCreateRequest(
            string name,
            ComputeTarget computeTarget,
            Images.Image image)
        {
            Throw.IfNullOrEmpty(name, nameof(name));
            Throw.IfNull(computeTarget, nameof(computeTarget));
            Throw.IfNull(image, nameof(image));

            var createRequest = new AKSServiceCreateRequest
            {
                Name = name,
                ComputeType = "AKS",
                ComputeName = computeTarget?.Name,
                ImageId = image.Id,
                Properties = this.Properties,
                Description = this.Description,
                KvTags = this.Tags,
                NumReplicas = this.NumReplicas,
                AppInsightsEnabled = this.EnableAppInsights,
                LivenessProbeRequirements = new LivenessProbeRequirements
                {
                    PeriodSeconds = this.PeriodSeconds,
                    InitialDelaySeconds = this.InitialDelaySeconds,
                    TimeoutSeconds = this.TimeoutSeconds,
                    FailureThreshold = this.FailureThreshold,
                    SuccessThreshold = this.SuccessThreshold,
                },
                MaxConcurrentRequestsPerContainer = this.ReplicaMaxConcurrentRequests,
                MaxQueueWaitMs = this.MaxRequestWaitTime,
                Keys = new AuthKeys
                {
                    PrimaryKey = this.PrimaryKey,
                    SecondaryKey = this.SecondaryKey
                }
            };

            // Configure Data Collection
            if (this.CollectModelData == true)
            {
                createRequest.DataCollection.StorageEnabled = true;
            }

            // Configure AutoScaler
            if (this.AutoscaleEnabled == true)
            {
                createRequest.AutoScaler = new AutoScaler
                {
                    AutoscaleEnabled = true,
                    MinReplicas = this.AutoscaleMinReplicas,
                    MaxReplicas = this.AutoscaleMaxReplicas,
                    TargetUtilization = this.AutoscaleTargetUtilization,
                    RefreshPeriodInSeconds = this.AutoscaleRefreshSeconds
                };
            }

            // Configure Resource Requirements
            if (this.CpuCores != null && this.MemoryGb != null)
            {
                createRequest.ContainerResourceRequirements = new ContainerResourceRequirements
                {
                    Cpu = this.CpuCores,
                    MemoryInGB = this.MemoryGb,
                    Gpu = this.GpuCore,
                };
            }

            return createRequest;
        }

        private void ValidateConfiguration()
        {
            Throw.IfValueNotPositive(CpuCores, nameof(CpuCores));
            Throw.IfValueNotPositive(MemoryGb, nameof(MemoryGb));
            Throw.IfValueNotPositive(GpuCore, nameof(GpuCore));
            Throw.IfValueNotPositive(PeriodSeconds, nameof(PeriodSeconds));
            Throw.IfValueNotPositive(InitialDelaySeconds, nameof(InitialDelaySeconds));
            Throw.IfValueNotPositive(SuccessThreshold, nameof(SuccessThreshold));
            Throw.IfValueNotPositive(FailureThreshold, nameof(FailureThreshold));
            Throw.IfValueNotPositive(ScoringTimeoutMs, nameof(ScoringTimeoutMs));
            Throw.IfValueNotPositive(ReplicaMaxConcurrentRequests, nameof(ReplicaMaxConcurrentRequests));
            Throw.IfValueNotPositive(MaxRequestWaitTime, nameof(MaxRequestWaitTime));
            Throw.IfValueNotPositive(NumReplicas, nameof(NumReplicas));

            ValidateAutoscaleSettings();
        }

        private void ValidateAutoscaleSettings()
        {
            if (AutoscaleEnabled != null && AutoscaleEnabled == true)
            {
                if (NumReplicas != null)
                {
                    throw new ArgumentException(
                        "Invalid configuration. AutoScale enabled and NumReplicas provided.");
                }

                Throw.IfValueNotPositive(AutoscaleMinReplicas, nameof(AutoscaleMinReplicas));
                Throw.IfValueNotPositive(AutoscaleMaxReplicas, nameof(AutoscaleMaxReplicas));
                Throw.IfValueNotPositive(AutoscaleRefreshSeconds, nameof(AutoscaleRefreshSeconds));
                Throw.IfValueNotPositive(AutoscaleTargetUtilization, nameof(AutoscaleTargetUtilization));

                if (AutoscaleMinReplicas != null &&
                    AutoscaleMaxReplicas != null &&
                    AutoscaleMinReplicas > AutoscaleMaxReplicas)
                {
                    throw new ArgumentException(
                        "Invalid configuration. AutoscaleMinReplicas cannot be greater than AutoscaleMaxReplicas.");
                }
            }
            else
            {
                if (AutoscaleEnabled == false && NumReplicas == null)
                {
                    throw new ArgumentException(
                        "Invalid configuration. Autoscale disabled but NumReplicas not provided.");
                }

                if (AutoscaleMinReplicas != null)
                {
                    throw new ArgumentException(
                        "Invalid configuration. AutoscaleMinReplicas provided without enabling autoscaling.");
                }

                if (AutoscaleMaxReplicas != null)
                {
                    throw new ArgumentException(
                        "Invalid configuration. AutoscaleMaxReplicas provided without enabling autoscaling.");
                }

                if (AutoscaleRefreshSeconds != null)
                {
                    throw new ArgumentException(
                        "Invalid configuration. AutoscaleRefreshSeconds provided without enabling autoscaling.");
                }

                if (AutoscaleTargetUtilization != null)
                {
                    throw new ArgumentException(
                        "Invalid configuration. AutoscaleTargetUtilization provided without enabling autoscaling.");
                }
            }
        }
    }
}
