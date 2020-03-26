// <copyright file="AutoMLEngine.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Azure.MachineLearning.Services.AMLRest;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.ML.AutoML;
using Microsoft.ML.CodeGenerator;
using Microsoft.ML.CodeGenerator.CodeGenerator.CSharp.AzureCodeGenerator;
using Microsoft.ML.CodeGenerator.CSharp;
using Microsoft.ML.Experimental;
using Microsoft.ML.ModelBuilder.AutoMLService;
using Microsoft.ML.ModelBuilder.AutoMLService.Contract;
using Microsoft.ML.ModelBuilder.AutoMLService.Contract.AzureResourceObjects;
using Microsoft.ML.ModelBuilder.AutoMLService.Experiments;
using Microsoft.ML.ModelBuilder.AutoMLService.RemoteAutoML;
using Microsoft.ML.ModelBuilder.AutoMLService.Telemetry;
using Microsoft.ML.Vision;
using DataReceivedEventArgs = Microsoft.ML.ModelBuilder.AutoMLService.DataReceivedEventArgs;

namespace Microsoft.ML.ModelBuilder
{
    internal class AutoMLEngine : IAutoMLService
    {
        private Pipeline bestPipeline;
        private ITransformer bestModel;
        private DataViewSchema inputSchema;
        private IPredictEngine predictEngine;
        private MLContext mlContext;
        private bool isDisposed = false;
        private readonly string CODEGEN_STABLE_VERSION = "1.5.0-preview";
        private readonly string CODEGEN_UNSTABLE_VERSION = "0.17.0-preview";
        private readonly IAutoMLServiceLogger logger = AutoMLServiceLogger.Instance;

        public AutoMLEngine(IAutoMLServiceLogger logger)
            : this()
        {
            this.logger = logger;
        }

        public AutoMLEngine()
        {
            this.mlContext = new MLContext();
            this.mlContext.Log += this.Context_Log;
            AutoMLTelemetry.Instance.AutoMLTelemetryEventHandler += this.Telemetry_AutoMLTelemetryEventHandler;
        }

        ~AutoMLEngine()
        {
            this.Dispose();
        }

        public event EventHandler<RemoteRunStartedEventArgs> RunStarted;

        public event EventHandler<AlgorithmIterationEventArgs> AlgorithmIterationCompleted;

        public event EventHandler<DataReceivedEventArgs> DiagnosticDataReceived;

        public event EventHandler<AutoMLTelemetryEvent> AutoMLTelemetryReceived;

        public string BestModelMap { get; private set; }

        private IModelBuilderService ModelBuilderService
        {
            get
            {
                var service = AutoMLService.AutoMLService.ServiceCollection.BuildServiceProvider().GetRequiredService<IModelBuilderService>();
                if (service == null)
                {
                    throw new Exception();
                }

                return service;
            }
        }

        // start a new train round
        public async Task<TrainResult> StartTrainingAsync(AutoMLServiceParamater config, CancellationToken userCancellationToken)
        {
            // Track SystemInfoEvent
            SystemInfoEvent.TrackEvent();

            // clear up temp location
            // sample TempOutputDirectory: /path/to/Tempfolder/config.Name
            if (Directory.Exists(config.TempOutputDirectory))
            {
                Directory.Delete(config.TempOutputDirectory, true);
            }

            // Setup LogLevel
            this.logger.LogLevel = config.Verbosity;

            // Load data and set column information from the user
            var inputColumnInformation = new ColumnInformation();
            inputColumnInformation.LabelColumnName = config.LabelColumn;
            inputColumnInformation.UserIdColumnName = config.UserColumn;
            inputColumnInformation.ItemIdColumnName = config.ItemColumn;
            foreach (var ignoredColumnName in config.IgnoredColumnNames)
            {
                inputColumnInformation.IgnoredColumnNames.Add(ignoredColumnName);
            }

            // Infer the column names and types
            var inferColumnsStopwatch = Stopwatch.StartNew();

            // Use internal API to do column inference.
            var inferColumnInformation = ColumnInferenceApi.InferColumns(this.mlContext, config.InputFile, inputColumnInformation, null, null, null, false, false, config.HasHeader);

            // Track InferColumnsEvent
            InferColumnsEvent.TrackEvent(inputColumnInformation, inferColumnsStopwatch.Elapsed);

            var textLoader = this.mlContext.Data.CreateTextLoader(inferColumnInformation.TextLoaderOptions);
            var trainData = textLoader.Load(config.InputFile);
            IDataView validateData = null;
            if (config.ValidateFile != null)
            {
                validateData = textLoader.Load(config.ValidateFile);
            }

            this.inputSchema = trainData.Schema;

            // clear label, or for next experiment that is not azure training, it will cause strange label bug
            LabelMapping.Label = null;

            // Set up cancellation token source for when the training time ends.
            var timeoutCancellationSource = new CancellationTokenSource();
            timeoutCancellationSource.CancelAfter(config.TrainTime * 1000);
            var timeoutCancellationToken = timeoutCancellationSource.Token;

            // register cancellation in userCancellationToken
            userCancellationToken.Register(() =>
            {
                this.mlContext.CancelExecution();
            });

            var experiment = ExperimentFactory.CreateExperiment(this.mlContext, config);
            experiment.RunStarted += this.OnRunStarted;
            experiment.AlgorithmIterationCompleted += this.OnAlgorithmIterationEvent;
            var result = await experiment.ExecuteAsync(trainData, validateData, inputColumnInformation, userCancellationToken, timeoutCancellationToken);
            this.bestModel = result.BestModel;
            this.bestPipeline = result.BestPipeline;

            if (this.bestModel != null && this.bestPipeline != null)
            {
                // generate model
                this.SaveModel(this.mlContext, this.bestModel, trainData.Schema, config.ModelPath);

                // Code Gen
                IProjectGenerator codeGen;
                if (config.IsAzureTraining && config.Scenario == AutoMLSharedServiceConstants.ImageClassification)
                {
                    codeGen = this.CreateAzureImageCodeGenerator(this.bestPipeline, inferColumnInformation, config);
                }
                else
                {
                    codeGen = this.CreateCodeGenerator(this.bestPipeline, inferColumnInformation, config);
                }

                codeGen.GenerateOutput();
                this.logger?.Info("Code Generated");
            }

            // initialize predict Engine
            this.predictEngine = new PredictEngine()
            {
                Model = this.bestModel,
                InputSchema = this.inputSchema,
            };

            return result.TrainResult;
        }

