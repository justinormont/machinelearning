// <copyright file = "RunArtifact.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Azure.MachineLearning.Services.GeneratedOld.Models;

namespace Azure.MachineLearning.Services.RunArtifacts
{
    public class RunArtifact : IRefreshableFromDto<ArtifactDto>
    {
        public RunArtifact(
            ServiceContext serviceContext,
            string experimentName,
            string runId,
            ArtifactDto artifactDto)
        {
            Throw.IfNull(serviceContext, nameof(serviceContext));
            Throw.IfNullOrEmpty(experimentName, nameof(experimentName));
            Throw.IfNullOrEmpty(runId, nameof(runId));
            this.ServiceContext = serviceContext;
            this.ExperimentName = experimentName;
            this.RunId = runId;
            this.RunArtifactOperations = new RunArtifactOperations(serviceContext, experimentName, runId);
            this.RefreshFromDto(artifactDto);
        }

        public ServiceContext ServiceContext { get; private set; }

        public string ArtifactId { get; private set; }

        public string Origin { get; private set; }

        public string Container { get; private set; }

        public string Path { get; private set; }

        public string Etag { get; private set; }

        public string ExperimentName { get; private set; }

        public string RunId { get; private set; }

        public RunArtifactOperations RunArtifactOperations { get; private set; }

        public DateTime? CreatedTime { get; private set; }

        public DateTime LastRefreshFromDto { get; private set; }

        public RunArtifactContentInformation GetRunArtifactMetaData(string artifactPath)
        {
            Throw.IfNullOrEmpty(artifactPath, nameof(artifactPath));
            return RunArtifactOperations.GetRunArtifactMetaDataAsync(artifactPath).Result;
        }

        public async Task<RunArtifactContentInformation> GetRunArtifactMetaDataAsync(
            string artifactPath,
            Dictionary<string, List<string>> customHeaders = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            Throw.IfNullOrEmpty(artifactPath, nameof(artifactPath));
            return await RunArtifactOperations.GetRunArtifactMetaDataAsync(artifactPath, customHeaders, cancellationToken).ConfigureAwait(false);
        }

        public string DownloadRunArtifactContentAsString(string artifactPath)
        {
            Throw.IfNullOrEmpty(artifactPath, nameof(artifactPath));
            return RunArtifactOperations.DownloadRunArtifactContentAsString(artifactPath);
        }

        public async Task<string> DownloadRunArtifactContentAsStringAsync(
            string artifactPath,
            Dictionary<string, List<string>> customHeaders = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            Throw.IfNullOrEmpty(artifactPath, nameof(artifactPath));
            return await RunArtifactOperations.DownloadRunArtifactContentAsStringAsync(artifactPath, customHeaders, cancellationToken).ConfigureAwait(false);
        }

        public void DownloadRunArtifactContentAsFile(string artifactPath, string destinationFile)
        {
            Throw.IfNullOrEmpty(artifactPath, nameof(artifactPath));
            Throw.IfNullOrEmpty(destinationFile, nameof(destinationFile));
            RunArtifactOperations.DownloadRunArtifactContentAsFile(artifactPath, destinationFile);
        }

        public async Task DownloadRunArtifactContentAsFileAsync(
            string artifactPath,
            string destinationFile,
            Dictionary<string, List<string>> customHeaders = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            Throw.IfNullOrEmpty(artifactPath, nameof(artifactPath));
            Throw.IfNullOrEmpty(destinationFile, nameof(destinationFile));
            await RunArtifactOperations.DownloadRunArtifactContentAsFileAsync(
                artifactPath,
                destinationFile,
                customHeaders,
                cancellationToken).ConfigureAwait(false);
        }

        public ArtifactDto ConvertToDto()
        {
            var artifactDto = new ArtifactDto();
            artifactDto.ArtifactId = this.ArtifactId;
            artifactDto.Origin = this.Origin;
            artifactDto.Container = this.Container;
            artifactDto.Path = this.Path;
            artifactDto.Etag = this.Etag;
            artifactDto.CreatedTime = this.CreatedTime;
            return artifactDto;
        }

        public void RefreshFromDto(ArtifactDto artifactDto)
        {
            this.ArtifactId = artifactDto.ArtifactId;
            this.Origin = artifactDto.Origin;
            this.Container = artifactDto.Container;
            this.Path = artifactDto.Path;
            this.Etag = artifactDto.Etag;
            this.CreatedTime = artifactDto.CreatedTime;
            this.LastRefreshFromDto = DateTime.UtcNow;
        }
    }
}
