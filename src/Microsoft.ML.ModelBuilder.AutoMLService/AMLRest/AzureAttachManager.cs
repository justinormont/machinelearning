// <copyright file="AzureAttachManager.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Azure.MachineLearning.Services.Workspaces;
using Microsoft.Azure.Management.ResourceManager.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent.Authentication;
using Microsoft.ML.ModelBuilder.AutoMLService;
using Microsoft.ML.ModelBuilder.AutoMLService.Contract.AzureResourceObjects;
using Microsoft.Rest;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Azure.MachineLearning.Services.AMLRest
{
    public class AzureAttachManager
    {
        private const string GET = "GET";
        private const string APIVersion = "?api-version=2019-06-01";
        private const string SUBSCRIPTIONS = "/subscriptions";

        public static async Task<List<SubscriptionInformation>> RequestSubscriptionsAsync(string token)
        {
            var url = ConcatURL(SUBSCRIPTIONS);
            var jsonGetResult = await AzureWebRequest.GetResponseAsync(url, token, GET);
            var jsonResult = ExtractValueFromJson(jsonGetResult);

            var listOfSubs = JsonConvert.DeserializeObject<List<SubscriptionInformation>>(jsonResult);
            return listOfSubs;
        }

        public static async Task<List<WorkspaceInformation>> RequestAzureWorkspaceAsync(SubscriptionInformation selectedSubscription, string token)
        {
            var childResources = string.Concat(selectedSubscription.Id, "/providers/Microsoft.MachineLearningServices/workspaces");
            var url = ConcatURL(childResources);
            var jsonGetResult = await AzureWebRequest.GetResponseAsync(url, token, GET);
            var jsonResult = ExtractValueFromJson(jsonGetResult);

            return JsonConvert.DeserializeObject<List<WorkspaceInformation>>(jsonResult);
        }

        public static async Task<List<ComputeResource>> RequestAzureMLComputeAsync(AzureResource mlWorkspace, string token)
        {
            var childResources = string.Concat(mlWorkspace.Id, "/computes");
            var url = ConcatURL(childResources);
            var jsonGetResult = await AzureWebRequest.GetResponseAsync(url, token, GET);
            var jsonResult = ExtractValueFromJson(jsonGetResult);

            return JsonConvert.DeserializeObject<List<ComputeResource>>(jsonResult);
        }

        public static async Task<List<Datasource>> RequestDataStoresAsync(AzureResource mlWorkspace, string token)
        {
            var resourceId = mlWorkspace.Id + "/datastores";

            var urlBase = "https://{0}.experiments.azureml.net/datastore/v1.0{1}";
            var url = string.Format(urlBase, mlWorkspace.Location, resourceId);

            var jsonGetResult = await AzureWebRequest.GetResponseAsync(url, token, GET);
            var jsonResult = ExtractValueFromJson(jsonGetResult);

            return JsonConvert.DeserializeObject<List<Datasource>>(jsonResult);
        }

        /// <summary>
        /// Create an experiment on the chosen workspace and input name. If experiment already exists
        /// nothing will happen.
        /// </summary>
        /// <param name="selectedWorkspace">Workspace to create the experiment.</param>
        /// <param name="experimentName">Naem for experiment.</param>
        /// <returns>Async method.</returns>
        public static async Task PostCreateExperimentAsync(AzureResource selectedWorkspace, string experimentName, string token)
        {
            var urlBase = "https://{0}.experiments.azureml.net/history/v1.0"; // {0} is workspace location
            var workspaceResourceUrl = selectedWorkspace.Id;
            var experimentsUrl = "/experiments/{0}"; // {0} is experiment name

            urlBase = string.Format(urlBase, selectedWorkspace.Location);
            experimentsUrl = string.Format(experimentsUrl, experimentName);

            string url = string.Concat(urlBase, workspaceResourceUrl, experimentsUrl);
            await AzureWebRequest.GetResponseAsync(url, token, "POST");
        }

        public static async Task<List<ResourceGroup>> RequestResourceGroupsAsync(SubscriptionInformation selectedSubscription, string token)
        {
            var childResources = string.Concat(selectedSubscription.Id, "/resourcegroups");
            var url = ConcatURL(childResources);
            var jsonGetResult = await AzureWebRequest.GetResponseAsync(url, token, GET);
            var jsonResult = ExtractValueFromJson(jsonGetResult);

            return JsonConvert.DeserializeObject<List<ResourceGroup>>(jsonResult);
        }

        public static async Task<List<AzureRegion>> RequestRegionsAsync(SubscriptionInformation selectedSubscription, string token)
        {
            var childResources = string.Concat(selectedSubscription.Id, "/locations");
            var url = ConcatURL(childResources);

            var jsonGetResult = await AzureWebRequest.GetResponseAsync(url, token, GET);
            var jsonResult = ExtractValueFromJson(jsonGetResult);

            return JsonConvert.DeserializeObject<List<AzureRegion>>(jsonResult);
        }

        public static async Task<WorkspaceInformation> PutCreateWorkspaceAsync(SubscriptionInformation selectedSubscription, string selectedRegion, ResourceGroup selectedGroup, string newResourceGroupName, string workspaceName, string token)
        {
            token = token.Replace("Bearer ", string.Empty);
            var tokenCredentials = new TokenCredentials(token);

            var serviceClientCredentials = new AzureCredentials(
                tokenCredentials,
                tokenCredentials,
                null,  // TODO: provide a way to specify TenantId?
                AzureEnvironment.AzureGlobalCloud);

            var azuremlClient = new WorkspaceClient(serviceClientCredentials);
            var region = Microsoft.Azure.Management.ResourceManager.Fluent.Core.Region.Create(selectedRegion);

            Workspace workspace;

            if (selectedGroup != null)
            {
                workspace = await azuremlClient.Workspaces.CreateAsync(region, workspaceName, new Guid(selectedSubscription.SubscriptionId), selectedGroup.Name, workspaceName, createResourceGroup: false, isBasicSku: false);
            }
            else if (!string.IsNullOrEmpty(newResourceGroupName))
            {
                workspace = await azuremlClient.Workspaces.CreateAsync(region, workspaceName, new Guid(selectedSubscription.SubscriptionId), newResourceGroupName, workspaceName, createResourceGroup: true, isBasicSku: false);
            }
            else
            {
                throw new ArgumentException("Must provide resource group or new resource group name");
            }

            return new WorkspaceInformation
            {
                Name = workspace.Name,
                Id = workspace.Id,
                Type = workspace.Type,
            };
        }

        public static async Task<AzureResource> PutCreateComputeAsync(WorkspaceInformation selectedWorksapce, string computeType, string computeName, string token)
        {
            var root = "https://management.azure.com";
            var childResources = string.Concat(selectedWorksapce.Id, "/computes/", computeName);
            var url = string.Concat(root, childResources, APIVersion);

            var body = GetCreateComputeJsonBody(selectedWorksapce.Location, computeType);

            var jsonResult = await AzureWebRequest.GetResponseAsync(url, token, "PUT", null, body);

            return JsonConvert.DeserializeObject<AzureResource>(jsonResult);
        }

        public static async Task<List<ComputeTypeResource>> GetAvailableComputesForRegionAsync(SubscriptionInformation subscription, string location, string token)
        {
            var childResources = string.Concat(subscription.Id, "/providers/Microsoft.MachineLearningServices/locations/", location, "/vmSizes");
            var url = string.Concat("https://management.azure.com", childResources, APIVersion);

            var jsonResult = await AzureWebRequest.GetResponseAsync(url, token, GET);

            JObject jObject = JObject.Parse(jsonResult);
            string jsonResultObject = jObject["amlCompute"].ToString(Newtonsoft.Json.Formatting.None);

            return JsonConvert.DeserializeObject<List<ComputeTypeResource>>(jsonResultObject);
        }

        public static async Task<ComputeResource> GetComputeResourceAsync(string computeId, string token)
        {
            var url = string.Concat("https://management.azure.com", computeId, APIVersion);

            var jsonResult = await AzureWebRequest.GetResponseAsync(url, token, GET);

            return JsonConvert.DeserializeObject<ComputeResource>(jsonResult);
        }

        private static string ConcatURL(string childResources)
        {
            // Ids provided in data objects will start with '/', so our root url doesn't need to end with a slash.
            const string root = "https://management.azure.com";
            return string.Concat(root, childResources, APIVersion);
        }

        private static string ExtractValueFromJson(string jsonResult)
        {
            // All json GET comes back wrapped in "value":{}. Remove that
            // so the true object can be deserialized by callers
            JObject jObject = JObject.Parse(jsonResult);
            string jsonResultObject = jObject["value"].ToString(Newtonsoft.Json.Formatting.None);

            return jsonResultObject;
        }

        private static string GetCreateComputeJsonBody(string location, string vmSize)
        {
            var locationJson = $"{{\"location\": \"{location}\",";
            var propertiesJson = @"""properties"": {
                    ""computeType"": ""AmlCompute"",";
            var vmJson = $"\"properties\": {{\"vmSize\": \"{vmSize}\",";
            var scaleSettings =
                    @"""vmPriority"": ""Dedicated"",
                    ""scaleSettings"": {
                        ""maxNodeCount"": 1,
                        ""minNodeCount"": 0,
                        ""nodeIdleTimeBeforeScaleDown"": ""PT5M""
                        }
                    }
                }
            }";

            return string.Concat(locationJson, propertiesJson, vmJson, scaleSettings);
        }
    }
}
