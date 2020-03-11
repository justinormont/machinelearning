// <copyright file="SystemServiceInformation.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

using System;

namespace Azure.MachineLearning.Services.Compute
{
    public class SystemServiceInformation :
        IRefreshableFromDto<GeneratedOld.Models.SystemService>
    {
        public SystemServiceInformation(GeneratedOld.Models.SystemService systemServiceDto = null)
        {
            if (systemServiceDto != null)
            {
                this.RefreshFromDto(systemServiceDto);
            }
        }

        public string SystemServiceType { get; private set; }

        public string PublicIpAddress { get; private set; }

        public string Version { get; private set; }

        public DateTime LastRefreshFromDto { get; private set; }

        public void RefreshFromDto(GeneratedOld.Models.SystemService systemServiceDto)
        {
            Throw.IfNull(systemServiceDto, nameof(systemServiceDto));

            this.SystemServiceType = systemServiceDto.SystemServiceType;
            this.PublicIpAddress = systemServiceDto.PublicIpAddress;
            this.Version = systemServiceDto.Version;

            this.LastRefreshFromDto = DateTime.UtcNow;
        }
    }
}
