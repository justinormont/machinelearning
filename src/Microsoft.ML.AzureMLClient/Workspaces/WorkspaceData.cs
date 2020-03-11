// <copyright file="WorkspaceData.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

using System.Collections.Generic;

using Newtonsoft.Json;

namespace Azure.MachineLearning.Services.Workspaces
{
    public class WorkspaceData
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "type")]
        public string Type { get; set; }

        [JsonProperty(PropertyName = "location")]
        public string Location { get; set; }

        [JsonProperty(PropertyName = "tags")]
        public Dictionary<string, string> Tags { get; set; }

        [JsonProperty(PropertyName = "etag")]
        public string ETag { get; set; }

        [JsonProperty(PropertyName = "properties")]
        public WorkspacePropertiesData Properties { get; set; }
    }
}
