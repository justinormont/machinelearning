// <copyright file = "ExperimentOperations.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Azure.MachineLearning.Services.GeneratedOld;
using Azure.MachineLearning.Services.GeneratedOld.Models;

namespace Azure.MachineLearning.Services.Experiments
{
    public class ExperimentOperations
    {
        public ExperimentOperations(ServiceContext serviceContext)
        {
            Throw.IfNull(serviceContext, nameof(serviceContext));
            this.ServiceContext = serviceContext;
        }

        public ServiceContext ServiceContext { get; private set; }

        public async Task<Experiment> CreateIfNotExistAsync(
            string name,
            Dictionary<string, List<string>> customHeaders = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            Throw.IfNullOrEmpty(name, nameof(name));

            var restClient = new RestClient(this.ServiceContext.Credentials);
            restClient.BaseUri = this.ServiceContext.ExperimentationEndpoint;

            // At the present time, the 'Create Experiment' controller is actually
            // 'CreateIfNotExist'
            ExperimentDto response = await RestCallWrapper.WrapAsync(
                () => restClient.Experiment.CreateWithHttpMessagesAsync(
                    this.ServiceContext.SubscriptionId,
                    this.ServiceContext.ResourceGroupName,
                    this.ServiceContext.WorkspaceName,
                    name,
                    customHeaders: customHeaders,
                    cancellationToken: cancellationToken)).ConfigureAwait(false);
            return new Experiment(this.ServiceContext, response);
        }

        public Experiment CreateUncommitted(string name)
        {
            var dto = new ExperimentDto();
            dto.Name = name;
            var res = new Experiment(this.ServiceContext, dto);

            return res;
        }

        public async Task<Experiment> GetAsync(
            string name,
            Dictionary<string, List<string>> customHeaders = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            Throw.IfNullOrEmpty(name, nameof(name));

            var restClient = new RestClient(this.ServiceContext.Credentials);
            restClient.BaseUri = this.ServiceContext.ExperimentationEndpoint;

            ExperimentDto response = await RestCallWrapper.WrapAsync(()
                => restClient.Experiment.GetWithHttpMessagesAsync(
                    this.ServiceContext.SubscriptionId,
                    this.ServiceContext.ResourceGroupName,
                    this.ServiceContext.WorkspaceName,
                    name,
                    customHeaders: customHeaders,
                    cancellationToken: cancellationToken)).ConfigureAwait(false);

            return new Experiment(this.ServiceContext, response);
        }

        public IPageFetcher<Experiment> GetPagedList(
            string filter = default(string),
            IList<string> orderby = default(IList<string>),
            string sortorder = default(string),
            int? top = default(int?))
        {
            var fetcher = new ExperimentPageFetcher(this.ServiceContext, filter, orderby, sortorder, top);
            return fetcher;
        }

        public IEnumerable<Experiment> List(
            string filter = default(string),
            IList<string> orderby = default(IList<string>),
            string sortorder = default(string),
            int? top = default(int?))
        {
            var lister = new LazyEnumerator<Experiment>();
            lister.Fetcher = this.GetPagedList(filter, orderby, sortorder, top);

            return lister;
        }
    }
}
