// <copyright file="GlusterFsDatastore.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

using System;
using System.Linq;

namespace Azure.MachineLearning.Services.Datastores
{
    public class GlusterFsDatastore : Datastore
    {
        /// <summary>
        /// Initializes a new instance of the GlusterFsDatastore
        /// </summary>
        public GlusterFsDatastore(
            ServiceContext serviceContext,
            GeneratedOld.Models.DataStore dataStoreDto)
            : base(serviceContext)
        {
            this.RefreshFromDto(dataStoreDto);
        }

        public GlusterFsDatastore(
            string datastoreName,
            string serverAddress,
            string volumeName)
            : base(null)
        {
            Throw.IfNullOrEmpty(datastoreName, nameof(datastoreName));
            Throw.IfNullOrEmpty(serverAddress, nameof(serverAddress));
            Throw.IfNullOrEmpty(volumeName, nameof(volumeName));

            this.DatastoreName = datastoreName;
            this.ServerAddress = serverAddress;
            this.VolumeName = volumeName;
        }

        public string ServerAddress { get; set; }

        public string VolumeName { get; set; }

        public override void RefreshFromDto(GeneratedOld.Models.DataStore dataStoreDto)
        {
            Throw.IfNull(dataStoreDto, nameof(dataStoreDto));
            Throw.IfNull(dataStoreDto.GlusterFsSection, nameof(dataStoreDto.GlusterFsSection));
            Throw.IfNullOrEmpty(dataStoreDto.Name, nameof(dataStoreDto.Name));
            this.DatastoreName = dataStoreDto.Name;
            var properties = dataStoreDto.GlusterFsSection;
            this.ServerAddress = properties.ServerAddress;
            this.VolumeName = properties.VolumeName;
            this.LastRefreshFromDto = DateTime.Now;
        }

        public override GeneratedOld.Models.DataStore ToDto()
        {
            var dto = base.ToDto();
            dto.Name = this.DatastoreName;
            dto.DataStoreType = Datastore.DatastoreType.GlusterFs.ToString();
            var glusterFsSection = new GeneratedOld.Models.GlusterFs();
            glusterFsSection.ServerAddress = this.ServerAddress;
            glusterFsSection.VolumeName = this.VolumeName;
            dto.GlusterFsSection = glusterFsSection;
            return dto;
        }
    }
}