// <copyright file="WorkspaceFetcher.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Azure.MachineLearning.Services.Workspaces
{
    public class WorkspaceFetcher : ArmDataFetcher,
        IPageFetcher<Workspace>
    {
        public WorkspaceFetcher(WorkspaceClient client, Guid subscriptionId)
            : base(client, subscriptionId)
        {
        }

        public new IEnumerable<Workspace> FetchNextPage()
        {
            return this.FetchNextPageAsync().Result;
        }

        public new async Task<IEnumerable<Workspace>> FetchNextPageAsync(
            Dictionary<string, List<string>> customHeaders = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            List<Workspace> result = null;
            IEnumerable<ArmData> armData = await base.FetchNextPageAsync(customHeaders, cancellationToken).ConfigureAwait(false);

            if (armData != null)
            {
                result = new List<Workspace>();
                foreach (ArmData workspace in armData)
                {
                    try
                    {
                        result.Add(await this.Client.Workspaces.GetAsync(workspace, customHeaders, cancellationToken).ConfigureAwait(false));
                    }
                    catch (MachineLearningServiceException serviceCallException)
                    {
                        if (serviceCallException.Response.StatusCode == HttpStatusCode.NotFound)
                        {
                            // There is an inherent race condition since a workspace could exist when the
                            // ArmData is fetched, but no longer available if there has been a deleted request
                            // issued in that time.
                            continue;
                        }
                        throw;
                    }
                }
            }

            return result;
        }
    }
}
