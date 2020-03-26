// <copyright file="AssetCreationRequest.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;

namespace Azure.MachineLearning.Services.Assets
{
    public class AssetCreationRequest
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public IList<IDictionary<string, string>> Artifacts { get; set; }

        public string RunId { get; set; }

        public DateTime Created { get; set; }

        public GeneratedOld.Models.Asset ToDto()
        {
            var res = new GeneratedOld.Models.Asset();
            res.Name = this.Name;
            res.Description = this.Description;
            res.Runid = this.RunId;
            res.Artifacts = this.Artifacts.Select(x =>
            {
                var y = new GeneratedOld.Models.Artifact();
                y.Prefix = x["prefix"];
                return y;
            }).ToList();
            res.CreatedTime = this.Created;

            return res;
        }
    }
}
