// <copyright file="AzureFileDatastore.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

using System;
using System.Linq;

namespace Azure.MachineLearning.Services.Datastores
{
    public class AzureFileDatastore : Datastore
    {
        /// <summary>
        /// Initializes a new instance of the AzureFileDatastore class.
        /// </summary>
        public AzureFileDatastore(
            ServiceContext serviceContext,
            GeneratedOld.Models.DataStore dataStoreDto)
            : base(serviceContext)
        {
            this.RefreshFromDto(dataStoreDto);
        }

        public AzureFileDatastore(
            string datastoreName,
            string containerName,
            string accountName,
            string sasToken = null,
            string accountKey = null,
            string protocol = "https",
            string endpoint = "core.windows.net")
            : base(null)
        {
            Throw.IfNullOrEmpty(datastoreName, nameof(datastoreName));
            Throw.IfNullOrEmpty(containerName, nameof(containerName));
            Throw.IfNullOrEmpty(accountName, nameof(accountName));
            Throw.IfNullOrEmpty(endpoint, nameof(endpoint));

            ValidateSasAndAccountKey(sasToken, accountKey);
            ValidateStorageProtocol(protocol);

            if (containerName.Any(char.IsUpper))
            {
                throw new ArgumentException("containerName can not contain uppercase characters.");
            }

            this.DatastoreName = datastoreName;
            this.ContainerName = containerName;
            this.AccountName = accountName;
            this.AccountKey = accountKey;
            this.CredentialType = StorageCredential.AccountKey;
            this.Credential = accountKey;
            this.Endpoint = endpoint;
            this.IsSas = !(sasToken == null);
            this.Protocol = protocol;
        }

        public string AccountName { get; set; }

        public string ContainerName { get; set; }

        public string Endpoint { get; set; }

        public string Protocol { get; set; }

        public StorageCredential CredentialType { get; set; }

        public string Credential { get; set; }

        public int? BlobCacheTimeout { get; set; }

        public bool? IsSas { get; set; }

        public string AccountKey { get; set; }

        public string SasToken { get; set; }

        public override void RefreshFromDto(GeneratedOld.Models.DataStore dataStoreDto)
        {
            Throw.IfNull(dataStoreDto, nameof(dataStoreDto));
            Throw.IfNull(dataStoreDto.AzureStorageSection, nameof(dataStoreDto.AzureStorageSection));
            Throw.IfNullOrEmpty(dataStoreDto.Name, nameof(dataStoreDto.Name));
            this.DatastoreName = dataStoreDto.Name;
            var properties = dataStoreDto.AzureStorageSection;
            this.AccountKey = properties.AccountKey;
            this.AccountName = properties.AccountName;
            this.BlobCacheTimeout = properties.BlobCacheTimeout;
            this.ContainerName = properties.ContainerName;
            this.Credential = properties.Credential;
            this.CredentialType =
                StaticHelpers.ParseEnum<StorageCredential>(
                    properties.CredentialType,
                    true,
                    StorageCredential.None);
            this.Endpoint = properties.Endpoint;
            this.IsSas = properties.IsSas;
            this.Protocol = properties.Protocol;
            this.SasToken = properties.SasToken;
            this.LastRefreshFromDto = DateTime.Now;
        }

        public override GeneratedOld.Models.DataStore ToDto()
        {
            var dto = base.ToDto();
            dto.DataStoreType = Datastore.DatastoreType.AzureFile.ToString();
            var azureStorageSection = new GeneratedOld.Models.AzureStorage();
            azureStorageSection.AccountKey = this.AccountKey;
            azureStorageSection.AccountName = this.AccountName;
            azureStorageSection.BlobCacheTimeout = this.BlobCacheTimeout;
            azureStorageSection.ContainerName = this.ContainerName;
            azureStorageSection.Credential = this.Credential;
            azureStorageSection.CredentialType = this.CredentialType.ToString();
            azureStorageSection.Endpoint = this.Endpoint;
            azureStorageSection.IsSas = this.IsSas;
            azureStorageSection.Protocol = this.Protocol;
            azureStorageSection.SasToken = this.SasToken;
            dto.AzureStorageSection = azureStorageSection;
            return dto;
        }
    }
}