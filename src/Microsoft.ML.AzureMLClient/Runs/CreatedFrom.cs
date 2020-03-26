// <copyright file="CreatedFrom.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace Azure.MachineLearning.Services.Runs
{
    public class CreatedFrom
    {
        public CreatedFrom(GeneratedOld.Models.CreatedFromDto createdFromDto)
        {
            Throw.IfNull(createdFromDto, nameof(createdFromDto));
            this.Type = createdFromDto.Type;
            this.LocationType = createdFromDto.LocationType;
            this.Location = createdFromDto.Location;
        }

        public string Type { get; set; }

        public string LocationType { get; set; }

        public string Location { get; set; }
    }
}
