// <copyright file="DiscoveryData.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

using System;

using Newtonsoft.Json;

namespace Azure.MachineLearning.Services
{
    public class DiscoveryData
    {
        [JsonProperty(PropertyName = "catalog")]
        public Uri CatalogUri { get; set; }

        [JsonProperty(PropertyName = "experimentation")]
        public Uri ExperimentationUri { get; set; }

        [JsonProperty(PropertyName = "gallery")]
        public Uri GalleryUri { get; set; }

        [JsonProperty(PropertyName = "history")]
        public Uri HistoryUri { get; set; }

        [JsonProperty(PropertyName = "modelmanagement")]
        public Uri ModelManagementUri { get; set; }

        [JsonProperty(PropertyName = "pipelines")]
        public Uri PipelinesUri { get; set; }
    }
}
