// <copyright file="DatastoreOperations.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Azure.MachineLearning.Services.GeneratedOld;
using Azure.MachineLearning.Services.GeneratedOld.Models;
using Microsoft.Rest;

namespace Azure.MachineLearning.Services.Datastores
{
    public class DatastoreOperations
    {
        public DatastoreOperations(ServiceContext serviceContext)
        {
            Throw.IfNull(serviceContext, nameof(serviceContext));
            this.Context = serviceContext;
            this.RestClient = new RestClient(this.Context.Credentials);
            this.RestClient.BaseUri = this.Context.ExperimentationEndpoint;
            this.Datastore = GetDataStore();
            this.Factory = new DatastoreFactory(this.Context);
        }

        public ServiceContext Context { get; private set; }

        protected DataStores Datastore { get; set; }

        protected RestClient RestClient { get; set; }

        private DatastoreFactory Factory { get; set; }

        public IPageFetcher<Datastore> GetPagedList()
        {
            var fetcher = new DatastorePageFetcher(this.Context);
            return fetcher;
        }

        public IEnumerable<Datastore> List()
        {
            var lister = new LazyEnumerator<Datastore>();
            lister.Fetcher = this.GetPagedList();

            return lister;
        }

        public async Task<Datastore> GetDatastoreAsync(
            string name,
            Dictionary<string, List<string>> customHeaders = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            GeneratedOld.Models.DataStore response = await RestCallWrapper.WrapAsync(
                () => GetDataStore().GetWithHttpMessagesAsync(
                    name,
                    this.Context.SubscriptionId.ToString(),
                    this.Context.ResourceGroupName,
                    this.Context.WorkspaceName,
                    customHeaders: customHeaders,
                    cancellationToken: cancellationToken)).ConfigureAwait(false);

            return Factory.ConvertFromDto(response);
        }

        public async Task<Datastore> GetDefaultDatastoreAsync(
            Dictionary<string, List<string>> customHeaders = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            GeneratedOld.Models.DataStore response = await RestCallWrapper.WrapAsync(
                () => GetDataStore().GetDefaultWithHttpMessagesAsync(
                    this.Context.SubscriptionId.ToString(),
                    this.Context.ResourceGroupName,
                    this.Context.WorkspaceName,
                    customHeaders: customHeaders,
                    cancellationToken: cancellationToken)).ConfigureAwait(false);

            return Factory.ConvertFromDto(response);
        }

        public async Task<Datastore> RegisterAzureBlobStorageDatastoreAsync(
            string datastoreName,
            string containerName,
            string accountName,
            string sasToken = null,
            string accountKey = null,
            string protocol = "https",
            string endpoint = "core.windows.net",
            bool createIfNotExists = false,
            bool skipValidation = false,
            Dictionary<string, List<string>> customHeaders = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var blobDatastore = new AzureBlobStorageDatastore(
                datastoreName,
                containerName,
                accountName,
                sasToken,
                accountKey,
                protocol,
                endpoint);

            return await CreateDatastoreAsync(
                blobDatastore,
                createIfNotExists,
                skipValidation,
                customHeaders,
                cancellationToken).ConfigureAwait(false);
        }

        public async Task<Datastore> RegisterAzureDataLakeDatastoreAsync(
            string dataStoreName,
            Guid tenantId,
            Guid clientId,
            string clientSecret,
            Uri resourceUri = null,
            Uri authorityUri = null,
            bool createifNotExists = false,
            bool skipValidation = false,
            Dictionary<string, List<string>> customHeaders = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var datalakeDatastore = new AzureDataLakeDatastore(
                dataStoreName,
                tenantId,
                clientId,
                clientSecret,
                resourceUri,
                authorityUri);

            return await CreateDatastoreAsync(
                datalakeDatastore,
                createifNotExists,
                skipValidation,
                customHeaders,
                cancellationToken).ConfigureAwait(false);
        }

