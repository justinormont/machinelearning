// <auto-generated>
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for
// license information.
//
// Code generated by Microsoft (R) AutoRest Code Generator 1.0.0.0
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>

namespace Azure.MachineLearning.Services.Generated.Models
{
    using Newtonsoft.Json;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Results of the Batch Artifact Content Information request.
    /// </summary>
    public partial class BatchArtifactContentInformationResult
    {
        /// <summary>
        /// Initializes a new instance of the
        /// BatchArtifactContentInformationResult class.
        /// </summary>
        public BatchArtifactContentInformationResult()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the
        /// BatchArtifactContentInformationResult class.
        /// </summary>
        /// <param name="artifacts">Artifact details of the Artifact Ids
        /// requested.</param>
        /// <param name="artifactContentInformation">Artifact Content
        /// Information details of the Artifact Ids requested.</param>
        /// <param name="errors">Errors occurred while fetching the requested
        /// Artifact Ids.</param>
        public BatchArtifactContentInformationResult(IDictionary<string, Artifact> artifacts = default(IDictionary<string, Artifact>), IDictionary<string, ArtifactContentInformation> artifactContentInformation = default(IDictionary<string, ArtifactContentInformation>), IDictionary<string, ErrorResponse> errors = default(IDictionary<string, ErrorResponse>))
        {
            Artifacts = artifacts;
            ArtifactContentInformation = artifactContentInformation;
            Errors = errors;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// Gets or sets artifact details of the Artifact Ids requested.
        /// </summary>
        [JsonProperty(PropertyName = "artifacts")]
        public IDictionary<string, Artifact> Artifacts { get; set; }

        /// <summary>
        /// Gets or sets artifact Content Information details of the Artifact
        /// Ids requested.
        /// </summary>
        [JsonProperty(PropertyName = "artifactContentInformation")]
        public IDictionary<string, ArtifactContentInformation> ArtifactContentInformation { get; set; }

        /// <summary>
        /// Gets or sets errors occurred while fetching the requested Artifact
        /// Ids.
        /// </summary>
        [JsonProperty(PropertyName = "errors")]
        public IDictionary<string, ErrorResponse> Errors { get; set; }

    }
}
