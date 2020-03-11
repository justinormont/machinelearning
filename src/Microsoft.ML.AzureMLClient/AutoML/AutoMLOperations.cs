// <copyright file="AutoMLOperations.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Azure.MachineLearning.Services.GeneratedOld;
using Azure.MachineLearning.Services.GeneratedOld.Models;
using Azure.MachineLearning.Services.Snapshots;
using Newtonsoft.Json;

namespace Azure.MachineLearning.Services.AutoML
{
    public class AutoMLOperations
    {
        public AutoMLOperations(ServiceContext serviceContext)
        {
            Throw.IfNull(serviceContext, nameof(serviceContext));
            this.ServiceContext = serviceContext;
        }

        public ServiceContext ServiceContext { get; private set; }

        internal async Task<string> CreateParentRunAsync(
            AutoMLConfiguration configuration,
            string experimentName,
            Dictionary<string, List<string>> customHeaders = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            RestClient restClient = GetRestClient();

            CreateParentRunDto createParentRunDto = CreateParentRunDtoFromConfiguration(configuration);

            string jasonSettingsString = $"{{\"debug_log\": \"automl_errors.log\", \"primary_metric\": \"accuracy\", \"iterations\": 1, \"compute_target\": \"{configuration.ComputeTarget.Name}\",\"images_folder\": \"{configuration.AutoMLSettings.ImagesFolder}\" , \"enable_dnn\": true, \"labels_file\": \"{configuration.AutoMLSettings.LabelsFile}\", \"max_concurrent_iterations\": 5, \"max_cores_per_iteration\": 2, \'exit_score\': 0.99, \'metric_operation\': \"maximize\", \"task_type\": \"image-classification\", \"epochs\": 2}}";
            string rawAMLSettingsString = $"{{'debug_log': 'automl_errors.log', 'primary_metric': 'accuracy', 'iterations': 1, 'compute_target': '{configuration.ComputeTarget.Name}', 'images_folder': '{configuration.AutoMLSettings.ImagesFolder}', 'enable_dnn': True, 'labels_file': '{configuration.AutoMLSettings.LabelsFile}', 'max_concurrent_iterations': 5, 'max_cores_per_iteration': 2, 'exit_score': 0.99, 'metric_operation': 'maximize', 'task_type': 'image-classification', 'epochs': 2}}";

            createParentRunDto.AmlSettingsJsonString = jasonSettingsString;
            createParentRunDto.RawAMLSettingsString = rawAMLSettingsString;

            // Require that Jasmine sends us back JSON so we can
            // properly deserialize the result.
            Dictionary<string, List<string>> customHeadersWithAcceptJson;
            if (customHeaders == null)
            {
                customHeadersWithAcceptJson = new Dictionary<string, List<string>>();
            }
            else
            {
                customHeadersWithAcceptJson = new Dictionary<string, List<string>>(customHeaders);
            }

            customHeadersWithAcceptJson.Add("Accept", new List<string>() { "application/json" });

            string parentRunId = await RestCallWrapper.WrapAsync(
                () => restClient.Jasmine.CreateParentRunWithHttpMessagesAsync(
                    ServiceContext.SubscriptionId,
                    ServiceContext.ResourceGroupName,
                    ServiceContext.WorkspaceName,
                    experimentName,
                    createParentRunDto,
                    customHeaders: customHeadersWithAcceptJson,
                    cancellationToken: cancellationToken)).ConfigureAwait(false);

            return parentRunId;
        }

        internal async Task PostRemoteSnapshotRunAsync(
            string parentRunId,
            string experimentName,
            AutoMLConfiguration configuration,
            Dictionary<string, List<string>> customHeaders = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            RestClient restClient = GetRestClient();

            Guid? snapshotId = null;

            if (configuration.Path != null)
            {
                var snapshotClient = new SnapshotOperations(ServiceContext);
                snapshotId = await snapshotClient.SnapshotDirectoryAsync(
                    configuration.Path,
                    parentSnapshotId: null,
                    customHeaders: customHeaders,
                    cancellationToken: cancellationToken).ConfigureAwait(false);
            }

            RunDefinition runDefinition = configuration.BuildChildRunDefinition(
                parentRunId,
                snapshotId);

            var runDefinitionJson = JsonConvert.SerializeObject(runDefinition);

            await RestCallWrapper.WrapAsync(
                () => restClient.Jasmine.PostRemoteSnapshotRunWithHttpMessagesAsync(
                    ServiceContext.SubscriptionId,
                    ServiceContext.ResourceGroupName,
                    ServiceContext.WorkspaceName,
                    experimentName,
                    parentRunId,
                    runDefinitionJson,
                    snapshotId,
                    customHeaders: customHeaders,
                    cancellationToken: cancellationToken)).ConfigureAwait(false);
        }

        private CreateParentRunDto CreateParentRunDtoFromConfiguration(AutoMLConfiguration autoMLConfiguration)
        {
            return new CreateParentRunDto
            {
                Target = autoMLConfiguration.ComputeTarget.Name,
                NumIterations = autoMLConfiguration.AutoMLSettings.Iterations,
                TrainingType = null,
                AcquisitionFunction = null,
                Metrics = new List<string>() { autoMLConfiguration.AutoMLSettings.PrimaryMetric }, // TODO: what does this do?
                PrimaryMetric = autoMLConfiguration.AutoMLSettings.PrimaryMetric,
                TrainSplit = autoMLConfiguration.AutoMLSettings.ValidationSize,
                MaxTimeSeconds = autoMLConfiguration.AutoMLSettings.IterationTimeoutMinutes * 60,
                AcquisitionParameter = 0.0,
                NumCrossValidation = autoMLConfiguration.AutoMLSettings.NCrossValidations,
                AmlSettingsJsonString = autoMLConfiguration.AutoMLSettings.ToJsonString(),
                RawAMLSettingsString = autoMLConfiguration.AutoMLSettings.ToPythonString(),
                EnableSubsampling = autoMLConfiguration.AutoMLSettings.EnableSubsampling
            };
        }

        private RestClient GetRestClient()
        {
            var restClient = new RestClient(ServiceContext.Credentials);
            restClient.HttpClient.DefaultRequestHeaders.UserAgent.TryParseAdd("MLNET Model Builder");
            restClient.BaseUri = ServiceContext.HistoryEndpoint;

            return restClient;
        }
    }
}
