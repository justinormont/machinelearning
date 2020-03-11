// <copyright file="ArmDataList.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;

using Newtonsoft.Json;

namespace Azure.MachineLearning.Services.Workspaces
{
    public class ArmDataList
    {
        [JsonProperty(PropertyName = "value")]
        public IList<ArmData> Resources { get; set; }

        [JsonProperty(PropertyName = "nextLink")]
        public Uri Next { get; set; }
    }
}
