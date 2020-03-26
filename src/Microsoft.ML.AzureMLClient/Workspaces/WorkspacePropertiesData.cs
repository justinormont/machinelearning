// <copyright file="WorkspacePropertiesData.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

using System;

using Newtonsoft.Json;

namespace Azure.MachineLearning.Services.Workspaces
{
    public class WorkspacePropertiesData
    {
        [JsonProperty(PropertyName = "friendlyName")]
        public string FriendlyName { get; set; }

        [JsonProperty(PropertyName = "description")]
        public string Description { get; set; }

        [JsonProperty(PropertyName = "creationTime")]
        public DateTime CreationTime { get; set; }

        [JsonProperty(PropertyName = "storageAccount")]
        public string StorageAccount { get; set; }

        [JsonProperty(PropertyName = "containerRegistry")]
        public string ContainerRegistry { get; set; }

        [JsonProperty(PropertyName = "keyVault")]
        public string KeyVault { get; set; }

        [JsonProperty(PropertyName = "applicationInsights")]
        public string ApplicationInsights { get; set; }

        [JsonProperty(PropertyName = "workspaceId")]
        public Guid WorkspaceId { get; set; }

        [JsonProperty(PropertyName = "subscriptionState")]
        public string SubscriptionState { get; set; }

        [JsonProperty(PropertyName = "subscriptionStatusChangeTimeStampUtc")]
        public string SubscriptionStatusChangeTimeStampUtc { get; set; }

        [JsonProperty(PropertyName = "discoveryUrl")]
        public Uri DiscoveryUrl { get; set; }
    }
}
