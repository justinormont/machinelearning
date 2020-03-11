// <copyright file = "ArtifactContent.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

using System;
using Azure.MachineLearning.Services.Generated.Models;

namespace Azure.MachineLearning.Services.Artifacts
{
    public class ArtifactContent
    {
        public ArtifactContent(
            ServiceContext serviceContext,
            ArtifactContentInformation artifactContentInformationDto)
        {
            this.ServiceContext = serviceContext;
            this.RefreshFromDto(artifactContentInformationDto);
        }

        public ServiceContext ServiceContext { get; private set; }

        public Uri ContentUri { get; private set; }

        public string Origin { get; private set; }

        public string Container { get; private set; }

        public string Path { get; private set; }

        public void RefreshFromDto(ArtifactContentInformation artifactContentInformationDto)
        {
            this.ContentUri = new Uri(artifactContentInformationDto.ContentUri);
            this.Origin = artifactContentInformationDto.Origin;
            this.Container = artifactContentInformationDto.Container;
            this.Path = artifactContentInformationDto.Path;
        }
    }
}
