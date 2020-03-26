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

    public partial class BatchEventCommandDto
    {
        /// <summary>
        /// Initializes a new instance of the BatchEventCommandDto class.
        /// </summary>
        public BatchEventCommandDto()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the BatchEventCommandDto class.
        /// </summary>
        public BatchEventCommandDto(IList<object> events = default(IList<object>))
        {
            Events = events;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "events")]
        public IList<object> Events { get; set; }

    }
}
