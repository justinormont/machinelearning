// <copyright file="Datastore.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Azure.MachineLearning.Services.GeneratedOld;
using Microsoft.Rest;
using Newtonsoft.Json;

namespace Azure.MachineLearning.Services.Datastores
{
    public abstract class Datastore : IRefreshableFromDto<GeneratedOld.Models.DataStore>
    {
        public Datastore(ServiceContext serviceContext)
        {
            this.Context = serviceContext;
            this.LastRefreshFromDto = DateTime.Now;
        }

        public enum DatastoreType
        {
            AzureBlob,
            AzureFile,
            AzureDataLake,
            AzureSqlDatabase,
            AzurePostgreSql,
            DBFS,
            AzureDataLakeGen2,
            GlusterFs,
            Unknown
        }

        public enum StorageCredential
        {
            None,
            AccountKey,
            SaSToken
        }

        [JsonProperty(PropertyName = "name")]
        public string DatastoreName { get; protected set; }

        public bool HasBeenValidated { get; protected set; }

        public ServiceContext Context { get; protected set; }

        public DateTime LastRefreshFromDto { get; protected set; }

        public abstract void RefreshFromDto(GeneratedOld.Models.DataStore dataStoreDto);

        public async Task SetDefaultAsync(
            Dictionary<string, List<string>> customHeaders = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            Throw.IfNull(this.DatastoreName, nameof(DatastoreName));
            await RestCallWrapper.WrapAsync(
                () => this.GetDataStore().SetDefaultWithHttpMessagesAsync(
                    this.DatastoreName,
                    this.Context.SubscriptionId.ToString(),
                    this.Context.ResourceGroupName,
                    this.Context.WorkspaceName,
                    customHeaders: customHeaders,
                    cancellationToken: cancellationToken)).ConfigureAwait(false);
            return;
        }

        public async Task DeleteAsync(
            Dictionary<string, List<string>> customHeaders = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            Throw.IfNull(this.DatastoreName, nameof(DatastoreName));
            await RestCallWrapper.WrapAsync(
                () => GetDataStore().DeleteWithHttpMessagesAsync(
                    this.DatastoreName,
                    this.Context.SubscriptionId.ToString(),
                    this.Context.ResourceGroupName,
                    this.Context.WorkspaceName,
                    customHeaders: customHeaders,
                    cancellationToken: cancellationToken)).ConfigureAwait(false);
            return;
        }

        public async Task UpdateAsync(
            Dictionary<string, List<string>> customHeaders = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            Throw.IfNull(this.DatastoreName, nameof(DatastoreName));
            var dto = this.ToDto();
            await RestCallWrapper.WrapAsync(
                () => GetDataStore().UpdateWithHttpMessagesAsync(
                    this.DatastoreName,
                    this.Context.SubscriptionId.ToString(),
                    this.Context.ResourceGroupName,
                    this.Context.WorkspaceName,
                    dto,
                    customHeaders: customHeaders,
                    cancellationToken: cancellationToken)).ConfigureAwait(false);
        }

        public virtual GeneratedOld.Models.DataStore ToDto()
        {
            var dto = new GeneratedOld.Models.DataStore();
            dto.Name = this.DatastoreName;
            dto.HasBeenValidated = this.HasBeenValidated;
            dto.AzureDataLakeSection = null;
            dto.AzurePostgreSqlSection = null;
            dto.AzureSqlDatabaseSection = null;
            dto.GlusterFsSection = null;
            dto.AzureStorageSection = null;
            return dto;
        }

        protected void ValidateSasAndAccountKey(string sasToken, string accountKey)
        {
            if (sasToken == null && accountKey == null)
            {
                throw new ArgumentException("sasToken and accountKey can't both be null");
            }

            if (sasToken != null && accountKey != null)
            {
                throw new ArgumentException("use either sasToken or accountKey, not both");
            }
        }

        protected bool ValidateStorageProtocol(string protocol)
        {
            Throw.IfNull(protocol, nameof(protocol));
            var str = protocol.ToLower();
            if (str.StartsWith("http") || str.StartsWith("https"))
            {
                return true;
            }
            return false;
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