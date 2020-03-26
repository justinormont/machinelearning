// <copyright file="DiscoveryServiceClient.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

using System;
using System.Net.Http;
using System.Threading.Tasks;

using Microsoft.Rest;

using Newtonsoft.Json;

namespace Azure.MachineLearning.Services
{
    public class DiscoveryServiceClient
    {
        public Uri CatalogEndpoint { get; set; }

        public Uri ExperimentationEndpoint { get; set; }

        public Uri GalleryEndpoint { get; set; }

        public Uri HistoryEndpoint { get; set; }

        public Uri ModelManagementEndpoint { get; set; }

        public Uri PipelinesEndpoint { get; set; }

        public async Task PopulateAsync(Uri discoveryUri)
        {
            Throw.IfNull(discoveryUri, nameof(discoveryUri));

            var client = new HttpClient();

            string responseContent = await client.GetStringAsync(discoveryUri).ConfigureAwait(false);

            try
            {
                var discoveryData =
                    Microsoft.Rest.Serialization.SafeJsonConvert.DeserializeObject<DiscoveryData>(responseContent);

                this.CatalogEndpoint = discoveryData.CatalogUri;
                this.ExperimentationEndpoint = discoveryData.ExperimentationUri;
                this.GalleryEndpoint = discoveryData.GalleryUri;
                this.HistoryEndpoint = discoveryData.HistoryUri;
                this.ModelManagementEndpoint = discoveryData.ModelManagementUri;
                this.PipelinesEndpoint = discoveryData.PipelinesUri;
            }
            catch (JsonException ex)
            {
                throw new SerializationException("Unable to deserialize the response.", responseContent, ex);
            }
        }
    }
}
