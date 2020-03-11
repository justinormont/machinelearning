// <copyright file = "WebserviceOperations.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Azure.MachineLearning.Services.Compute;
using Azure.MachineLearning.Services.GeneratedOld;
using Azure.MachineLearning.Services.Images;
using Azure.MachineLearning.Services.Models;

namespace Azure.MachineLearning.Services.Webservices
{
    public class WebserviceOperations
    {
        public WebserviceOperations(ServiceContext serviceContext)
        {
            Throw.IfNull(serviceContext, nameof(serviceContext));
            this.ServiceContext = serviceContext;
        }

        public ServiceContext ServiceContext { get; private set; }

        public async Task<Webservice> DeployFromImage(
            string name,
            Image image,
            WebserviceDeploymentConfigurationBase deployConfig,
            ComputeTarget computeTarget = null,
            Dictionary<string, List<string>> customHeaders = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            Throw.IfNull(name, nameof(name));
            Throw.IfNull(image, nameof(image));
            Throw.IfNull(deployConfig, nameof(deployConfig));

            if (await CheckServiceNameAsync(name, customHeaders, cancellationToken).ConfigureAwait(false))
            {
                throw new ArgumentException(string.Format("A webservice with name {0} already exists.", name));
            }

            GeneratedOld.Models.ServiceCreateRequest serviceCreateRequest = deployConfig.ToServiceCreateRequest(
                name,
                computeTarget,
                image);

            var restClient = new RestClient(this.ServiceContext.Credentials);
            restClient.BaseUri = this.ServiceContext.ModelManagementEndpoint;

            GeneratedOld.Models.ServiceCreateHeaders createResponse = await RestCallWrapper.WrapAsync(
                () => restClient.Service.CreateWithHttpMessagesAsync(
                    ServiceContext.SubscriptionId,
                    ServiceContext.ResourceGroupName,
                    ServiceContext.WorkspaceName,
                    serviceCreateRequest,
                    customHeaders: customHeaders,
                    cancellationToken: cancellationToken)).ConfigureAwait(false);

            // Create the web service and get the operationId that can be used to check creation status.
            var operationId = createResponse.OperationLocation.Split('/').Last();

            // Get the Webservice with this name.
            var listResponse = await RestCallWrapper.WrapAsync(
                () => restClient.Service.ListQueryWithHttpMessagesAsync(
                    ServiceContext.SubscriptionId,
                    ServiceContext.ResourceGroupName,
                    ServiceContext.WorkspaceName,
                    name: name,
                    customHeaders: customHeaders,
                    cancellationToken: cancellationToken)).ConfigureAwait(false);

            if (listResponse == null)
            {
                throw new InvalidOperationException(
                    string.Format("Webservice {0} was just created but could not be found.", name));
            }

            // Construct the webservice based on the type of the configuration.
            if (deployConfig is AksServiceDeploymentConfiguration)
            {
                if (listResponse.Value.First() is GeneratedOld.Models.AKSServiceResponse newWebServiceResponse)
                {
                    return new AksWebservice(ServiceContext, newWebServiceResponse, operationId);
                }

                throw new InvalidOperationException(
                    string.Format(
                        "Response was an unexpected type {0}.",
                        listResponse.Value.First().GetType()));
            }
            else
            {
                throw new NotImplementedException(
                    string.Format(
                        "Deployment configuration is of type {0}. Currently only AKS deployments are supported.",
                        deployConfig.GetType()));
            }
        }

        public async Task<Webservice> DeployFromModel(
            string name,
            IEnumerable<Azure.MachineLearning.Services.Models.Model> models,
            ContainerImageConfig imageConfig,
            WebserviceDeploymentConfigurationBase deployConfig = null,
            ComputeTarget computeTarget = null,
            Dictionary<string, List<string>> customHeaders = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            Throw.IfNullOrEmpty(name, nameof(name));
            Throw.IfNull(models, nameof(models));
            Throw.IfNull(imageConfig, nameof(imageConfig));

            if (await CheckServiceNameAsync(name, customHeaders, cancellationToken).ConfigureAwait(false))
            {
                throw new InvalidOperationException(string.Format(
                "Webservices must have unique names. A webservice with the name {0} already exists in this resource group.",
                name));
            }

            Image image = await CreateImageAndWaitForCreation(
                name,
                imageConfig,
                models,
                customHeaders,
                cancellationToken).ConfigureAwait(false);

            return await DeployFromImage(
                name,
                image,
                deployConfig,
                computeTarget,
                customHeaders,
                cancellationToken).ConfigureAwait(false);
        }

