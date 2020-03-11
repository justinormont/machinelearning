// <copyright file="SnapshotDirectoryNode.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using Azure.MachineLearning.Services.GeneratedOld.Models;

namespace Azure.MachineLearning.Services.Snapshots
{
    public class SnapshotDirectoryNode : IRefreshableFromDto<GeneratedOld.Models.DirTreeNode>
    {
        internal SnapshotDirectoryNode(GeneratedOld.Models.DirTreeNode dirTreeNodeDto)
        {
            Throw.IfNull(dirTreeNodeDto, nameof(dirTreeNodeDto));
            this.RefreshFromDto(dirTreeNodeDto);
        }

        public string Name { get; set; }

        public string Hash { get; set; }

        public string Type { get; set; }

        public DateTime Timestamp { get; set; }

        public Uri SasUrl { get; set; }

        public long? SizeInBytes { get; set; }

        public IDictionary<string, SnapshotDirectoryNode> Children { get; set; }

        public DateTime LastRefreshFromDto { get; private set; }

        public void RefreshFromDto(DirTreeNode dirTreeNodeDto)
        {
            this.Name = dirTreeNodeDto.Name;
            this.Hash = dirTreeNodeDto.Hash;
            this.Type = dirTreeNodeDto.Type;
            this.Timestamp = dirTreeNodeDto.Timestamp.Value;
            if (dirTreeNodeDto.SasUrl != null)
            {
                this.SasUrl = new Uri(dirTreeNodeDto.SasUrl);
            }
            this.SizeInBytes = dirTreeNodeDto.SizeBytes;
            if (dirTreeNodeDto.Children != null)
            {
                this.Children = new Dictionary<string, SnapshotDirectoryNode>();
                foreach (var kvPair in dirTreeNodeDto.Children)
                {
                    this.Children.Add(kvPair.Key, new SnapshotDirectoryNode(kvPair.Value));
                }
            }
            this.LastRefreshFromDto = DateTime.UtcNow;
        }
    }
}
