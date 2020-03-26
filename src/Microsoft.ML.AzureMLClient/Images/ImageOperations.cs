// <copyright file = "ImageOperations.cs" company="Microsoft">
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
    public class ImageOperations
    {
        public ImageOperations(ServiceContext serviceContext)
        {
            Throw.IfNull(serviceContext, nameof(serviceContext));
            this.ServiceContext = serviceContext;
        }

        public ServiceContext ServiceContext { get; private set; }

        public async Task<Image> CreateAsync(
            string name,
            ContainerImageConfig imageConfig,
            IEnumerable<Azure.MachineLearning.Services.Models.Model> models,
            Dictionary<string, List<string>> customHeaders = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            Throw.IfNullOrEmpty(name, nameof(name));
            Throw.IfNull(imageConfig, nameof(imageConfig));
            Throw.IfNull(models, nameof(models));

            DockerImageRequest creationRequest = await imageConfig.ToDockerImageRequestAsync(
                this.ServiceContext,
                name,
                models,
                customHeaders,
                cancellationToken).ConfigureAwait(false);

            var restClient = new RestClient(this.ServiceContext.Credentials);
            restClient.BaseUri = this.ServiceContext.ModelManagementEndpoint;

            ImageCreateHeaders createResponse = await RestCallWrapper.WrapAsync(
                () => restClient.Image.CreateWithHttpMessagesAsync(
                    this.ServiceContext.SubscriptionId,
                    this.ServiceContext.ResourceGroupName,
                    this.ServiceContext.WorkspaceName,
                    creationRequest,
                    customHeaders: customHeaders,
                    cancellationToken: cancellationToken)).ConfigureAwait(false);

            string operationId = createResponse.OperationLocation.Split('/').Last();

            // Get the image that we just created. Names are unique, so there will
            // only be one image returned.
            PaginatedImageList listResponse = await RestCallWrapper.WrapAsync(
                () => restClient.Image.ListQueryWithHttpMessagesAsync(
                    this.ServiceContext.SubscriptionId,
                    this.ServiceContext.ResourceGroupName,
                    this.ServiceContext.WorkspaceName,
                    name: creationRequest.Name,
                    customHeaders: customHeaders,
                    cancellationToken: cancellationToken)).ConfigureAwait(false);

            ImageResponseBase createdImageResponse = listResponse.Value.First();
            return new Image(ServiceContext, createdImageResponse, operationId);
        }

        public IEnumerable<Image> List(
            string imageName = null,
            string modelName = null,
            string modelId = null,
            string tag = null,
            Dictionary<string, List<string>> customHeaders = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var restClient = new RestClient(this.ServiceContext.Credentials);
            restClient.BaseUri = this.ServiceContext.ModelManagementEndpoint;

            IPageFetcher<Image> pageFetcher = GetPagedList(
                imageName: imageName,
                modelName: modelName,
                modelId: modelId,
                tag: tag);

            return new LazyEnumerator<Image>
            {
                Fetcher = pageFetcher,
            };
        }

        public IPageFetcher<Image> GetPagedList(
            string imageName = null,
            string modelId = null,
            string modelName = null,
            string tag = null)
        {
            var fetcher = new ImagePageFetcher(
                serviceContext: ServiceContext,
                tag: tag,
                imageName: imageName,
                modelId: modelId,
                modelName: modelName);

            return fetcher;
        }
    }
}