        public void Dispose()
        {
            if (this.isDisposed == false)
            {
                AutoMLTelemetry.Instance.AutoMLTelemetryEventHandler -= this.Telemetry_AutoMLTelemetryEventHandler;
                this.mlContext.Log -= this.Context_Log;
                this.mlContext.CancelExecution();
                this.isDisposed = true;
            }
        }

        public async Task<KeyValuePair<string, float>> PredictBinaryClassificationAsync(IDictionary<string, object> values, string predictedLabelColumnName = "PredictedLabel", string scoreColumnName = "Score")
        {
            return await this.predictEngine.PredictBinaryClassificationAsync(values, predictedLabelColumnName, scoreColumnName);
        }

        public async Task<IEnumerable<KeyValuePair<string, float>>> PredictMultiClassClassificationAsync(IDictionary<string, object> values, string labelColumnName = "Name", string scoreColumnName = "Score")
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            return await this.predictEngine.PredictMultiClassClassificationAsync(values, labelColumnName, scoreColumnName);
        }

        public async Task<KeyValuePair<string, float>> PredictRegressionAsync(IDictionary<string, object> values, string labelColumnName, string scoreColumnName = "Score")
        {
            return await this.predictEngine.PredictRegressionAsync(values, labelColumnName, scoreColumnName);
        }

        public async Task<KeyValuePair<string, float>> PredictRecommendationAsync(IDictionary<string, object> values, string labelColumnName, string scoreColumnName = "Score")
        {
            return await this.predictEngine.PredictRecommendationAsync(values, labelColumnName, scoreColumnName);
        }

        public async Task<IEnumerable<KeyValuePair<string, float>>> PredictRecommendationBatchAsync(IEnumerable<IDictionary<string, object>> values, string labelColumnName, string scoreColumnName = "Score")
        {
            return await this.predictEngine.PredictRecommendationBatchAsync(values, labelColumnName, scoreColumnName);
        }

        public async Task<List<SubscriptionInformation>> RequestSubscriptionsAsync(string token)
        {
            return await AzureAttachManager.RequestSubscriptionsAsync(token);
        }

        public async Task<List<WorkspaceInformation>> RequestAzureWorkspaceAsync(SubscriptionInformation selectedSubscription, string token)
        {
            return await AzureAttachManager.RequestAzureWorkspaceAsync(selectedSubscription, token);
        }

        public async Task<List<ComputeResource>> RequestAzureMLComputeAsync(AzureResource mlWorkspace, string token)
        {
            return await AzureAttachManager.RequestAzureMLComputeAsync(mlWorkspace, token);
        }

        public async Task<List<Datasource>> RequestDataStoresAsync(AzureResource mlWorkspace, string token)
        {
            return await AzureAttachManager.RequestDataStoresAsync(mlWorkspace, token);
        }

        public async Task PostCreateExperimentAsync(AzureResource selectedWorkspace, string experimentName, string token)
        {
            await AzureAttachManager.PostCreateExperimentAsync(selectedWorkspace, experimentName, token);
        }

        public async Task<List<ResourceGroup>> RequestResourceGroupsAsync(SubscriptionInformation selectedSubscription, string token)
        {
            return await AzureAttachManager.RequestResourceGroupsAsync(selectedSubscription, token);
        }

        public async Task<List<AzureRegion>> RequestRegionsAsync(SubscriptionInformation selectedSubscription, string token)
        {
            return await AzureAttachManager.RequestRegionsAsync(selectedSubscription, token);
        }

        public async Task<WorkspaceInformation> PutCreateWorkspaceAsync(SubscriptionInformation selectedSubscription, string selectedRegion, ResourceGroup selectedGroup, string newResourceGroupName, string workspaceName, string token)
        {
            return await AzureAttachManager.PutCreateWorkspaceAsync(selectedSubscription, selectedRegion, selectedGroup, newResourceGroupName, workspaceName, token);
        }

