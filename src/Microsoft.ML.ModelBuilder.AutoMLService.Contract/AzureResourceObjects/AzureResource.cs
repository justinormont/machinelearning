// <copyright file="AzureResource.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

using System.Collections.Generic;

/// <summary>
/// Wrapper around Azure management Resource. Projects referencing these types (i.e. ModelBuilder.csproj
/// don't have to take a dependency on Microsoft.Azure.Management.ResourceManager.
/// </summary>
namespace Microsoft.ML.ModelBuilder.AutoMLService.Contract.AzureResourceObjects
{
    public class AzureResource
    {
        /// <summary>
        /// Gets or sets resource ID.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets resource name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets resource type.
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Gets or sets resource location.
        /// </summary>
        public string Location { get; set; }
  
        /// <summary>
        /// Gets or sets resource tags.
        /// </summary>
        public IDictionary<string, string> Tags { get; set; }
    }
}
