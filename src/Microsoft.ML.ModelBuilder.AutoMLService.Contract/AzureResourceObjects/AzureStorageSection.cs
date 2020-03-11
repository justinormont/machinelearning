// <copyright file="AzureStorageSection.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace Microsoft.ML.ModelBuilder.AutoMLService
{
    public class AzureStorageSection
    {
        public string AccountName { get; set; }

        public string ContainerName { get; set; }

        public string Endpoint { get; set; }

        public string Protocol { get; set; }

        public string CredentialType { get; set; }

        public string Credential { get; set; }

        public string AccountKey { get; set; }
    }
}
