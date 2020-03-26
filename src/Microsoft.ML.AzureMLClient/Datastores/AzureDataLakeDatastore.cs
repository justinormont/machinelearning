// <copyright file="AzureDataLakeDatastore.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

using System;

namespace Azure.MachineLearning.Services.Datastores
{
    public class AzureDataLakeDatastore : Datastore
    {
        /// <summary>
        /// Initialize new instance of AzureDataLakeDatastore
        /// </summary>
        public AzureDataLakeDatastore(
            ServiceContext serviceContext,
            GeneratedOld.Models.DataStore dataStoreDto)
            : base(serviceContext)
        {
            this.RefreshFromDto(dataStoreDto);
        }

        public AzureDataLakeDatastore(
            string datastoreName,
            Guid tenantId,
            Guid clientId,
            string clientSecret,
            Uri resourceUri = null,
            Uri authorityUri = null)
            : base(null)
        {
            Throw.IfNullOrEmpty(datastoreName, nameof(datastoreName));
            Throw.IfNullOrEmpty(clientSecret, nameof(clientSecret));
            this.DatastoreName = datastoreName;
            this.TenantId = tenantId;
            this.ClientId = clientId;
            this.ClientSecret = clientSecret;
            this.ResourceUri = resourceUri;
            this.AuthorityUri = authorityUri;
            this.IsCertAuth = false;
        }

        public string StoreName { get; set; }

        public System.Guid? SubscriptionId { get; set; }

        public string ResourceGroup { get; set; }

        public System.Guid? ClientId { get; set; }

        public System.Guid? TenantId { get; set; }

        public bool? IsCertAuth { get; set; }

        public string Certificate { get; set; }

        public string Thumbprint { get; set; }

        public string ClientSecret { get; set; }

        public Uri AuthorityUri { get; set; }

        public Uri ResourceUri { get; set; }

        public override void RefreshFromDto(GeneratedOld.Models.DataStore dataStoreDto)
        {
            Throw.IfNull(dataStoreDto, nameof(dataStoreDto));
            Throw.IfNull(dataStoreDto.AzureDataLakeSection, nameof(dataStoreDto.AzureDataLakeSection));
            Throw.IfNullOrEmpty(dataStoreDto.Name, nameof(dataStoreDto.Name));
            this.DatastoreName = dataStoreDto.Name;
            var properties = dataStoreDto.AzureDataLakeSection;
            StoreName = properties.StoreName;
            SubscriptionId = properties.SubscriptionId;
            ResourceGroup = properties.ResourceGroup;
            ClientId = properties.ClientId;
            TenantId = properties.TenantId;
            IsCertAuth = properties.IsCertAuth;
            Certificate = properties.Certificate;
            Thumbprint = properties.Thumbprint;
            ClientSecret = properties.ClientSecret;
            AuthorityUri = new Uri(properties.AuthorityUrl);
            ResourceUri = new Uri(properties.ResourceUri);
            this.LastRefreshFromDto = DateTime.Now;
        }

        public override GeneratedOld.Models.DataStore ToDto()
        {
            var dto = new GeneratedOld.Models.DataStore();
            dto.Name = this.DatastoreName;
            dto.DataStoreType = Datastore.DatastoreType.AzureDataLake.ToString();
            var azureDataLakeSection = new GeneratedOld.Models.AzureDataLake();
            azureDataLakeSection.StoreName = this.StoreName;
            azureDataLakeSection.SubscriptionId = this.SubscriptionId;
            azureDataLakeSection.ResourceGroup = this.ResourceGroup;
            azureDataLakeSection.ClientId = this.ClientId;
            azureDataLakeSection.TenantId = this.TenantId;
            azureDataLakeSection.IsCertAuth = this.IsCertAuth;
            azureDataLakeSection.Certificate = this.Certificate;
            azureDataLakeSection.Thumbprint = this.Thumbprint;
            azureDataLakeSection.ClientSecret = this.ClientSecret;
            azureDataLakeSection.AuthorityUrl = this.AuthorityUri?.ToString();
            azureDataLakeSection.ResourceUri = this.ResourceUri?.ToString();
            dto.AzureDataLakeSection = azureDataLakeSection;
            return dto;
        }
    }
}