        public async Task<Datastore> RegisterFileDatastoreAsync(
            string datastoreName,
            string containerName,
            string accountName,
            string sasToken = null,
            string accountKey = null,
            string protocol = "https",
            string endpoint = "core.windows.net",
            bool createIfNotExists = false,
            bool skipValidation = false,
            Dictionary<string, List<string>> customHeaders = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var azureDatastore = new AzureFileDatastore(
                datastoreName,
                containerName,
                accountName,
                sasToken,
                accountKey,
                protocol,
                endpoint);

            return await CreateDatastoreAsync(
                azureDatastore,
                createIfNotExists,
                skipValidation,
                customHeaders,
                cancellationToken).ConfigureAwait(false);
        }

        public async Task<Datastore> RegisterAzurePostgresSqlDatastoreAsync(
            string datastoreName,
            string serverName,
            string databaseName,
            string userId,
            string userPassword,
            int portNumber,
            string endpoint,
            bool createIfNotExists = false,
            bool skipValidation = false,
            Dictionary<string, List<string>> customHeaders = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var postgresSqlDatastore = new AzurePostgreSqlDatastore(
                datastoreName,
                serverName,
                databaseName,
                userId,
                userPassword,
                portNumber,
                endpoint);

            return await CreateDatastoreAsync(
                postgresSqlDatastore,
                createIfNotExists,
                skipValidation,
                customHeaders,
                cancellationToken).ConfigureAwait(false);
        }

        public async Task<Datastore> RegisterAzureSqlDatabaseDatastoreAsync(
            string datastoreName,
            string serverName,
            string databaseName,
            Guid tenantId,
            Guid clientId,
            string clientSecret,
            string resourceUri,
            string authorityUrl,
            string endpoint,
            bool createIfNotExists = false,
            bool skipValidation = false,
            Dictionary<string, List<string>> customHeaders = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var azureSqlDatastore = new AzureSqlDatabaseDatastore(
                datastoreName,
                serverName,
                databaseName,
                tenantId,
                clientId,
                clientSecret,
                resourceUri,
                authorityUrl,
                endpoint);

            return await CreateDatastoreAsync(
                azureSqlDatastore,
                createIfNotExists,
                skipValidation,
                customHeaders,
                cancellationToken).ConfigureAwait(false);
        }

        public async Task<Datastore> RegisterGlusterFsDatastoreAsync(
            string datastoreName,
            string serverAddress,
            string volumeName,
            bool createIfNotExists = false,
            bool skipValidation = false,
            Dictionary<string, List<string>> customHeaders = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var glusterFsDatastore = new GlusterFsDatastore(
                datastoreName,
                serverAddress,
                volumeName);

            return await CreateDatastoreAsync(
                glusterFsDatastore,
                createIfNotExists,
                skipValidation,
                customHeaders,
                cancellationToken).ConfigureAwait(false);
        }

        protected async Task<Datastore> CreateDatastoreAsync(
            Datastore datastore,
            bool createIfNotExists,
            bool skipValidation,
            Dictionary<string, List<string>> customHeaders = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            Throw.IfNull(datastore, nameof(datastore));
            var datastoreDto = datastore.ToDto();

            await RestCallWrapper.WrapAsync(
                () => GetDataStore().CreateWithHttpMessagesAsync(
                    this.Context.SubscriptionId.ToString(),
                    this.Context.ResourceGroupName,
                    this.Context.WorkspaceName,
                    datastoreDto,
                    createIfNotExists,
                    skipValidation,
                    customHeaders: customHeaders,
                    cancellationToken: cancellationToken)).ConfigureAwait(false);

            return await GetDatastoreAsync(datastoreDto.Name, customHeaders, cancellationToken).ConfigureAwait(false);
            // TODO(srmorin): check for 400 code, there is a special meeting for this in the python code
        }

        private DataStores GetDataStore()
        {
            RestClient restClient = new RestClient(this.Context.Credentials);
            restClient.BaseUri = this.Context.ExperimentationEndpoint;
            DataStores dataStore = new DataStores(restClient);
            return dataStore;
        }
    }
}