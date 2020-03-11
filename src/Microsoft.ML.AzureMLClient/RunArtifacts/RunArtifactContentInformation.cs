// <copyright file = "RunArtifactContentInformation.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

using System;

using Azure.MachineLearning.Services.GeneratedOld.Models;

namespace Azure.MachineLearning.Services.RunArtifacts
{
    public class RunArtifactContentInformation : IRefreshableFromDto<ArtifactContentInformationDto>
    {
        public RunArtifactContentInformation(
            ServiceContext serviceContext,
            ArtifactContentInformationDto artifactContentDto)
        {
            this.ServiceContext = serviceContext;
            this.RefreshFromDto(artifactContentDto);
        }

        public ServiceContext ServiceContext { get; private set; }

        public string ContentUri { get; set; }

        public string Origin { get; set; }

        public string Container { get; set; }

        public string Path { get; set; }

        public DateTime LastRefreshFromDto { get; private set; }

        public GeneratedOld.Models.ArtifactContentInformationDto ConvertToDto()
        {
            var artifactContentInformationDto = new ArtifactContentInformationDto(
                   this.ContentUri,
                   this.Origin,
                   this.Container,
                   this.Path);
            return artifactContentInformationDto;
        }

        public void RefreshFromDto(ArtifactContentInformationDto artifactContentInformationDto)
        {
            this.ContentUri = artifactContentInformationDto.ContentUri;
            this.Origin = artifactContentInformationDto.Origin;
            this.Container = artifactContentInformationDto.Container;
            this.Path = artifactContentInformationDto.Path;
            this.LastRefreshFromDto = DateTime.UtcNow;
        }
    }
}
