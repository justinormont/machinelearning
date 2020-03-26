// <copyright file="ModelPageFetcher.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Azure.MachineLearning.Services.GeneratedOld;

namespace Azure.MachineLearning.Services.Models
{
    public class ModelPageFetcher : IPageFetcher<Model>
    {
        public ModelPageFetcher(
            ServiceContext serviceContext,
            FactoryManager<IModelFactory> factories,
            string name = default(string),
            string tag = default(string),
            string version = default(string),
            string orderby = default(string))
        {
            Throw.IfNull(serviceContext, nameof(serviceContext));
            Throw.IfNull(factories, nameof(factories));

            this.ServiceContext = serviceContext;
            this.OnLastPage = false;

            this.Factories = factories;
            this.Name = name;
            this.Tag = tag;
            this.Version = version;
            this.OrderBy = orderby;
        }

        public ServiceContext ServiceContext { get; private set; }

        public FactoryManager<IModelFactory> Factories { get; private set; }

        public string Name { get; private set; }

        public string Tag { get; private set; }

        public string Version { get; private set; }

        public string OrderBy { get; private set; }

        public string ContinuationToken { get; private set; }

        public bool OnLastPage { get; private set; }

        public IEnumerable<Model> FetchNextPage()
        {
            return this.FetchNextPageAsync().Result;
        }

        public async Task<IEnumerable<Model>> FetchNextPageAsync(
            Dictionary<string, List<string>> customHeaders = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            List<Model> result = new List<Model>();

            if (!this.OnLastPage)
            {
                var restClient = new RestClient(this.ServiceContext.Credentials);
                restClient.BaseUri = this.ServiceContext.ModelManagementEndpoint;

                GeneratedOld.Models.PaginatedModelList response = await RestCallWrapper.WrapAsync(
                    () => restClient.Model.ListQueryWithHttpMessagesAsync(
                        this.ServiceContext.SubscriptionId,
                        this.ServiceContext.ResourceGroupName,
                        this.ServiceContext.WorkspaceName,
                        this.Name,
                        this.Tag,
                        this.Version,
                        orderBy: this.OrderBy,
                        skipToken: this.ContinuationToken,
                        customHeaders: customHeaders,
                        cancellationToken: cancellationToken)).ConfigureAwait(false);

                if (response.NextLink != null)
                {
                    this.ContinuationToken = StaticHelpers.ExtractSingleQueryParamFromUrl(response.NextLink, "$skipToken");
                }
                else
                {
                    this.ContinuationToken = null;
                }
                this.OnLastPage = string.IsNullOrEmpty(this.ContinuationToken);

                foreach (var modelData in response.Value)
                {
                    IModelFactory factory = this.Factories.GetFactory(modelData.MimeType);
                    result.Add(factory.Create(this.ServiceContext, modelData));
                }
            }
            return result;
        }
    }
}
