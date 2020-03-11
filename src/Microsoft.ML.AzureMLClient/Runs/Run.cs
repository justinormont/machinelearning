// <copyright file = "Run.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

using Azure.MachineLearning.Services.Assets;
using Azure.MachineLearning.Services.Metrics;
using Azure.MachineLearning.Services.Models;
using Azure.MachineLearning.Services.RunArtifacts;
using Azure.MachineLearning.Services.Utils;
using Microsoft.Rest;
using Newtonsoft.Json;

namespace Azure.MachineLearning.Services.Runs
{
    public class Run : IRefreshableFromDto<GeneratedOld.Models.RunDto>
    {
        public Run(
            ServiceContext serviceContext,
            string experimentName,
            FactoryManager<IRunFactory> factories,
            GeneratedOld.Models.RunDto runDto)
        {
            Throw.IfNull(serviceContext, nameof(serviceContext));
            Throw.IfNullOrEmpty(experimentName, nameof(experimentName));
            Throw.IfNull(factories, nameof(factories));
            Throw.IfNull(runDto, nameof(runDto));

            this.ServiceContext = serviceContext;
            this.ExperimentName = experimentName;
            this.Factories = factories;
            this.RefreshFromDto(runDto);
            this.RunArtifact = new RunArtifactOperations(serviceContext, this.ExperimentName, this.Id);
        }

        public ServiceContext ServiceContext { get; private set; }

        public string ExperimentName { get; private set; }

        public FactoryManager<IRunFactory> Factories { get; private set; }

        public int? Number { get; private set; }

        public string Id { get; private set; }

        public string ParentId { get; private set; }

        public string RootRunId { get; private set; }

        public string Name { get; private set; }

        public DateTime? CreatedUtc { get; private set; }

        public string UserId { get; private set; }

        public string Token { get; private set; }

        public DateTime? TokenExpiryUtc { get; private set; }

        public DateTime? StartTimeUtc { get; private set; }

        public DateTime? EndTimeUtc { get; private set; }

        public bool? HeartbeatEnabled { get; private set; }

        public string DataContainerId { get; private set; }

        public string Description { get; private set; }

        public bool? Hidden { get; private set; }

        public string Target { get; private set; }

        public string Type { get; private set; }

        public string ScriptName { get; set; }

        public RunOptions Options { get; set; }

        public CreatedFrom CreatedFrom { get; private set; }

        public IDictionary<string, string> Properties { get; private set; }

        public IReadOnlyDictionary<string, string> Tags { get; private set; }

        public object RunDefinition { get; private set; }

        public string Status { get; private set; }

        public Uri CancellationUri { get; private set; }

        public RunArtifactOperations RunArtifact { get; private set; }

        public DateTime LastRefreshFromDto { get; private set; }

        public bool InTerminalState
        {
            get
            {
                return TerminalStates.States.Contains(this.Status.ToLowerInvariant());
            }
        }

        public async Task WaitForCompletionAsync(
            PollingConfiguration pollConfig = default(PollingConfiguration),
            bool showOutput = false,
            Dictionary<string, List<string>> customHeaders = null,
            CancellationToken cancellation = default(CancellationToken))
        {
            pollConfig = pollConfig ?? new PollingConfiguration();

            await pollConfig.WaitForCompletion(
                async () => await this.RefreshAsync(customHeaders, cancellation).ConfigureAwait(false),
                () => this.InTerminalState == true,
                showOutput,
                cancellation).ConfigureAwait(false);
        }

