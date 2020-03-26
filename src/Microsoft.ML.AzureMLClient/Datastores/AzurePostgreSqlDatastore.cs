// <copyright file="AzurePostgreSqlDatastore.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

using System;

namespace Azure.MachineLearning.Services.Datastores
{
    public class AzurePostgreSqlDatastore : Datastore
    {
        /// <summary>
        /// Initialize new instance of AzureDataLakeDatastore
        /// </summary>
        public AzurePostgreSqlDatastore(
            ServiceContext serviceContext,
            GeneratedOld.Models.DataStore dataStoreDto)
            : base(serviceContext)
        {
            this.RefreshFromDto(dataStoreDto);
        }

        public AzurePostgreSqlDatastore(
            string datastoreName,
            string serverName,
            string databaseName,
            string userId,
            string userPassword,
            int portNumber,
            string endpoint)
            : base(null)
        {
            Throw.IfNullOrEmpty(datastoreName, nameof(datastoreName));
            Throw.IfNullOrEmpty(serverName, nameof(serverName));
            Throw.IfNullOrEmpty(databaseName, nameof(databaseName));
            Throw.IfNullOrEmpty(userId, nameof(userId));
            Throw.IfNullOrEmpty(endpoint, nameof(endpoint));
            if (!ValidatePort(portNumber))
            {
                throw new ArgumentException(string.Format(
                    "{0} is not a valid port between 0 and 65535.",
                    portNumber));
            }

            this.DatastoreName = datastoreName;
            this.ServerName = serverName;
            this.DatabaseName = databaseName;
            this.UserId = userId;
            this.UserPassword = userPassword;
            this.PortNumber = portNumber.ToString();
            this.Endpoint = endpoint;
        }

        public string ServerName { get; set; }

        public string DatabaseName { get; set; }

        public string UserId { get; set; }

        public string UserPassword { get; set; }

        public string PortNumber { get; set; }

        public string Endpoint { get; set; }

        public override void RefreshFromDto(GeneratedOld.Models.DataStore dataStoreDto)
        {
            Throw.IfNull(dataStoreDto, nameof(dataStoreDto));
            Throw.IfNull(dataStoreDto.AzurePostgreSqlSection, nameof(dataStoreDto.AzurePostgreSqlSection));
            Throw.IfNullOrEmpty(dataStoreDto.Name, nameof(dataStoreDto.Name));
            this.DatastoreName = dataStoreDto.Name;
            GeneratedOld.Models.AzurePostgreSql properties = dataStoreDto.AzurePostgreSqlSection;
            ServerName = properties.ServerName;
            DatabaseName = properties.DatabaseName;
            UserId = properties.UserId;
            UserPassword = properties.UserPassword;
            PortNumber = properties.PortNumber;
            Endpoint = properties.Endpoint;
            this.LastRefreshFromDto = DateTime.Now;
        }

        public override GeneratedOld.Models.DataStore ToDto()
        {
            GeneratedOld.Models.DataStore dto = base.ToDto();
            dto.Name = this.DatastoreName;
            dto.DataStoreType = Datastore.DatastoreType.AzurePostgreSql.ToString();
            var azurePostgreSqlSection = new GeneratedOld.Models.AzurePostgreSql();
            dto.AzurePostgreSqlSection = azurePostgreSqlSection;
            return dto;
        }

        private bool ValidatePort(int x)
        {
            if (x >= 0 && x < 65536)
            {
                return true;
            }
            return false;
        }
    }
}