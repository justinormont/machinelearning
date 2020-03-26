// <copyright file = "Artifact.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved
// </copyright>

namespace Azure.MachineLearning.Services.Artifacts
{
    public class Artifact
    {
        public Artifact(ServiceContext serviceContext, GeneratedOld.Models.Artifact artifactDto)
        {
            this.ServiceContext = serviceContext;
            this.RefreshFromDto(artifactDto);
        }

        public ServiceContext ServiceContext { get; private set; }

        public string Id { get; private set; }

        public string Prefix { get; private set; }

        public void RefreshFromDto(GeneratedOld.Models.Artifact artifactDto)
        {
            this.Id = artifactDto.Id;
            this.Prefix = artifactDto.Prefix;
        }
    }
}
