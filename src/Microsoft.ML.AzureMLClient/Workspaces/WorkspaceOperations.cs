// <copyright file = "WorkspaceOperations.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Azure.MachineLearning.Services.GeneratedOld.Models;
using Azure.MachineLearning.Services.Management;

using Microsoft.Azure.Management.ResourceManager.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent.Authentication;
using Microsoft.Azure.Management.ResourceManager.Fluent.Core;
using Microsoft.Azure.Management.ResourceManager.Fluent.Models;
using Microsoft.Rest;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Azure.MachineLearning.Services.Workspaces
{
    public class WorkspaceOperations
    {
        private const string AppInsightsApiVersion = "2018-09-01";
        private const string AzureContainerRegistryApiVersion = "2018-09-01";
        private const string KeyVaultApiVersion = "2018-05-01";
        private const string StorageAccountApiVersion = "2018-11-01";

        public WorkspaceOperations(WorkspaceClient workspaceClient)
        {
            this.WorkspaceClient = workspaceClient;
        }

        public WorkspaceClient WorkspaceClient { get; private set; }

        public async Task<bool> DeleteWorkspaceAsync(
            Workspace workspace,
            bool deleteDependentResources = false,
            Dictionary<string, List<string>> customHeaders = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            Throw.IfNull(workspace, nameof(workspace));

            AzureCredentials azureCredentials = new AzureCredentials(
                this.WorkspaceClient.Credentials,
                this.WorkspaceClient.Credentials,
                this.WorkspaceClient.TenantId,
                AzureEnvironment.AzureGlobalCloud);

            var azure = Microsoft.Azure.Management.Fluent.Azure
                .Configure()
                .WithLogLevel(HttpLoggingDelegatingHandler.Level.Basic)
                .Authenticate(azureCredentials)
                .WithSubscription(workspace.Context.SubscriptionId.ToString());

            if (deleteDependentResources)
            {
                await DeleteAzureResourceAsync(workspace.AppInsightsArmId, AppInsightsApiVersion, customHeaders, cancellationToken).ConfigureAwait(false);
                await DeleteAzureResourceAsync(workspace.AcrArmId, AzureContainerRegistryApiVersion, customHeaders, cancellationToken).ConfigureAwait(false);
                await DeleteAzureResourceAsync(workspace.KeyVaultArmId, KeyVaultApiVersion, customHeaders, cancellationToken).ConfigureAwait(false);
                await DeleteAzureResourceAsync(workspace.StorageAccountArmId, StorageAccountApiVersion, customHeaders, cancellationToken).ConfigureAwait(false);
            }

            await DeleteAzureResourceAsync(workspace.Id, this.WorkspaceClient.ApiVersion, customHeaders, cancellationToken).ConfigureAwait(false);

            return true;
        }

        // Exceptions creating workspaces will propogate directly to the user
        public async Task<Workspace> CreateAsync(
            Region region,
            string workspaceName,
            Guid subscriptionId,
            string resourceGroupName,
            string friendlyName = null,
            bool createResourceGroup = true,
            bool isBasicSku = true,
            string storageAccountName = null,
            string keyVaultName = null,
            string applicationInsightsName = null,
            string azureContainerRegistryName = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            Throw.IfNullOrEmpty(resourceGroupName, nameof(resourceGroupName));

            if (friendlyName == null)
            {
                friendlyName = workspaceName;
            }

            if (string.IsNullOrEmpty(storageAccountName))
            {
                storageAccountName = WorkspaceManagementHelpers.GenerateNameForDependentResource(workspaceName, WorkspaceManagementHelpers.DependentResourceType.StorageAccount);
            }

            if (string.IsNullOrEmpty(keyVaultName))
            {
                keyVaultName = WorkspaceManagementHelpers.GenerateNameForDependentResource(workspaceName, WorkspaceManagementHelpers.DependentResourceType.KeyVault);
            }

            if (string.IsNullOrEmpty(applicationInsightsName))
            {
                applicationInsightsName = WorkspaceManagementHelpers.GenerateNameForDependentResource(workspaceName, WorkspaceManagementHelpers.DependentResourceType.ApplicationInsights);
            }

            if (string.IsNullOrEmpty(azureContainerRegistryName))
            {
                azureContainerRegistryName = WorkspaceManagementHelpers.GenerateNameForDependentResource(workspaceName, WorkspaceManagementHelpers.DependentResourceType.ContainerRegistry);
            }

            if (!WorkspaceManagementHelpers.ValidateApplicationInsightName(applicationInsightsName))
            {
                throw new ArgumentException(string.Format("Application Insights name name is invalid, does not conform to Azure naming rules.  {0}", applicationInsightsName));
            }

            if (!WorkspaceManagementHelpers.ValidateAzureDNSName(keyVaultName))
            {
                throw new ArgumentException(string.Format("Key Vault name is invalid, does not conform to Azure naming rules.  {0}", keyVaultName));
            }

            if (!WorkspaceManagementHelpers.ValidateAzureContainerName(azureContainerRegistryName))
            {
                throw new ArgumentException(string.Format("Container Registry name is invalid, does not conform to Azure naming rules.  {0}", azureContainerRegistryName));
            }

            if (!WorkspaceManagementHelpers.ValidateAzureDNSName(storageAccountName))
            {
                throw new ArgumentException(string.Format("Storage Account name is invalid, does not conform to Azure naming rules.  {0}", storageAccountName));
            }

            // For Auth piece, also see:
            // https://github.com/Azure/azure-libraries-for-java/blob/master/AUTH.md

            var template = new ArmTemplateBuilder();
            var workspace_dependencies = new JArray();

            // Build KeyVault
            JObject kvResource = template.BuildKVTemplate(keyVaultName, region, this.WorkspaceClient.TenantId);

            template.AddResource(kvResource);
            workspace_dependencies.Add(string.Format("[resourceId('{0}/{1}', '{2}')]", "Microsoft.KeyVault", "vaults", keyVaultName));

            string kvResourceId = GetArmResourceId(subscriptionId.ToString(), resourceGroupName, "Microsoft.KeyVault/vaults", keyVaultName);

            // Build Storage Account
            JObject storageResource = template.BuildStorageAccountTemplate(storageAccountName, region);
            template.AddResource(storageResource);

            workspace_dependencies.Add(string.Format("[resourceId('{0}/{1}', '{2}')]", "Microsoft.Storage", "storageAccounts", storageAccountName));
            string storageResourceId = GetArmResourceId(subscriptionId.ToString(), resourceGroupName, "Microsoft.Storage/storageAccounts", storageAccountName);

            // Build Azure Container Registry
            JObject acrResource = template.BuildAzureContainerRegistryTemplate(azureContainerRegistryName, region);
            template.AddResource(acrResource);
            workspace_dependencies.Add(string.Format("[resourceId('{0}/{1}', '{2}')]", "Microsoft.ContainerRegistry", "registries", azureContainerRegistryName));
            string acrResourceId = GetArmResourceId(subscriptionId.ToString(), resourceGroupName, "Microsoft.ContainerRegistry/registries", azureContainerRegistryName);

            // Build App Insights Instance
            JObject appInsightsResource = template.BuildApplicationInsightsTemplate(applicationInsightsName, region);
            template.AddResource(appInsightsResource);
            workspace_dependencies.Add(string.Format("[resourceId('{0}/{1}', '{2}')]", "Microsoft.Insights", "components", applicationInsightsName));
            string appInsightsResourceId = GetArmResourceId(subscriptionId.ToString(), resourceGroupName, "Microsoft.Insights/components", applicationInsightsName);

            // Build Workspace
            JObject workspaceResource = template.BuildWorkspaceResource(workspaceName, region, kvResourceId, acrResourceId, storageResourceId, appInsightsResourceId, isBasicSku);
            workspaceResource.GetValue("dependsOn").Replace(workspace_dependencies);

            template.AddResource(workspaceResource);

            // See https://github.com/Azure-Samples/resources-dotnet-deploy-using-arm-template

            var azure = Microsoft.Azure.Management.Fluent.Azure
                .Configure()
                .WithLogLevel(HttpLoggingDelegatingHandler.Level.Basic)
                .Authenticate(new AzureCredentials(this.WorkspaceClient.Credentials, this.WorkspaceClient.Credentials, this.WorkspaceClient.TenantId, AzureEnvironment.AzureGlobalCloud))
                .WithSubscription(subscriptionId.ToString());

            IDeployment deployment;

            if (createResourceGroup)
            {
                deployment = await azure.Deployments.Define(Guid.NewGuid().ToString())
                    .WithNewResourceGroup(resourceGroupName, region)
                    .WithTemplate(template.GetTemplate().ToString())
                    .WithParameters("{}")
                    .WithMode(DeploymentMode.Incremental)
                    .CreateAsync(cancellationToken).ConfigureAwait(false);
            }
            else
            {
                deployment = await azure.Deployments.Define(Guid.NewGuid().ToString())
                    .WithExistingResourceGroup(resourceGroupName)
                    .WithTemplate(template.GetTemplate().ToString())
                    .WithParameters("{}")
                    .WithMode(DeploymentMode.Incremental)
                    .CreateAsync(cancellationToken);
            }

            Workspace workspace = await this.GetAsync(subscriptionId, resourceGroupName, workspaceName).ConfigureAwait(false);
            return workspace;
        }

        public IEnumerable<ArmData> List(Guid subscriptionId, string resourceGroupNameFilter = default(string), string workspaceNameFilter = default(string))
        {
            var lister = new LazyEnumerator<ArmData>();
            lister.Fetcher = this.GetPagedList(subscriptionId, resourceGroupNameFilter, workspaceNameFilter);

            return lister;
        }

        public IPageFetcher<ArmData> GetPagedList(Guid subscriptionId, string resourceGroupNameFilter = default(string), string workspaceNameFilter = default(string))
        {
            var fetcher = new ArmDataFetcher(this.WorkspaceClient, subscriptionId);

            if (resourceGroupNameFilter != default(string))
            {
                throw new NotImplementedException("Resource Group filter not implemented");
            }
            if (workspaceNameFilter != default(string))
            {
                throw new NotImplementedException("Workspace Name filter not implemented");
            }

            return fetcher;
        }

        public IEnumerable<Workspace> ListWorkspaces(Guid subscriptionId, string resourceGroupNameFilter = default(string), string workspaceNameFilter = default(string))
        {
            var lister = new LazyEnumerator<Workspace>();
            lister.Fetcher = this.GetPagedWorkspaceList(subscriptionId, resourceGroupNameFilter, workspaceNameFilter);

            return lister;
        }

        public IPageFetcher<Workspace> GetPagedWorkspaceList(Guid subscriptionId, string resourceGroupNameFilter = default(string), string workspaceNameFilter = default(string))
        {
            var fetcher = new WorkspaceFetcher(this.WorkspaceClient, subscriptionId);

            if (resourceGroupNameFilter != default(string))
            {
                throw new NotImplementedException("Resource Group filter not implemented");
            }
            if (workspaceNameFilter != default(string))
            {
                throw new NotImplementedException("Workspace Name filter not implemented");
            }

            return fetcher;
        }

        public async Task<Workspace> GetAsync(
            Guid subscriptionId,
            string resourceGroupName,
            string workspaceName,
            Dictionary<string, List<string>> customHeaders = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            // https://management.azure.com/subscriptions/{subscriptionId}/resourceGroups/{resourceGroupName}/providers/Microsoft.MachineLearningServices/workspaces/{workspaceName}?api-version=2018-03-01-preview
            Throw.IfNullOrEmpty(resourceGroupName, nameof(resourceGroupName));
            Throw.IfNullOrEmpty(workspaceName, nameof(workspaceName));

            string apiVersion = "2018-11-19";

            // Compute URL
            string baseUri = this.WorkspaceClient.HttpClient.BaseAddress.AbsoluteUri;
            var url = new System.Uri(new System.Uri(baseUri + (baseUri.EndsWith("/") ? string.Empty : "/")), "subscriptions/{subscriptionId}/resourceGroups/{resourceGroupName}/providers/Microsoft.MachineLearningServices/workspaces/{workspace}").ToString();
            url = url.Replace("{subscriptionId}", System.Uri.EscapeDataString(Microsoft.Rest.Serialization.SafeJsonConvert.SerializeObject(subscriptionId, WorkspaceClient.SerializationSettings).Trim('"')));
            url = url.Replace("{resourceGroupName}", System.Uri.EscapeDataString(resourceGroupName));
            url = url.Replace("{workspace}", System.Uri.EscapeDataString(workspaceName));

            // Get the API version string
            var queryParameters = new List<string>();
            if (apiVersion != null)
            {
                queryParameters.Add(string.Format("api-version={0}", System.Uri.EscapeDataString(apiVersion)));
            }

            if (queryParameters.Count > 0)
            {
                url += "?" + string.Join("&", queryParameters);
            }

            // Create HTTP transport objects
            using (var httpRequest = new HttpRequestMessage())
            {
                httpRequest.Method = new HttpMethod("GET");
                httpRequest.RequestUri = new System.Uri(url);

                if (customHeaders != null)
                {
                    foreach (var header in customHeaders)
                    {
                        if (httpRequest.Headers.Contains(header.Key))
                        {
                            httpRequest.Headers.Remove(header.Key);
                        }
                        httpRequest.Headers.TryAddWithoutValidation(header.Key, header.Value);
                    }
                }

                // Set Credentials
                if (this.WorkspaceClient.Credentials != null)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    await this.WorkspaceClient.Credentials.ProcessHttpRequestAsync(httpRequest, cancellationToken).ConfigureAwait(false);
                }

                // Send Request
                using (HttpResponseMessage httpResponse = await this.WorkspaceClient.HttpClient.SendAsync(httpRequest, cancellationToken).ConfigureAwait(false))
                {
                    if (httpResponse.StatusCode != HttpStatusCode.OK)
                    {
                        var msg = string.Format("Got {0} response from service", (int)httpResponse.StatusCode);
                        var e = new MachineLearningServiceException(msg);
                        e.Request = new HttpRequestMessageWrapper(httpRequest, string.Empty);
                        e.Response = new HttpResponseMessageWrapper(httpResponse, string.Empty);

                        throw e;
                    }

                    var result = new HttpOperationResponse<WorkspaceData>();

                    // The deserialisation needs improvement to match what Azure returns
                    result.Request = httpRequest;
                    result.Response = httpResponse;
                    string responseContent = await httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);

                    try
                    {
                        result.Body = Microsoft.Rest.Serialization.SafeJsonConvert.DeserializeObject<WorkspaceData>(responseContent, this.WorkspaceClient.DeserializationSettings);
                    }
                    catch (JsonException ex)
                    {
                        throw new SerializationException("Unable to deserialize the response.", responseContent, ex);
                    }

                    var serviceContext = new ServiceContext();
                    await serviceContext.PopulateAsync(this.WorkspaceClient, result.Body.Properties.DiscoveryUrl, subscriptionId, resourceGroupName, workspaceName, result.Body.Location).ConfigureAwait(false);

                    return new Workspace(result.Body, serviceContext);
                }
            }
        }

        public async Task<Workspace> GetAsync(ArmData armData, Dictionary<string, List<string>> customHeaders = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await this.GetAsync(armData.SubscriptionId, armData.ResourceGroupName, armData.Name, customHeaders, cancellationToken).ConfigureAwait(false);
        }

        private static string GetArmResourceId(string subscriptionId, string regionName, string provider, string resourceName)
        {
            string format = "/subscriptions/{0}/resourceGroups/{1}/providers/{2}/{3}";

            return string.Format(format, subscriptionId, regionName, provider, resourceName);
        }

        private async Task DeleteAzureResourceAsync(string resourceId, string apiVersion, Dictionary<string, List<string>> customHeaders, CancellationToken cancellationToken)
        {
            var restClient = new GeneratedOld.RestClient(this.WorkspaceClient.Credentials);

            Throw.IfNullOrEmpty(resourceId, nameof(resourceId));
            Throw.IfNullOrEmpty(apiVersion, nameof(apiVersion));

            // Tracing
            bool shouldTrace = ServiceClientTracing.IsEnabled;
            string invocationId = null;
            if (shouldTrace)
            {
                invocationId = ServiceClientTracing.NextInvocationId.ToString();
                var tracingParameters = new Dictionary<string, object>();
                tracingParameters.Add("apiVersion", apiVersion);
                tracingParameters.Add("cancellationToken", cancellationToken);
                ServiceClientTracing.Enter(invocationId, this, "Delete", tracingParameters);
            }
            // Construct URL
            string baseUrl = AzureEnvironment.AzureGlobalCloud.ResourceManagerEndpoint;
            var uri = new System.Uri(new System.Uri(baseUrl + (baseUrl.EndsWith("/") ? string.Empty : "/")), resourceId).ToString();
            List<string> queryParameters = new List<string>();
            if (apiVersion != null)
            {
                queryParameters.Add(string.Format("api-version={0}", System.Uri.EscapeDataString(apiVersion)));
            }
            if (queryParameters.Count > 0)
            {
                uri += "?" + string.Join("&", queryParameters);
            }
            // Create HTTP transport objects
            var httpRequest = new HttpRequestMessage();
            HttpResponseMessage httpResponse = null;
            httpRequest.Method = new HttpMethod("DELETE");
            httpRequest.RequestUri = new System.Uri(uri);

            // Set Headers
            if (customHeaders != null)
            {
                foreach (var header in customHeaders)
                {
                    if (httpRequest.Headers.Contains(header.Key))
                    {
                        httpRequest.Headers.Remove(header.Key);
                    }
                    httpRequest.Headers.TryAddWithoutValidation(header.Key, header.Value);
                }
            }

            // Serialize Request
            string requestContent = null;

            cancellationToken.ThrowIfCancellationRequested();
            await this.WorkspaceClient.Credentials.ProcessHttpRequestAsync(httpRequest, cancellationToken).ConfigureAwait(false);

            // Send Request
            if (shouldTrace)
            {
                ServiceClientTracing.SendRequest(invocationId, httpRequest);
            }
            cancellationToken.ThrowIfCancellationRequested();
            httpResponse = await restClient.HttpClient.SendAsync(httpRequest, cancellationToken).ConfigureAwait(false);
            if (shouldTrace)
            {
                ServiceClientTracing.ReceiveResponse(invocationId, httpResponse);
            }
            HttpStatusCode statusCode = httpResponse.StatusCode;
            cancellationToken.ThrowIfCancellationRequested();
            string responseContent = null;
            if ((int)statusCode != 200 && (int)statusCode != 204)
            {
                var ex = new MachineLearningServiceErrorException(string.Format("Operation returned an invalid status code '{0}'", statusCode));
                try
                {
                    responseContent = await httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
                    MachineLearningServiceError errorBody = Microsoft.Rest.Serialization.SafeJsonConvert.DeserializeObject<MachineLearningServiceError>(responseContent, restClient.DeserializationSettings);
                    if (errorBody != null)
                    {
                        ex.Body = errorBody;
                    }
                }
                catch (JsonException)
                {
                    // Ignore the exception
                }
                ex.Request = new HttpRequestMessageWrapper(httpRequest, requestContent);
                ex.Response = new HttpResponseMessageWrapper(httpResponse, responseContent);
                if (shouldTrace)
                {
                    ServiceClientTracing.Error(invocationId, ex);
                }
                httpRequest.Dispose();
                if (httpResponse != null)
                {
                    httpResponse.Dispose();
                }
                throw ex;
            }
            // Create Result
            var result = new HttpOperationResponse();
            result.Request = httpRequest;
            result.Response = httpResponse;
            if (shouldTrace)
            {
                ServiceClientTracing.Exit(invocationId, result);
            }
        }
    }
}