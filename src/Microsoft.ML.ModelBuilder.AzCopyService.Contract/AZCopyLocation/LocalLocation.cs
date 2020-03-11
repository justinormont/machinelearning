// <copyright file="LocalLocation.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace Microsoft.ML.ModelBuilder.AzCopyService.Contract
{
    public class LocalLocation : IAZCopyLocation
    {
        public string Path { get; set; }

        public bool UseWildCard { get; set; }

        public string LocationToString()
        {
            return this.Path + (this.UseWildCard ? "*" : string.Empty);
        }
    }
}