        public async Task<AzureResource> PutCreateComputeAsync(WorkspaceInformation selectedWorksapce, string computeType, string computeName, string token)
        {
            return await AzureAttachManager.PutCreateComputeAsync(selectedWorksapce, computeType, computeName, token);
        }

        public async Task<List<ComputeTypeResource>> GetAvailableComputesForRegionAsync(SubscriptionInformation subscription, string location, string token)
        {
            return await AzureAttachManager.GetAvailableComputesForRegionAsync(subscription, location, token);
        }

        public async Task<ComputeResource> GetComputeResourceAsync(string computeId, string token)
        {
            return await AzureAttachManager.GetComputeResourceAsync(computeId, token);
        }

        protected virtual void OnRunStarted(object sender, RemoteRunStartedEventArgs e)
        {
            this.RunStarted?.Invoke(this, e);
        }

        protected virtual void OnAlgorithmIterationEvent(object sender, AlgorithmIterationEventArgs e)
        {
            this.AlgorithmIterationCompleted?.Invoke(this, e);
        }

        protected virtual void OnDiagnosticDataReceived(object sender, AutoMLService.DataReceivedEventArgs e)
        {
            this.DiagnosticDataReceived?.Invoke(this, e);
        }

        private void Telemetry_AutoMLTelemetryEventHandler(object sender, AutoMLTelemetryEvent e)
        {
            this.AutoMLTelemetryReceived?.Invoke(this, e);
        }

        private void Context_Log(object sender, LoggingEventArgs e)
        {
            if (e.Message.Contains(nameof(ImageClassificationTrainer)))
            {
                this.logger.Info(e.Message);
            }

            this.logger.Trace(e.Message);
        }

        private void SaveModel(MLContext context, ITransformer model, DataViewSchema dataViewSchema, string location)
        {
            var modelPath = new FileInfo(location);
            if (!Directory.Exists(modelPath.Directory.FullName))
            {
                Directory.CreateDirectory(modelPath.Directory.FullName);
            }

            using (var fs = File.Create(modelPath.FullName))
            {
                context.Model.Save(model, dataViewSchema, fs);
            }
        }

        private IProjectGenerator CreateCodeGenerator(Pipeline pipeline, ColumnInferenceResults columnInferenceResults, ILocalAutoMLTrainParameters config, bool isAzureAttach = false)
        {
            TaskKind mlTask = this.GetTaskKind(config.Scenario);

            var codeGenerator = new CodeGenerator.CSharp.CodeGenerator(pipeline, columnInferenceResults, new CodeGeneratorSettings()
            {
                TrainDataset = config.InputFile,
                TestDataset = config.TestFile,
                ModelPath = config.ModelPath,
                MlTask = mlTask,
                OutputName = config.Name,
                OutputBaseDir = config.TempOutputDirectory,
                LabelName = config.LabelColumn,
                Target = GenerateTarget.ModelBuilder,
                StablePackageVersion = this.CODEGEN_STABLE_VERSION,
                UnstablePackageVersion = this.CODEGEN_UNSTABLE_VERSION,
            });

            return codeGenerator;
        }

        private IProjectGenerator CreateAzureImageCodeGenerator(Pipeline pipeline, ColumnInferenceResults columnInference, IRemoteAutoMLTrainParameters config)
        {
            TaskKind mlTask = this.GetTaskKind(config.Scenario);
            var setting = new CodeGeneratorSettings()
            {
                TrainDataset = config.InputFile,
                ModelPath = config.ModelPath,
                MlTask = mlTask,
                OutputName = config.Name,
                OutputBaseDir = Path.GetDirectoryName(config.TempOutputDirectory),
                LabelName = config.LabelColumn,
                Target = GenerateTarget.ModelBuilder,
                StablePackageVersion = this.CODEGEN_STABLE_VERSION,
                UnstablePackageVersion = this.CODEGEN_UNSTABLE_VERSION,
                OnnxModelPath = config.OnnxModelPath,
                ClassificationLabel = LabelMapping.Label,
                IsAzureAttach = true,
                IsImage = true,
            };
            var codeGen = new AzureAttachCodeGenenrator(pipeline, columnInference, setting);
            return codeGen;
        }

        private TaskKind GetTaskKind(string taskKind)
        {
            TaskKind mlTask;

            if (taskKind == AutoMLSharedServiceConstants.MulticlassClassification || taskKind == AutoMLSharedServiceConstants.ImageClassification)
            {
                mlTask = TaskKind.MulticlassClassification;
            }
            else if (taskKind == AutoMLSharedServiceConstants.BinaryClassification)
            {
                mlTask = TaskKind.BinaryClassification;
            }
            else if (taskKind == AutoMLSharedServiceConstants.Regression)
            {
                mlTask = TaskKind.Regression;
            }
            else if (taskKind == AutoMLSharedServiceConstants.Recommendation)
            {
                mlTask = TaskKind.Recommendation;
            }
            else
            {
                throw new Exception("mlTask transfer fail");
            }

            return mlTask;
        }
    }
}
