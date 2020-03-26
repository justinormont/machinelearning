// <copyright file = "ModelOperations.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Azure.MachineLearning.Services.Assets;
using Azure.MachineLearning.Services.GeneratedOld;

namespace Azure.MachineLearning.Services.Models
{
    public class ModelOperations
    {
        public ModelOperations(ServiceContext serviceContext)
        {
            this.ServiceContext = serviceContext;
            this.Factories = new FactoryManager<IModelFactory>();
            this.Factories.DefaultFactory = new DefaultModelFactory();
        }

        public ServiceContext ServiceContext { get; private set; }

        public FactoryManager<IModelFactory> Factories { get; set; }

        public IPageFetcher<Model> GetPagedList(
            string name = default(string),
            string tag = default(string),
            string version = default(string),
            string orderBy = default(string))
        {
            var fetcher = new ModelPageFetcher(this.ServiceContext, this.Factories, name, tag, version, orderBy);
            return fetcher;
        }

        public IEnumerable<Model> List(
            string name = default(string),
            string tag = default(string),
            string version = default(string),
            string orderBy = default(string))
        {
            var lister = new LazyEnumerator<Model>();
            lister.Fetcher = this.GetPagedList(name, tag, version, orderBy);

            return lister;
        }

        public async Task<Model> GetLatestAsync(
            string modelName,
            Dictionary<string, List<string>> customHeaders = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            Throw.IfNullOrEmpty(modelName, nameof(modelName));
            const string orderByCreatedDesc = "CreatedAtDesc";

            IPageFetcher<Model> modelList = this.GetPagedList(modelName, orderBy: orderByCreatedDesc);

            var firstPage = await modelList.FetchNextPageAsync(customHeaders, cancellationToken).ConfigureAwait(false);
            return firstPage.FirstOrDefault();
        }

        public async Task<Model> RegisterFromLocalFileAsync(
            string modelName,
            FileInfo modelPath,
            Dictionary<string, List<string>> customHeaders = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            Throw.IfNullOrEmpty(modelName, nameof(modelName));
            Throw.IfFileNotExists(modelPath);

            var origin = "LocalUpload";
            string container = BuildContainerName();

            // The asset is internally associated with this artifact based on the
            // container name and model name. The Artifact URI is not relevant since MMS
            // fetches based on prefix.
            var artifactClient = new Artifacts.ArtifactOperations(ServiceContext);
            await artifactClient.UploadDependencyAsync(
                modelPath,
                name: modelName,
                container: container,
                customHeaders: customHeaders,
                cancellationToken: cancellationToken).ConfigureAwait(false);

            Asset asset = await RegisterModelAssetAsync(
                origin,
                container,
                modelName,
                customHeaders,
                cancellationToken).ConfigureAwait(false);

            var modelRequest = new ModelRegistrationRequest
            {
                Name = modelName,
                Unpack = false,
                MimeType = "application/json",
            };
            modelRequest.SetUrlWithAssetId(asset.Id);

            // This is a problem if the workspace has extra model factories registered in the ModelOperations
            // object dangling off the Workspace. We don't have a way of accessing those here, so the
            // user will only have the DefaultModelFactory available at this point in the code
            // If they need another factory, they would need to re-get the Model from
            var modelOperations = new ModelOperations(ServiceContext);
            Model model = await modelOperations.RegisterAsync(modelRequest, customHeaders, cancellationToken).ConfigureAwait(false);
            return model;
        }

        public async Task<Model> RegisterAsync(
            ModelRegistrationRequest registrationRequest,
            Dictionary<string, List<string>> customHeaders = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            Throw.IfNull(registrationRequest, nameof(registrationRequest));

            var dto = registrationRequest.ToDto();

            var restClient = new GeneratedOld.RestClient(this.ServiceContext.Credentials);
            restClient.BaseUri = this.ServiceContext.ModelManagementEndpoint;

            var response = await RestCallWrapper.WrapAsync(
                () => restClient.Model.RegisterWithHttpMessagesAsync(
                    this.ServiceContext.SubscriptionId,
                    this.ServiceContext.ResourceGroupName,
                    this.ServiceContext.WorkspaceName,
                    dto,
                    customHeaders: customHeaders,
                    cancellationToken: cancellationToken)).ConfigureAwait(false);

            return this.Factories.GetFactory(response.MimeType).Create(this.ServiceContext, response);
        }

        private AssetCreationRequest BuildAssetCreationRequest(
            string origin,
            string container,
            string modelName)
        {
            var assetDict = new Dictionary<string, string>
            {
                { "prefix", string.Format("{0}/{1}/{2}", origin, container, modelName) }
            };

            return new AssetCreationRequest
            {
                Name = modelName,
                Artifacts = new List<IDictionary<string, string>> { assetDict },
                Created = DateTime.UtcNow,
                Description = string.Format("{0} uploaded from loca file.", modelName),
            };
        }

        private string BuildContainerName()
        {
            var time = DateTime.Now.ToString("yyyyMMddHmmss");
            var uuid = Guid.NewGuid().ToString("N").Substring(0, 8);
            return string.Format("{0}-{1}", time, uuid);
        }

        private async Task<Asset> RegisterModelAssetAsync(
            string origin,
            string container,
            string modelName,
            Dictionary<string, List<string>> customHeaders,
            CancellationToken cancellationToken)
        {
            var assetClient = new Assets.AssetOperations(ServiceContext);
            AssetCreationRequest assetUploadRequest =
                BuildAssetCreationRequest(origin, container, modelName);

            return await assetClient.CreateAsync(
                assetUploadRequest,
                customHeaders,
                cancellationToken).ConfigureAwait(false);
        }
    }
}
