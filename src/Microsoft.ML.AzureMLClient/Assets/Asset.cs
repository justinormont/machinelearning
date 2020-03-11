// <copyright file = "Asset.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;

namespace Azure.MachineLearning.Services.Assets
{
    public class Asset
    {
        public Asset(ServiceContext serviceContext, GeneratedOld.Models.Asset assetDto)
        {
            Throw.IfNull(serviceContext, nameof(serviceContext));
            Throw.IfNull(assetDto, nameof(assetDto));

            ServiceContext = serviceContext;
            this.RefreshFromDto(assetDto);
        }

        public ServiceContext ServiceContext { get; private set; }

        public string Id { get; private set; }

        public string Name { get; private set; }

        public string Description { get; private set; }

        public IReadOnlyList<Artifacts.Artifact> Artifacts { get; private set; }

        public IReadOnlyList<string> Tags { get; private set; }

        public string RunId { get; private set; }

        public DateTime? CreatedTime { get; private set; }

        public void RefreshFromDto(GeneratedOld.Models.Asset assetDto)
        {
            Throw.IfNull(assetDto, nameof(assetDto));

            this.Id = assetDto.Id;
            this.Name = assetDto.Name;
            this.Description = assetDto.Description;
            this.Artifacts = new List<Artifacts.Artifact>();
            if (assetDto.Artifacts != null)
            {
                this.Artifacts = assetDto.Artifacts.Select(
                    x => new Artifacts.Artifact(this.ServiceContext, x)).ToList().AsReadOnly();
            }
            this.Tags = new List<string>();
            if (assetDto.Tags != null)
            {
                this.Tags = assetDto.Tags.ToList().AsReadOnly();
            }
            this.RunId = assetDto.Runid;
            this.CreatedTime = assetDto.CreatedTime;
        }
    }
}
