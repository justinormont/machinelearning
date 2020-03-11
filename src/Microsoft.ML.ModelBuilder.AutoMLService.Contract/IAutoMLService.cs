// <copyright file="IAutoMLService.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ML.ModelBuilder.AutoMLService.Contract;
using Microsoft.ML.ModelBuilder.AutoMLService.Contract.AzureResourceObjects;

namespace Microsoft.ML.ModelBuilder.AutoMLService
{
    public interface IAutoMLService : IPredictEngine, IDisposable, IAzureRestService
    {
        event EventHandler<RemoteRunStartedEventArgs> RunStarted;

        event EventHandler<AlgorithmIterationEventArgs> AlgorithmIterationCompleted;

        event EventHandler<DataReceivedEventArgs> DiagnosticDataReceived;

        event EventHandler<AutoMLTelemetryEvent> AutoMLTelemetryReceived;

        Task<TrainResult> StartTrainingAsync(AutoMLServiceParamater config, CancellationToken cancellationToken);
    }

    public interface IPredictEngine
    {
        Task<KeyValuePair<string, float>> PredictBinaryClassificationAsync(IDictionary<string, object> values, string predictedLabelColumnName, string scoreColumnName);

        Task<IEnumerable<KeyValuePair<string, float>>> PredictMultiClassClassificationAsync(IDictionary<string, object> values, string labelColumnName, string scoreColumnName);

        Task<KeyValuePair<string, float>> PredictRegressionAsync(IDictionary<string, object> values, string labelColumnName, string scoreColumnName = "Score");

        Task<KeyValuePair<string, float>> PredictRecommendationAsync(IDictionary<string, object> values, string labelColumnName, string scoreColumnName = "Score");

        Task<IEnumerable<KeyValuePair<string, float>>> PredictRecommendationBatchAsync(IEnumerable<IDictionary<string, object>> values, string labelColumnName, string scoreColumnName = "Score");
    }

    public interface IAzureRestService
    {
        Task<List<SubscriptionInformation>> RequestSubscriptionsAsync(string token);

        Task<List<WorkspaceInformation>> RequestAzureWorkspaceAsync(SubscriptionInformation selectedSubscription, string token);

        Task<List<ComputeResource>> RequestAzureMLComputeAsync(AzureResource mlWorkspace, string token);

        Task<List<Datasource>> RequestDataStoresAsync(AzureResource mlWorkspace, string token);

        Task PostCreateExperimentAsync(AzureResource selectedWorkspace, string experimentName, string token);

        Task<List<AzureRegion>> RequestRegionsAsync(SubscriptionInformation selectedSubscription, string token);

        Task<WorkspaceInformation> PutCreateWorkspaceAsync(SubscriptionInformation selectedSubscription, string selectedRegion, ResourceGroup selectedGroup, string newResourceGroupName, string workspaceName, string token);

        Task<AzureResource> PutCreateComputeAsync(WorkspaceInformation selectedWorksapce, string computeType, string computeName, string token);

        Task<List<ComputeTypeResource>> GetAvailableComputesForRegionAsync(SubscriptionInformation subscription, string location, string token);

        Task<ComputeResource> GetComputeResourceAsync(string computeId, string token);

        Task<List<ResourceGroup>> RequestResourceGroupsAsync(SubscriptionInformation selectedSubscription, string token);
    }
}