        public virtual async Task<Model> RegisterModelAsync(
            string modelName,
            string modelPath = default(string),
            Dictionary<string, List<string>> customHeaders = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            const char desiredSeparator = '/';
            if (string.IsNullOrEmpty(modelPath))
            {
                modelPath = modelName;
            }

            // Normalise the path
            // - Get rid of things like './././' from the middle of the path
            // - Prevent directory escape attempts
            // - Switch to '/' separators
            string absolutePath = Path.GetFullPath(modelPath);
            string currentDirectory = Directory.GetCurrentDirectory();
            if (!absolutePath.Contains(currentDirectory))
            {
                // An attempt is being made to reference up in the directory tree, which should probably
                // be blocked
                throw new ArgumentException("modelPath appears to be attempting to escape from directory");
            }
            string normalizedPath = absolutePath.Replace(currentDirectory, string.Empty);
            normalizedPath = normalizedPath.Replace(Path.DirectorySeparatorChar, desiredSeparator);
            normalizedPath = normalizedPath.TrimStart(desiredSeparator);

            Throw.IfNullOrEmpty(this.DataContainerId, nameof(DataContainerId));

            var assetDict = new Dictionary<string, string>();
            assetDict.Add("prefix", string.Format("{0}", normalizedPath));

            var assetRequest = new AssetCreationRequest();
            assetRequest.Name = modelName;
            assetRequest.Artifacts = new List<IDictionary<string, string>> { assetDict };
            assetRequest.RunId = this.Id;
            assetRequest.Created = DateTime.UtcNow;
            assetRequest.Description = string.Format("{0} saved during run {1}", modelName, this.Id);

            var assetOperations = new Assets.AssetOperations(this.ServiceContext);
            Asset asset = await assetOperations.CreateAsync(assetRequest, customHeaders, cancellationToken).ConfigureAwait(false);

            var modelRequest = new ModelRegistrationRequest();
            modelRequest.Name = modelName;
            modelRequest.SetUrlWithAssetId(asset.Id);
            modelRequest.Unpack = false;
            modelRequest.MimeType = "application/json";

            // This is a problem if the workspace has extra model factories registered in the ModelOperations
            // object dangling off the Workspace. We don't have a way of accessing those here, so the
            // user will only have the DefaultModelFactory available at this point in the code
            // If they need another factory, they would need to re-get the Model from
            // ws.Models.GetAsync()
            var modelOperations = new Models.ModelOperations(this.ServiceContext);
            Model model = await modelOperations.RegisterAsync(modelRequest, customHeaders, cancellationToken).ConfigureAwait(false);
            return model;
        }

        public async Task<RunDetails> GetRunDetailsAsync(
            Dictionary<string, List<string>> customHeaders = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            // Not putting this in a separate 'Operations' class since the only thing to
            // do is a GET operation, which will return a single item
            var restClient = new GeneratedOld.RestClient(this.ServiceContext.Credentials);
            restClient.BaseUri = this.ServiceContext.ExperimentationEndpoint;

            GeneratedOld.Models.RunDetailsDto response = await RestCallWrapper.WrapAsync(
                () => restClient.Run.GetDetailsWithHttpMessagesAsync(
                    this.ServiceContext.SubscriptionId,
                    this.ServiceContext.ResourceGroupName,
                    this.ServiceContext.WorkspaceName,
                    this.ExperimentName,
                    this.Id,
                    customHeaders: customHeaders,
                    cancellationToken: cancellationToken)).ConfigureAwait(false);

            return new RunDetails(this, response);
        }

        public IPageFetcher<Run> GetPagedListOfChildren(
            string filter = default(string),
            IList<string> orderby = default(IList<string>),
            string sortorder = default(string),
            int? top = default(int?))
        {
            return new ChildRunPageFetcher(
                this.ServiceContext,
                this.ExperimentName,
                this.Factories,
                this.Id,
                filter,
                orderby,
                sortorder,
                top);
        }

        public IEnumerable<Run> ListChildren(
            string filter = default(string),
            IList<string> orderby = default(IList<string>),
            string sortorder = default(string),
            int? top = default(int?))
        {
            var lister = new LazyEnumerator<Run>();
            lister.Fetcher = this.GetPagedListOfChildren(filter, orderby, sortorder, top);

            return lister;
        }

        public IPageFetcher<RunMetric> GetMetricsPagedList()
        {
            string filter = FilterBuilder.BuildRunMetricFilter(Id);

            var metricOperations = new MetricOperations(ServiceContext, ExperimentName);

            return metricOperations.GetRunMetricPagedList(filter);
        }

