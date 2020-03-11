// <copyright file="DatastoreFactory.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

using System;

namespace Azure.MachineLearning.Services.Datastores
{
    public class DatastoreFactory
    {
        public DatastoreFactory(ServiceContext serviceContext)
        {
            Throw.IfNull(serviceContext, nameof(serviceContext));
            this.Context = serviceContext;
        }

        public ServiceContext Context { get; private set; }

        public Datastore ConvertFromDto(GeneratedOld.Models.DataStore dataStoreDto)
        {
            Throw.IfNull(dataStoreDto, nameof(dataStoreDto));
            Datastore.DatastoreType datastore =
                StaticHelpers.ParseEnum<Datastore.DatastoreType>(
                    dataStoreDto.DataStoreType,
                    true,
                    Datastore.DatastoreType.Unknown);

            switch (datastore)
            {
                case Datastore.DatastoreType.AzureBlob:
                    return new AzureBlobStorageDatastore(this.Context, dataStoreDto);

                case Datastore.DatastoreType.AzureDataLake:
                    return new AzureDataLakeDatastore(this.Context, dataStoreDto);

                case Datastore.DatastoreType.AzureFile:
                    return new AzureFileDatastore(this.Context, dataStoreDto);

                case Datastore.DatastoreType.AzurePostgreSql:
                    return new AzureSqlDatabaseDatastore(this.Context, dataStoreDto);

                case Datastore.DatastoreType.AzureSqlDatabase:
                    return new AzureSqlDatabaseDatastore(this.Context, dataStoreDto);

                case Datastore.DatastoreType.GlusterFs:
                    return new GlusterFsDatastore(this.Context, dataStoreDto);
            }

            throw new ArgumentOutOfRangeException(
                $"Unknown storage type {dataStoreDto.DataStoreType.ToString()}.");
        }
    }
}
