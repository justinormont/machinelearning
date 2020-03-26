// <copyright file="ScaleSettings.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

using System;

namespace Azure.MachineLearning.Services.Compute
{
    public class ScaleSettings
    {
        public ScaleSettings(GeneratedOld.Models.ScaleSettings scaleSettingsDto = null)
        {
            if (scaleSettingsDto != null)
            {
                this.MinNodeCount = scaleSettingsDto.MinNodeCount;
                this.MaxNodeCount = scaleSettingsDto.MaxNodeCount;
                this.NodeIdleTimeBeforeScaleDown = scaleSettingsDto.NodeIdleTimeBeforeScaleDown;
            }
        }

        public int? MinNodeCount { get; set; }

        public int MaxNodeCount { get; set; }

        public TimeSpan? NodeIdleTimeBeforeScaleDown { get; set; }
    }
}
