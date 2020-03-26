// <auto-generated>
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for
// license information.
//
// Code generated by Microsoft (R) AutoRest Code Generator 1.0.0.0
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>

namespace Azure.MachineLearning.Services.GeneratedOld.Models
{
    using Newtonsoft.Json;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// A paginated list of items and an optional link to the next page of
    /// items.
    /// </summary>
    public partial class PaginatedDataStoreList
    {
        /// <summary>
        /// Initializes a new instance of the PaginatedDataStoreList class.
        /// </summary>
        public PaginatedDataStoreList()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the PaginatedDataStoreList class.
        /// </summary>
        /// <param name="value">The list of items in this page.</param>
        /// <param name="continuationToken">The token used in retrieving the
        /// next page.  If null, there are no additional pages.</param>
        /// <param name="nextLink">The link to the next page constructed using
        /// the continuationToken.  If null, there are no additional
        /// pages.</param>
        public PaginatedDataStoreList(IList<DataStore> value = default(IList<DataStore>), string continuationToken = default(string), string nextLink = default(string))
        {
            Value = value;
            ContinuationToken = continuationToken;
            NextLink = nextLink;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// Gets or sets the list of items in this page.
        /// </summary>
        [JsonProperty(PropertyName = "value")]
        public IList<DataStore> Value { get; set; }

        /// <summary>
        /// Gets or sets the token used in retrieving the next page.  If null,
        /// there are no additional pages.
        /// </summary>
        [JsonProperty(PropertyName = "continuationToken")]
        public string ContinuationToken { get; set; }

        /// <summary>
        /// Gets or sets the link to the next page constructed using the
        /// continuationToken.  If null, there are no additional pages.
        /// </summary>
        [JsonProperty(PropertyName = "nextLink")]
        public string NextLink { get; set; }

    }
}
