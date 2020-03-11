// <copyright file="AzureSqlDatabaseDatastore.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

using System;

namespace Azure.MachineLearning.Services.Datastores
{
    public class AzureSqlDatabaseDatastore : Datastore
    {
        /// <summary>
        /// Initialize new instance of AzureSqlDatabaseDatastore
        /// </summary>
        public AzureSqlDatabaseDatastore(
            ServiceContext serviceContext,
            GeneratedOld.Models.DataStore dataStoreDto)
            : base(serviceContext)
        {
            this.RefreshFromDto(dataStoreDto);
        }

        public AzureSqlDatabaseDatastore(
            string datastoreName,
            string serverName,
            string databaseName,
            Guid tenantId,
            Guid clientId,
            string clientSecret,
            string resourceUri,
            string authorityUri,
            string endpoint)
            : base(null)
        {
            Throw.IfNullOrEmpty(datastoreName, nameof(datastoreName));
            Throw.IfNullOrEmpty(serverName, nameof(serverName));
            Throw.IfNullOrEmpty(databaseName, nameof(databaseName));
            Throw.IfNullOrEmpty(clientSecret, nameof(clientSecret));
            Throw.IfNullOrEmpty(resourceUri, nameof(resourceUri));
            Throw.IfNullOrEmpty(authorityUri, nameof(authorityUri));
            Throw.IfNullOrEmpty(endpoint, nameof(endpoint));
            this.DatastoreName = datastoreName;
            this.ServerName = serverName;
            this.DatabaseName = databaseName;
            this.ClientSecret = clientSecret;
            this.ClientId = clientId;
            this.TenantId = tenantId;
            this.ResourceUri = resourceUri;
            this.AuthorityUri = authorityUri;
        }

        public string ServerName { get; set; }

        public string DatabaseName { get; set; }

        public string Endpoint { get; set; }

        public System.Guid? ClientId { get; set; }

        public System.Guid? TenantId { get; set; }

        public bool? IsCertAuth { get; set; }

        public string Certificate { get; set; }

        public string Thumbprint { get; set; }

        public string ClientSecret { get; set; }

        public string AuthorityUri { get; set; }

        public string ResourceUri { get; set; }

        public override void RefreshFromDto(GeneratedOld.Models.DataStore dataStoreDto)
        {
            Throw.IfNull(dataStoreDto, nameof(dataStoreDto));
            Throw.IfNull(dataStoreDto.AzureSqlDatabaseSection, nameof(dataStoreDto.AzureSqlDatabaseSection));
            Throw.IfNullOrEmpty(dataStoreDto.Name, nameof(dataStoreDto.Name));
            this.DatastoreName = dataStoreDto.Name;
            GeneratedOld.Models.AzureSqlDatabase properties = dataStoreDto.AzureSqlDatabaseSection;
            ServerName = properties.ServerName;
            DatabaseName = properties.DatabaseName;
            Endpoint = properties.Endpoint;
            ClientId = properties.ClientId;
            TenantId = properties.TenantId;
            IsCertAuth = properties.IsCertAuth;
            Certificate = properties.Certificate;
            Thumbprint = properties.Thumbprint;
            ClientSecret = properties.ClientSecret;
            AuthorityUri = properties.AuthorityUrl;
            ResourceUri = properties.ResourceUri;
            LastRefreshFromDto = DateTime.Now;
        }

        public override GeneratedOld.Models.DataStore ToDto()
        {
            var dto = new GeneratedOld.Models.DataStore();
            dto.Name = this.DatastoreName;
            dto.DataStoreType = Datastore.DatastoreType.AzureSqlDatabase.ToString();
            var azureSqlDatabaseSection = new GeneratedOld.Models.AzureSqlDatabase();
            azureSqlDatabaseSection.AuthorityUrl = this.AuthorityUri;
            azureSqlDatabaseSection.Certificate = this.Certificate;
            azureSqlDatabaseSection.ClientId = this.ClientId;
            azureSqlDatabaseSection.ClientSecret = this.ClientSecret;
            azureSqlDatabaseSection.DatabaseName = this.DatabaseName;
            azureSqlDatabaseSection.Endpoint = this.Endpoint;
            azureSqlDatabaseSection.IsCertAuth = this.IsCertAuth;
            azureSqlDatabaseSection.ResourceUri = this.ResourceUri;
            azureSqlDatabaseSection.ServerName = this.ServerName;
            azureSqlDatabaseSection.TenantId = this.TenantId;
            azureSqlDatabaseSection.Thumbprint = this.Thumbprint;
            dto.AzureSqlDatabaseSection = azureSqlDatabaseSection;
            return dto;
        }
    }
}