// <copyright file="ImagePageFetcher.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Azure.MachineLearning.Services.GeneratedOld;
using Azure.MachineLearning.Services.GeneratedOld.Models;

namespace Azure.MachineLearning.Services.Images
{
    public class ImagePageFetcher : IPageFetcher<Image>
    {
        public ImagePageFetcher(
            ServiceContext serviceContext,
            string tag = null,
            string imageName = null,
            string modelId = null,
            string modelName = null)
        {
            Throw.IfNull(serviceContext, nameof(serviceContext));
            ServiceContext = serviceContext;

            Tag = tag;
            ImageName = imageName;
            ModelId = modelId;
            ModelName = modelName;
        }

        public bool OnLastPage { get; private set; }

        private ServiceContext ServiceContext { get; set; }

        private string NextLink { get; set; }

        private string Tag { get; set; }

        private string ImageName { get; set; }

        private string ModelId { get; set; }

        private string ModelName { get; set; }

        public IEnumerable<Image> FetchNextPage()
        {
            return this.FetchNextPageAsync().Result;
        }

        public async Task<IEnumerable<Image>> FetchNextPageAsync(
            Dictionary<string, List<string>> customHeaders = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var result = new List<Image>();

            if (!this.OnLastPage)
            {
                RestClient restClient = new RestClient(this.ServiceContext.Credentials);
                restClient.BaseUri = this.ServiceContext.ModelManagementEndpoint;

                PaginatedImageList imageList = await RestCallWrapper.WrapAsync(
                () => restClient.Image.ListQueryWithHttpMessagesAsync(
                    subscriptionId: ServiceContext.SubscriptionId,
                    resourceGroupName: ServiceContext.ResourceGroupName,
                    workspace: ServiceContext.WorkspaceName,
                    name: ImageName,
                    modelName: ModelName,
                    modelId: ModelId,
                    tag: Tag,
                    customHeaders: customHeaders,
                    cancellationToken: cancellationToken)).ConfigureAwait(false);

                this.NextLink = imageList.NextLink;
                this.OnLastPage = string.IsNullOrEmpty(this.NextLink);

                result.AddRange(imageList.Value.Select(elem => new Image(ServiceContext, elem)));
            }

            return result;
        }
    }
}