// <copyright file="RunOptions.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace Azure.MachineLearning.Services.Runs
{
    public class RunOptions
    {
        public RunOptions()
        {
        }

        public RunOptions(GeneratedOld.Models.RunOptions runOptionsDto)
        {
            Throw.IfNull(runOptionsDto, nameof(runOptionsDto));
            this.GenerateDataContainerIdIfNotSpecified = runOptionsDto.GenerateDataContainerIdIfNotSpecified;
        }

        public bool? GenerateDataContainerIdIfNotSpecified { get; set; }
    }
}
