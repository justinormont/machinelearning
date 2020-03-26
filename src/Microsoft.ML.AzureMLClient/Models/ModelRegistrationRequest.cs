// <copyright file="ModelRegistrationRequest.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace Azure.MachineLearning.Services.Models
{
    public class ModelRegistrationRequest
    {
        public string Name { get; set; }

        public string Url { get; set; }

        public string MimeType { get; set; }

        public bool Unpack { get; set; }

        public void SetUrlWithAssetId(string assetId)
        {
            this.Url = string.Format("aml://asset/{0}", assetId);
        }

        public GeneratedOld.Models.Model ToDto()
        {
            var result = new GeneratedOld.Models.Model();
            result.Name = this.Name;
            result.Url = this.Url;
            result.Unpack = this.Unpack;
            result.MimeType = this.MimeType;

            return result;
        }
    }
}
