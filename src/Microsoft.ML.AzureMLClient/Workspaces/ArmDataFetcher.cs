// <copyright file="ArmDataFetcher.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Rest;
using Newtonsoft.Json;

namespace Azure.MachineLearning.Services.Workspaces
{
    public class ArmDataFetcher : IPageFetcher<ArmData>
    {
        protected const string WorkspaceProvider = "Microsoft.MachineLearningServices/workspaces";

        public ArmDataFetcher(WorkspaceClient client, Guid subscriptionId)
        {
            Throw.IfNull(client, nameof(client));
            this.Client = client;
            this.SubscriptionId = subscriptionId;
            this.OnLastPage = false;
        }

        public WorkspaceClient Client { get; protected set; }

        public Guid SubscriptionId { get; protected set; }

        public bool OnLastPage { get; protected set; }

        public Uri ContinuationUrl { get; protected set; }

        public Uri BuildUrl(Guid subscriptionId)
        {
            // https://docs.microsoft.com/en-us/rest/api/resources/resources/list
            // GET https://management.azure.com/subscriptions/{subscriptionId}/resources?$filter={$filter}&$expand={$expand}&$top={$top}&api-version=2018-05-01

            string apiVersion = "2018-05-01";

            // Compute URL
            string baseUri = this.Client.HttpClient.BaseAddress.AbsoluteUri;
            var uri = new System.Uri(new System.Uri(baseUri), "subscriptions/{subscriptionId}/resources").ToString();
            uri = uri.Replace("{subscriptionId}", System.Uri.EscapeDataString(Microsoft.Rest.Serialization.SafeJsonConvert.SerializeObject(subscriptionId, Client.SerializationSettings).Trim('"')));

            // Get the API version string
            var queryParameters = new List<string>();
            if (apiVersion != null)
            {
                queryParameters.Add(string.Format("api-version={0}", System.Uri.EscapeDataString(apiVersion)));
            }
            // I'm not quite sure why these aren't working, but they don't stop me figuring out the pagination
            // queryParameters.Add(string.Format("filter={0}", Uri.EscapeDataString("resourceType eq 'Microsoft.MachineLearningServices/workspaces'")));
            // queryParameters.Add(string.Format("top=10"));

            if (queryParameters.Count > 0)
            {
                uri += "?" + string.Join("&", queryParameters);
            }

            return new Uri(uri);
        }

        public IEnumerable<ArmData> FetchNextPage()
        {
            return this.FetchNextPageAsync().Result;
        }

        public async Task<IEnumerable<ArmData>> FetchNextPageAsync(
            Dictionary<string, List<string>> customHeaders = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            List<ArmData> result = null;

            Uri nextUri;
            if (this.ContinuationUrl == null)
            {
                nextUri = this.BuildUrl(this.SubscriptionId);
            }
            else
            {
                nextUri = this.ContinuationUrl;
            }

            if (!this.OnLastPage)
            {
                ArmDataList response = await this.FetchNextAsync(nextUri, customHeaders, cancellationToken).ConfigureAwait(false);

                this.ContinuationUrl = response.Next;
                this.OnLastPage = this.ContinuationUrl == null;

                result = response.Resources.Where(x => x.Type == ArmDataFetcher.WorkspaceProvider).ToList();
            }

            return result;
        }

        private async Task<ArmDataList> FetchNextAsync(
            Uri nextUri,
            Dictionary<string, List<string>> customHeaders,
            CancellationToken cancellationToken)
        {
            Throw.IfNull(nextUri, nameof(nextUri));

            // Create HTTP transport objects
            using (var httpRequest = new HttpRequestMessage())
            {
                httpRequest.Method = new HttpMethod("GET");
                httpRequest.RequestUri = nextUri;

                if (customHeaders != null)
                {
                    foreach (var header in customHeaders)
                    {
                        if (httpRequest.Headers.Contains(header.Key))
                        {
                            httpRequest.Headers.Remove(header.Key);
                        }
                        httpRequest.Headers.TryAddWithoutValidation(header.Key, header.Value);
                    }
                }

                // Set Credentials
                if (this.Client.Credentials != null)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    await this.Client.Credentials.ProcessHttpRequestAsync(httpRequest, cancellationToken).ConfigureAwait(false);
                }

                // Send Request
                using (HttpResponseMessage httpResponse = await this.Client.HttpClient.SendAsync(httpRequest, cancellationToken).ConfigureAwait(false))
                {
                    if (httpResponse.StatusCode != HttpStatusCode.OK)
                    {
                        var msg = string.Format("Got {0} response from service", (int)httpResponse.StatusCode);
                        var e = new MachineLearningServiceException(msg);
                        e.Request = new HttpRequestMessageWrapper(httpRequest, string.Empty);
                        e.Response = new HttpResponseMessageWrapper(httpResponse, string.Empty);

                        throw e;
                    }

                    var result = new HttpOperationResponse<ArmDataList>();

                    // The deserialisation needs improvement to match what Azure returns
                    result.Request = httpRequest;
                    result.Response = httpResponse;
                    string responseContent = await httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);

                    try
                    {
                        result.Body = Microsoft.Rest.Serialization.SafeJsonConvert.DeserializeObject<ArmDataList>(responseContent, this.Client.DeserializationSettings);
                    }
                    catch (JsonException ex)
                    {
                        throw new SerializationException("Unable to deserialize the response.", responseContent, ex);
                    }

                    return result.Body;
                }
            }
        }
    }
}