        public IEnumerable<RunMetric> GetMetrics()
        {
            string filter = FilterBuilder.BuildRunMetricFilter(Id);

            var metricOperations = new MetricOperations(ServiceContext, ExperimentName);

            return metricOperations.ListRunMetrics(filter);
        }

        public async Task CancelAsync(
            Dictionary<string, List<string>> customHeaders = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var restClient = new GeneratedOld.RestClient(this.ServiceContext.Credentials);

            // Tracing
            bool shouldTrace = ServiceClientTracing.IsEnabled;
            string invocationId = null;
            if (shouldTrace)
            {
                invocationId = ServiceClientTracing.NextInvocationId.ToString();
                var tracingParameters = new Dictionary<string, object>();
                tracingParameters.Add("subscriptionId", this.ServiceContext.SubscriptionId);
                tracingParameters.Add("resourceGroupName", this.ServiceContext.ResourceGroupName);
                tracingParameters.Add("workspaceName", this.ServiceContext.WorkspaceName);
                tracingParameters.Add("experimentName", this.ExperimentName);
                tracingParameters.Add("runId", this.Id);
                tracingParameters.Add("cancellationToken", cancellationToken);
                ServiceClientTracing.Enter(invocationId, null, "PostRemoteRun", tracingParameters);
            }

            // Create the request body
            var runDetails = new GeneratedOld.Models.RunDetailsDto();
            runDetails.RunId = this.Id;

            var requestContent = new StringContent(
                Microsoft.Rest.Serialization.SafeJsonConvert.SerializeObject(
                    runDetails,
                    restClient.SerializationSettings));

            // Create HTTP transport objects
            var httpRequest = new HttpRequestMessage();
            HttpResponseMessage httpResponseMessage = null;
            httpRequest.Method = new HttpMethod("POST");
            httpRequest.RequestUri = this.CancellationUri;
            httpRequest.Content = requestContent;

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
            if (restClient.Credentials != null)
            {
                cancellationToken.ThrowIfCancellationRequested();
                await restClient.Credentials.ProcessHttpRequestAsync(httpRequest, cancellationToken).ConfigureAwait(false);
            }
            // Send Request
            if (shouldTrace)
            {
                ServiceClientTracing.SendRequest(invocationId, httpRequest);
            }
            cancellationToken.ThrowIfCancellationRequested();
            httpResponseMessage = await restClient.HttpClient.SendAsync(httpRequest, cancellationToken).ConfigureAwait(false);
            if (shouldTrace)
            {
                ServiceClientTracing.ReceiveResponse(invocationId, httpResponseMessage);
            }
            HttpStatusCode statusCode = httpResponseMessage.StatusCode;
            cancellationToken.ThrowIfCancellationRequested();
            string responseContent = null;
            if ((int)statusCode != 200)
            {
                var ex = new MachineLearningServiceException(string.Format("Operation returned an invalid status code '{0}'", statusCode));
                if (httpResponseMessage.Content != null)
                {
                    try
                    {
                        responseContent = await httpResponseMessage.Content.ReadAsStringAsync().ConfigureAwait(false);
                        var errorBody = Microsoft.Rest.Serialization.SafeJsonConvert.DeserializeObject<GeneratedOld.Models.ErrorResponse>(responseContent, restClient.DeserializationSettings);
                        if (errorBody != null)
                        {
                            ex.Body = new ErrorResponse(errorBody);
                        }
                    }
                    catch (JsonException)
                    {
                        // Ignore the exception
                    }
                }
                else
                {
                    responseContent = string.Empty;
                }
                ex.Request = new HttpRequestMessageWrapper(httpRequest, requestContent.ToString());
                ex.Response = new HttpResponseMessageWrapper(httpResponseMessage, responseContent);
                if (shouldTrace)
                {
                    ServiceClientTracing.Error(invocationId, ex);
                }
                httpRequest.Dispose();
                if (httpResponseMessage != null)
                {
                    httpResponseMessage.Dispose();
                }
                throw ex;
            }
            // Create Result
            var result = new HttpOperationResponse<GeneratedOld.Models.RunStatus>();
            result.Request = httpRequest;
            result.Response = httpResponseMessage;
            // Deserialize Response
            if ((int)statusCode == 200)
            {
                responseContent = await httpResponseMessage.Content.ReadAsStringAsync().ConfigureAwait(false);
                try
                {
                    result.Body =
                        Microsoft.Rest.Serialization.SafeJsonConvert.DeserializeObject<GeneratedOld.Models.RunStatus>(
                            responseContent,
                            restClient.DeserializationSettings);
                }
                catch (JsonException jsonException)
                {
                    httpRequest.Dispose();
                    if (httpResponseMessage != null)
                    {
                        httpResponseMessage.Dispose();
                    }
                    throw new SerializationException("Unable to deserialize the response.", responseContent, jsonException);
                }
            }
            if (shouldTrace)
            {
                ServiceClientTracing.Exit(invocationId, result);
            }

            await this.RefreshAsync(customHeaders, cancellationToken).ConfigureAwait(false);
        }

