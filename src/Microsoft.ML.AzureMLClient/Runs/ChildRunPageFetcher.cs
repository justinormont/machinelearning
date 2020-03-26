// <copyright file="ChildRunPageFetcher.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Azure.MachineLearning.Services.GeneratedOld;
using Azure.MachineLearning.Services.GeneratedOld.Models;

namespace Azure.MachineLearning.Services.Runs
{
    public class ChildRunPageFetcher : RunPageFetcher
    {
        public ChildRunPageFetcher(
            ServiceContext serviceContext,
            string experimentName,
            FactoryManager<IRunFactory> factories,
            string parentRunId,
            string filter = default(string),
            IList<string> orderby = default(IList<string>),
            string sortorder = default(string),
            int? top = default(int?))
             : base(serviceContext, experimentName, factories, filter, orderby, sortorder, top)
        {
            Throw.IfNullOrEmpty(parentRunId, nameof(parentRunId));
            this.ParentRunId = parentRunId;
        }

        public string ParentRunId { get; private set; }

        protected override async Task<GeneratedOld.Models.PaginatedRunDto> GetNextResultsAsync(
            Dictionary<string, List<string>> customHeaders,
            CancellationToken cancellationToken)
        {
            var restClient = new RestClient(this.ServiceContext.Credentials);
            restClient.BaseUri = this.ServiceContext.ExperimentationEndpoint;

            PaginatedRunDto response = await RestCallWrapper.WrapAsync(
                () => restClient.Run.GetChildWithHttpMessagesAsync(
                    this.ServiceContext.SubscriptionId,
                    this.ServiceContext.ResourceGroupName,
                    this.ServiceContext.WorkspaceName,
                    this.ExperimentName,
                    this.ParentRunId,
                    this.Filter,
                    this.ContinuationToken,
                    this.OrderBy,
                    this.SortOrder,
                    this.Top,
                    customHeaders: customHeaders,
                    cancellationToken: cancellationToken)).ConfigureAwait(false);

            return response;
        }
    }
}
