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
    using System.Linq;

    public partial class ArtifactDto
    {
        /// <summary>
        /// Initializes a new instance of the ArtifactDto class.
        /// </summary>
        public ArtifactDto()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the ArtifactDto class.
        /// </summary>
        public ArtifactDto(string artifactId = default(string), string origin = default(string), string container = default(string), string path = default(string), string etag = default(string), System.DateTime? createdTime = default(System.DateTime?))
        {
            ArtifactId = artifactId;
            Origin = origin;
            Container = container;
            Path = path;
            Etag = etag;
            CreatedTime = createdTime;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "artifactId")]
        public string ArtifactId { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "origin")]
        public string Origin { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "container")]
        public string Container { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "path")]
        public string Path { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "etag")]
        public string Etag { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "createdTime")]
        public System.DateTime? CreatedTime { get; set; }

    }
}
