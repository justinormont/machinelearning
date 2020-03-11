// <copyright file="NodeStateCounts.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace Azure.MachineLearning.Services.Compute
{
    public class NodeStateCounts
    {
        public NodeStateCounts(GeneratedOld.Models.NodeStateCounts nodeStateCountsDto)
        {
            Throw.IfNull(nodeStateCountsDto, nameof(nodeStateCountsDto));
            this.IdleNodeCount = nodeStateCountsDto.IdleNodeCount;
            this.RunningNodeCount = nodeStateCountsDto.RunningNodeCount;
            this.PreparingNodeCount = nodeStateCountsDto.PreparingNodeCount;
            this.LeavingNodeCount = nodeStateCountsDto.LeavingNodeCount;
            this.PreemptedNodeCount = nodeStateCountsDto.PreemptedNodeCount;
        }

        public int? IdleNodeCount { get; private set; }

        public int? RunningNodeCount { get; private set; }

        public int? PreparingNodeCount { get; private set; }

        public int? UnusableNodeCount { get; private set; }

        public int? LeavingNodeCount { get; private set; }

        public int? PreemptedNodeCount { get; private set; }
    }
}
