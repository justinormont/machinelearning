// <copyright file="ServiceContext.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

using System;
using System.Threading.Tasks;

using Azure.MachineLearning.Services.Workspaces;

using Microsoft.Rest;

namespace Azure.MachineLearning.Services
{
    public class ServiceContext
    {
        public ServiceClientCredentials Credentials { get; set; }

        public Guid SubscriptionId { get; private set; }

        public string ResourceGroupName { get; private set; }

        public string WorkspaceName { get; private set; }

        public string Location { get; private set; }

        public DiscoveryServiceClient Discovery { get; private set; } = new DiscoveryServiceClient();

        public Uri AzureResourceManagerEndpoint { get; private set; }

        public Uri CatalogEndpoint
        {
            get
            {
                return this.Discovery.CatalogEndpoint;
            }
        }

        public Uri ExperimentationEndpoint
        {
            get
            {
                string envVarName = this.GetEnvironmentVariableName("EXPERIMENTATION");
                return this.CheckForEnvironmentOverride(this.Discovery.ExperimentationEndpoint, envVarName);
            }
        }

        public Uri GalleryEndpoint
        {
            get
            {
                return this.Discovery.GalleryEndpoint;
            }
        }

        public Uri HistoryEndpoint
        {
            get
            {
                string envVarName = this.GetEnvironmentVariableName("RUN_HISTORY");
                return this.CheckForEnvironmentOverride(this.Discovery.HistoryEndpoint, envVarName);
            }
        }

        public Uri ModelManagementEndpoint
        {
            get
            {
                string envVarName = this.GetEnvironmentVariableName("MODEL_MANAGEMENT");
                return this.CheckForEnvironmentOverride(this.Discovery.ModelManagementEndpoint, envVarName);
            }
        }

        public Uri PipelinesEndpoint
        {
            get
            {
                string envVarName = this.GetEnvironmentVariableName("PIPELINES");
                return this.CheckForEnvironmentOverride(this.Discovery.PipelinesEndpoint, envVarName);
            }
        }

        public Uri ProjectContentEndpoint
        {
            get
            {
                string envVarName = this.GetEnvironmentVariableName("PROJECT_CONTENT");
                return this.CheckForEnvironmentOverride(this.Discovery.HistoryEndpoint, envVarName);
            }
        }

        public Uri ArtifactsEndpoint
        {
            get
            {
                string envVarName = this.GetEnvironmentVariableName("ARTIFACTS");
                return this.CheckForEnvironmentOverride(this.Discovery.HistoryEndpoint, envVarName);
            }
        }

        public Uri MetricsEndpoint
        {
            get
            {
                string envVarName = this.GetEnvironmentVariableName("METRICS");
                return this.CheckForEnvironmentOverride(this.Discovery.HistoryEndpoint, envVarName);
            }
        }

        public Uri AssetsEndpoint
        {
            get
            {
                string envVarName = this.GetEnvironmentVariableName("ASSETS");
                return this.CheckForEnvironmentOverride(this.Discovery.ModelManagementEndpoint, envVarName);
            }
        }

        public async Task PopulateAsync(
            WorkspaceClient workspaceClient,
            Uri discoveryUri,
            Guid subscriptionId,
            string resourceGroupName,
            string workspaceName,
            string location)
        {
            Throw.IfNull(workspaceClient, nameof(workspaceClient));
            Throw.IfNull(discoveryUri, nameof(discoveryUri));
            Throw.IfNullOrEmpty(resourceGroupName, nameof(resourceGroupName));
            Throw.IfNullOrEmpty(workspaceName, nameof(workspaceName));

            await this.Discovery.PopulateAsync(discoveryUri).ConfigureAwait(false);

            this.AzureResourceManagerEndpoint = new Uri("https://management.azure.com/"); // Will need to be configurable eventually
            this.SubscriptionId = subscriptionId;
            this.ResourceGroupName = resourceGroupName;
            this.WorkspaceName = workspaceName;
            this.Location = location;
            this.Credentials = workspaceClient.Credentials;
        }

        public string GetEnvironmentVariableName(string serviceSuffix)
        {
            return string.Format("AZUREML_DEV_URL_{0}", serviceSuffix);
        }

        public Uri CheckForEnvironmentOverride(Uri defaultUri, string environmentVariable)
        {
            Throw.IfNullOrEmpty(environmentVariable, nameof(environmentVariable));

            Uri result = defaultUri;

            string environmentVariableValue = Environment.GetEnvironmentVariable(environmentVariable);
            if (!string.IsNullOrEmpty(environmentVariableValue))
            {
                result = new Uri(environmentVariableValue);
            }

            return result;
        }
    }
}