        public async Task RefreshAsync(
            Dictionary<string, List<string>> customHeaders = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var restClient = new GeneratedOld.RestClient(this.ServiceContext.Credentials);
            restClient.BaseUri = this.ServiceContext.ExperimentationEndpoint;

            var response = await restClient.Run.GetWithHttpMessagesAsync(
                this.ServiceContext.SubscriptionId,
                this.ServiceContext.ResourceGroupName,
                this.ServiceContext.WorkspaceName,
                this.ExperimentName,
                this.Id,
                customHeaders: customHeaders,
                cancellationToken: cancellationToken).ConfigureAwait(false);

            this.RefreshFromDto(response.Body);
        }

        public void RefreshFromDto(GeneratedOld.Models.RunDto runDto)
        {
            Throw.IfNull(runDto, nameof(runDto));

            if ((this.Id != null) && (this.Id != runDto.RunId))
            {
                throw new ArgumentException("Mismatch in RunId");
            }

            this.Number = runDto.RunNumber;

            this.Id = runDto.RunId;
            this.ParentId = runDto.ParentRunId;
            this.RootRunId = runDto.RootRunId;

            this.CreatedUtc = runDto.CreatedUtc;
            this.UserId = runDto.UserId;

            this.Token = runDto.Token;
            this.TokenExpiryUtc = runDto.TokenExpiryTimeUtc;

            this.StartTimeUtc = runDto.StartTimeUtc;
            this.EndTimeUtc = runDto.EndTimeUtc;

            this.HeartbeatEnabled = runDto.HeartbeatEnabled;

            this.DataContainerId = runDto.DataContainerId;
            this.Hidden = runDto.Hidden;

            this.Target = runDto.Target;
            this.ScriptName = runDto.ScriptName;
            this.Type = runDto.RunType;

            this.Name = runDto.Name;
            this.Description = runDto.Description;
            this.Properties = runDto.Properties;

            this.RunDefinition = runDto.RunDefinition;

            this.Status = runDto.Status;

            if (runDto.Tags != null)
            {
                this.Tags = new ReadOnlyDictionary<string, string>(runDto.Tags);
            }
            else
            {
                this.Tags = null;
            }

            if (runDto.CreatedFrom != null)
            {
                this.CreatedFrom = new CreatedFrom(runDto.CreatedFrom);
            }
            else
            {
                this.CreatedFrom = null;
            }

            if (runDto.Options != null)
            {
                this.Options = new RunOptions(runDto.Options);
            }
            else
            {
                this.Options = null;
            }

            if (!string.IsNullOrEmpty(runDto.CancelUri))
            {
                this.CancellationUri = new Uri(runDto.CancelUri);
            }

            this.LastRefreshFromDto = DateTime.UtcNow;
        }
    }
}
