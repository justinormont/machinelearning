// <copyright file="Datasource.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace Microsoft.ML.ModelBuilder.AutoMLService
{
    public class Datasource
    {
        public string Name { get; set; }

        public string DataStoreType { get; set; }

        public bool HasBeenValidated { get; set; }

        public AzureStorageSection AzureStorageSection { get; set; }
    }
}
