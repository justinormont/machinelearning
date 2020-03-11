// <copyright file="AzureMLOperations.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Azure.MachineLearning.Services.Workspaces;
using Microsoft.Azure.Management.ResourceManager.Fluent.Core;
using Microsoft.Rest;

namespace Microsoft.ML.CLI.CodeGenerator
{
    public static class AzureMLOperations
    {
        public static IEnumerable<ArmData> ListWorkspaces(ServiceClientCredentials credentials, Guid subscriptionId)
        {
            var amlClient = new WorkspaceClient(credentials);

            var workspaceFetcher = amlClient.Workspaces.List(subscriptionId);

            return workspaceFetcher;
        }

        public static async Task<Workspace> GetSpecificWorkspaceAsync(ServiceClientCredentials credentials, Guid subscriptionId, string resourceGroupName, string workspaceName)
        {
            var amlClient = new WorkspaceClient(credentials);

            return await amlClient.Workspaces.GetAsync(subscriptionId, resourceGroupName, workspaceName).ConfigureAwait(false);
        }

        public static async Task CreateExperimentIfNotExistAsync(Workspace ws, string expName)
        {
            var exp = await ws.Experiments.CreateIfNotExistAsync(expName).ConfigureAwait(false);

            Console.WriteLine("Experiment: {0}", exp.Name);
        }
    }
}
