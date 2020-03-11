// <copyright file="DatastorePageFetcher.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Azure.MachineLearning.Services.GeneratedOld;

namespace Azure.MachineLearning.Services.Datastores
{
    public class DatastorePageFetcher : IPageFetcher<Datastore>
    {
        public DatastorePageFetcher(ServiceContext serviceContext)
        {
            Throw.IfNull(serviceContext, nameof(serviceContext));
            this.OnLastPage = false;
            this.ContinuationToken = default(string);
            this.ServiceContext = serviceContext;
            this.Factory = new DatastoreFactory(this.ServiceContext);
        }

        public ServiceContext ServiceContext { get; private set; }

        public DatastoreFactory Factory { get; private set; }

        public bool OnLastPage { get; private set; }

        public string ContinuationToken { get; private set; }

        public IEnumerable<Datastore> FetchNextPage()
        {
            return this.FetchNextPageAsync().Result;
        }

        public async Task<IEnumerable<Datastore>> FetchNextPageAsync(
            Dictionary<string, List<string>> customHeaders = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            List<Datastore> result = new List<Datastore>();

            if (!this.OnLastPage)
            {
                var restClient = new GeneratedOld.RestClient(this.ServiceContext.Credentials);
                restClient.BaseUri = this.ServiceContext.ExperimentationEndpoint;

                GeneratedOld.Models.PaginatedDataStoreList response = await RestCallWrapper.WrapAsync(
                    () => restClient.DataStores.ListWithHttpMessagesAsync(
                        this.ServiceContext.SubscriptionId.ToString(),
                        this.ServiceContext.ResourceGroupName,
                        this.ServiceContext.WorkspaceName,
                        continuationToken: this.ContinuationToken,
                        customHeaders: customHeaders,
                        cancellationToken: cancellationToken)).ConfigureAwait(false);

                this.ContinuationToken = response.ContinuationToken;
                this.OnLastPage = string.IsNullOrEmpty(this.ContinuationToken);

                foreach (var datastore in response.Value)
                {
                    result.Add(this.Factory.ConvertFromDto(datastore));
                }
            }
            return result;
        }
    }
}