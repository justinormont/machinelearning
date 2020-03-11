// <copyright file="WebservicePageFetcher.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Azure.MachineLearning.Services.GeneratedOld;
using Azure.MachineLearning.Services.GeneratedOld.Models;

namespace Azure.MachineLearning.Services.Webservices
{
    public class WebservicePageFetcher : IPageFetcher<ServiceResponseBase>
    {
        public WebservicePageFetcher(
            ServiceContext serviceContext,
            string tag = null,
            string properties = null,
            string name = null,
            string imageId = null,
            string imageName = null,
            string modelId = null,
            string modelName = null,
            string computeType = null)
        {
            Throw.IfNull(serviceContext, nameof(serviceContext));
            ServiceContext = serviceContext;

            Tag = tag;
            Properties = properties;
            Name = name;
            ImageId = imageId;
            ImageName = imageName;
            ModelId = modelId;
            ModelName = modelName;
            ComputeType = computeType;
        }

        public bool OnLastPage { get; private set; }

        private ServiceContext ServiceContext { get; set; }

        private string NextLink { get; set; }

        private string Tag { get; set; }

        private string Properties { get; set; }

        private string Name { get; set; }

        private string ImageId { get; set; }

        private string ImageName { get; set; }

        private string ModelId { get; set; }

        private string ModelName { get; set; }

        private string ComputeType { get; set; }

        public IEnumerable<ServiceResponseBase> FetchNextPage()
        {
            return this.FetchNextPageAsync().Result;
        }

        public async Task<IEnumerable<ServiceResponseBase>> FetchNextPageAsync(
            Dictionary<string, List<string>> customHeaders = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            List<ServiceResponseBase> result = new List<ServiceResponseBase>();

            if (!this.OnLastPage)
            {
                RestClient restClient = new RestClient(this.ServiceContext.Credentials);
                restClient.BaseUri = this.ServiceContext.ModelManagementEndpoint;

                PaginatedServiceList response = await RestCallWrapper.WrapAsync(
                    () => restClient.Service.ListQueryWithHttpMessagesAsync(
                        ServiceContext.SubscriptionId,
                        ServiceContext.ResourceGroupName,
                        ServiceContext.WorkspaceName,
                        tag: this.Tag,
                        properties: this.Properties,
                        name: this.Name,
                        imageId: this.ImageId,
                        imageName: this.ImageName,
                        modelId: this.ModelId,
                        modelName: this.ModelName,
                        computeType: this.ComputeType,
                        customHeaders: customHeaders,
                        cancellationToken: cancellationToken)).ConfigureAwait(false);

                this.NextLink = response.NextLink;
                this.OnLastPage = string.IsNullOrEmpty(this.NextLink);

                result.AddRange(response.Value);
            }
            return result;
        }
    }
}