        public async Task<IEnumerable<Webservice>> ListAsync(
            string tag = null,
            string properties = null,
            string name = null,
            string imageId = null,
            string imageName = null,
            string modelId = null,
            string modelName = null,
            string computeType = null,
            Dictionary<string, List<string>> customHeaders = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var restClient = new RestClient(this.ServiceContext.Credentials);
            restClient.BaseUri = this.ServiceContext.ModelManagementEndpoint;

            IPageFetcher<GeneratedOld.Models.ServiceResponseBase> fetcher = this.GetPagedList(
                tag: tag,
                properties: properties,
                name: name,
                imageId: imageId,
                imageName: imageName,
                modelId: modelId,
                modelName: modelName,
                computeType: computeType);

            List<Webservice> webservices = new List<Webservice>();

            while (!fetcher.OnLastPage)
            {
                foreach (var response in await fetcher.FetchNextPageAsync(customHeaders, cancellationToken).ConfigureAwait(false))
                {
                    if (response is GeneratedOld.Models.AKSServiceResponse aksResponse)
                    {
                        webservices.Add(new AksWebservice(ServiceContext, aksResponse));
                    }
                    else
                    {
                        Console.WriteLine(
                            "Found a webservice of type {0}. Currently, only AKS Services can be fully populated by the C# SDK. Service {1} will be may be missing some fields in the result.",
                            response.GetType(),
                            response.Id);

                        webservices.Add(new DefaultWebservice(ServiceContext, response));
                    }
                }
            }

            return webservices;
        }

        public async Task DeleteAsync(
            Webservice webService,
            Dictionary<string, List<string>> customHeaders = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            Throw.IfNull(webService, nameof(webService));

            var restClient = new RestClient(this.ServiceContext.Credentials);
            restClient.BaseUri = this.ServiceContext.ModelManagementEndpoint;

            await RestCallWrapper.WrapAsync(
                () => restClient.Service.DeleteWithHttpMessagesAsync(
                    ServiceContext.SubscriptionId,
                    ServiceContext.ResourceGroupName,
                    ServiceContext.WorkspaceName,
                    webService.ServiceId,
                    customHeaders: customHeaders,
                    cancellationToken: cancellationToken)).ConfigureAwait(false);
        }

        private async Task<Image> CreateImageAndWaitForCreation(
            string name,
            ContainerImageConfig imageConfig,
            IEnumerable<Model> models,
            Dictionary<string, List<string>> customHeaders,
            CancellationToken cancellationToken)
        {
            var imageOperations = new Images.ImageOperations(ServiceContext);
            Image newImage = await imageOperations.CreateAsync(
                name,
                imageConfig,
                models,
                customHeaders,
                cancellationToken).ConfigureAwait(false);

            // Call the REST client again to poll for completion.
            await newImage.WaitForCreation(cancellationToken: cancellationToken).ConfigureAwait(false);

            return newImage;
        }

        private async Task<bool> CheckServiceNameAsync(
            string name,
            Dictionary<string, List<string>> customHeaders,
            CancellationToken cancellationToken)
        {
            var restClient = new RestClient(this.ServiceContext.Credentials);
            restClient.BaseUri = this.ServiceContext.ModelManagementEndpoint;

            GeneratedOld.Models.PaginatedServiceList listResult = await RestCallWrapper.WrapAsync(
                () => restClient.Service.ListQueryWithHttpMessagesAsync(
                    ServiceContext.SubscriptionId,
                    ServiceContext.ResourceGroupName,
                    ServiceContext.WorkspaceName,
                    name: name,
                    customHeaders: customHeaders,
                    cancellationToken: cancellationToken)).ConfigureAwait(false);

            return listResult.Value.Count != 0;
        }

        private IPageFetcher<GeneratedOld.Models.ServiceResponseBase> GetPagedList(
            string tag = null,
            string properties = null,
            string name = null,
            string imageId = null,
            string imageName = null,
            string modelId = null,
            string modelName = null,
            string computeType = null)
        {
            var fetcher = new WebservicePageFetcher(
                serviceContext: ServiceContext,
                tag: tag,
                properties: properties,
                name: name,
                imageId: imageId,
                imageName: imageName,
                modelId: modelId,
                modelName: modelName,
                computeType: computeType);

            return fetcher;
        }
    }
}