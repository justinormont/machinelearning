// <copyright file="Snapshot.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

using System;

using Azure.MachineLearning.Services.GeneratedOld.Models;

namespace Azure.MachineLearning.Services.Snapshots
{
    public class Snapshot : IRefreshableFromDto<GeneratedOld.Models.SnapshotDto>
    {
        internal Snapshot(ServiceContext context, GeneratedOld.Models.SnapshotDto snapshotDto)
        {
            Throw.IfNull(context, nameof(context));
            Throw.IfNull(snapshotDto, nameof(snapshotDto));
            this.Context = context;
            this.RefreshFromDto(snapshotDto);
        }

        public ServiceContext Context { get; private set; }

        public Guid Id { get; private set; }

        public SnapshotDirectoryNode Root { get; private set; }

        public DateTime LastRefreshFromDto { get; private set; }

        public void RefreshFromDto(SnapshotDto snapshotDto)
        {
            this.Id = snapshotDto.Id.Value;
            this.Root = new SnapshotDirectoryNode(snapshotDto.Root);
            this.LastRefreshFromDto = DateTime.UtcNow;
        }
    }
}
