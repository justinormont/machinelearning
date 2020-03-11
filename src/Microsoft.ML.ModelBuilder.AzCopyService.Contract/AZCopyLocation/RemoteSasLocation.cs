// <copyright file="RemoteSasLocation.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace Microsoft.ML.ModelBuilder.AzCopyService.Contract
{
    public class RemoteSasLocation : IAZCopyLocation
    {
        public string Path { get; set; }

        public bool UseWildCard { get; set; }

        public string SasToken { get; set; }

        public string Container { get; set; }

        public string ResourceUri { get; set; }

        public string LocationToString()
        {
            return $"{this.ResourceUri}/{this.Container}/{this.Path}{this.SasToken}";
        }
    }
}
