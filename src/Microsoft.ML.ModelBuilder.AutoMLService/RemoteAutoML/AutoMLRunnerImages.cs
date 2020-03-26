// <copyright file="AutoMLRunnerImages.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Azure;
using Azure.MachineLearning.Services;
using Azure.MachineLearning.Services.AutoML;
using Azure.MachineLearning.Services.Compute;
using Azure.MachineLearning.Services.Runs;
using Azure.MachineLearning.Services.Workspaces;
using Microsoft.Azure.Management.ResourceManager.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent.Authentication;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.ML;
using Microsoft.ML.AutoML;
using Microsoft.ML.CodeGenerator.CSharp;
using Microsoft.ML.ModelBuilder.AutoMLService;
using Microsoft.ML.ModelBuilder.AutoMLService.Contract;
using Microsoft.ML.ModelBuilder.AutoMLService.RemoteAutoML;
using Microsoft.ML.ModelBuilder.AzCopyService.Contract;
using Microsoft.Rest;
using Newtonsoft.Json;

namespace AzureML
{
    // TODO
    // For orphan Transformer ( transformer that doesn't exist in AutoML )
    internal enum SpecialTransformer
    {
        ApplyOnnxModel = 0,
        ResizeImage = 1,
        ExtractPixel = 2,
        NormalizeMapping = 3,
        LabelMapping = 4,
    }


    internal class AutoMLRunnerImages
    {
        private const string DataRefName = "AUTOML1";
        private readonly IAutoMLServiceLogger logger;
        private TimeSpan maxExplorationTime;
        private string taskType;
        private string subscriptionId;
        private string resourceGroup;
        private string experimentName;
        private string workspaceName;
        private string remoteTrainingFileName;
        private string localTrainingFileName;
        private string labelColumnName;
        private string computeTarget;
        private string token;
        private string projectLocation;
        private string bestOnnxModelLocation;
        private string bestOnnxModelLabelLocation;
        private AutoMLRun parentRun;
        private RemoteSasLocation remoteTrainArtifactLocation;

        public AutoMLRunnerImages(IRemoteAutoMLTrainParameters trainParameters, IAutoMLServiceLogger logger = null)
        {
            this.logger = logger;
            this.maxExplorationTime = TimeSpan.FromSeconds(trainParameters.TrainTime);
            this.taskType = trainParameters.TaskType;
            this.subscriptionId = trainParameters.SubscriptionId;
            this.resourceGroup = trainParameters.ResourceGroup;
            this.experimentName = trainParameters.ExperimentName;
            this.workspaceName = trainParameters.WorkspaceName;
            this.localTrainingFileName = trainParameters.InputFile;
            this.remoteTrainingFileName = trainParameters.RemoteInputFile.Replace('\\', '/');
            this.labelColumnName = trainParameters.RemoteLabelColumn;
            this.computeTarget = trainParameters.ComputeTarget;
            this.projectLocation = trainParameters.TempOutputDirectory;
            this.bestOnnxModelLocation = trainParameters.OnnxModelPath;
            this.bestOnnxModelLabelLocation = trainParameters.OnnxModelLabelPath;
            this.remoteTrainArtifactLocation = trainParameters.TrainArtifactLoaction;
        }

        public event EventHandler<RemoteRunStartedEventArgs> RunStarted;

        public event EventHandler<AlgorithmIterationEventArgs> AlgorithmIterationCompleted;

        private IModelBuilderService ModelBuilderService
        {
            get
            {
                var service = AutoMLService.ServiceCollection.BuildServiceProvider().GetRequiredService<IModelBuilderService>();
                if (service == null)
                {
                    throw new Exception();
                }

                return service;
            }
        }

        public async System.Threading.Tasks.Task<AutoMLRunnerResult> RunAutoMLAsync(MLContext mLContext, CancellationToken cancellationToken = default)
        {
            // System.Diagnostics.Debugger.Break();
            var token = await this.ModelBuilderService.RefreshTokenAsync(cancellationToken);
            var tokenCredentials = new TokenCredentials(token);

            var serviceClientCredentials = new AzureCredentials(
                tokenCredentials,
                tokenCredentials,
                null,  // TODO: provide a way to specify TenantId?
                AzureEnvironment.AzureGlobalCloud);

            try
            {
                this.logger?.Info("Verifying parameters: workspace.. ");

                var workspace = await AmlUtils.CallAMLAndHandleExceptionsAsync(
                    async () =>
                    {
                        var wsClient = new WorkspaceClient(serviceClientCredentials);
                        return await wsClient.Workspaces.GetAsync(new Guid(this.subscriptionId), this.resourceGroup, this.workspaceName, cancellationToken: cancellationToken);
                    },
                    "workspace",
                    this.workspaceName);

                this.logger?.Info("Ok, experiment.. ");

                var experiment = await AmlUtils.CallAMLAndHandleExceptionsAsync(
                    async () => { return await workspace.Experiments.GetAsync(this.experimentName, cancellationToken: cancellationToken); }, "experiment", this.experimentName);

                this.logger?.Info("Ok, compute target.. ");

                var computeTargets = AmlUtils.CallAMLAndHandleExceptions(
                    () => { return workspace.ComputeTargets.List(); }, "computeTargets", this.experimentName);

                ComputeTarget aMLComputeTarget = null;
                if (this.computeTarget != null)
                {
                    aMLComputeTarget = computeTargets.Where(ct => ct.Name.Contains(this.computeTarget)).FirstOrDefault();
                }
                else
                {
                    if (computeTargets.Count() == 1)
                    {
                        aMLComputeTarget = computeTargets.First();
                    }
                }

                if (aMLComputeTarget == null)
                {
                    throw new Exception($"No compute targets found in workspace {workspace.Name}");
                }

                this.logger?.Info("Ok.");

                var autoMLConfig = this.GetTrainingRunConfig(workspace, aMLComputeTarget, this.maxExplorationTime, this.taskType, this.remoteTrainingFileName, this.labelColumnName);

                // Training
                this.logger?.Info($"Starting AutoML run in workspace {workspace.Name}, experiment {experiment.Name} using compute target {aMLComputeTarget.Name}.");

                this.parentRun = (await experiment.Runs.CreateAsync(autoMLConfig, cancellationToken: cancellationToken)) as AutoMLRun;
                var monitorUrl = AutoMLRunMonitoringImages.GetRunUrl(this.parentRun, workspace.SubscriptionId.ToString(), workspace.ResourceGroupName, workspace.Name);

                this.logger?.Info($"Created AutoML run {this.parentRun.Name}.");
                this.logger?.Info($"You can also monitor this run using URL: {monitorUrl}.");
                this.OnRunStarted(this, monitorUrl);

                var runResults = await AutoMLRunMonitoringImages.ReportStatusAsync(this.parentRun, workspace, experiment, (AlgorithmIterationEventArgs args) => this.OnAlgorithmIterationEvent(this, args), this.logger, cancellationToken);

                this.logger?.Info($"Completed a training run");

                this.logger?.Info($"Downloading best model to {this.bestOnnxModelLocation}..");

                var bestRun = runResults.bestRun;

                var modelPath = "train_artifacts/model.onnx";
                var modelMapPath = "train_artifacts/labels.json";

                var blobContainer = bestRun.DataContainerId;
                var autoMLRunSubFolder = $"azureml/{this.parentRun.Name}_HD_0";

                var modelDirectoryPath = new FileInfo(this.bestOnnxModelLocation).Directory.FullName;
                if (!Directory.Exists(modelDirectoryPath))
                {
                    Directory.CreateDirectory(modelDirectoryPath);
                }

                var remoteOnnxModelLoaction = new RemoteSasLocation()
                {
                    ResourceUri = this.remoteTrainArtifactLocation.ResourceUri,
                    SasToken = this.remoteTrainArtifactLocation.SasToken,
                    Container = this.remoteTrainArtifactLocation.Container,
                    Path = $"azureml/{this.parentRun.Name}_HD_0/{modelPath}",
                };

                var localOnnxModelLocation = new LocalLocation()
                {
                    Path = this.bestOnnxModelLocation,
                };

                var remoteOnnxModelMapLocation = new RemoteSasLocation()
                {
                    ResourceUri = this.remoteTrainArtifactLocation.ResourceUri,
                    SasToken = this.remoteTrainArtifactLocation.SasToken,
                    Container = this.remoteTrainArtifactLocation.Container,
                    Path = $"azureml/{this.parentRun.Name}_HD_0/{modelMapPath}",
                };

                var localOnnxModelMapLocation = new LocalLocation()
                {
                    Path = this.bestOnnxModelLabelLocation,
                };

                var localOnnxLocation = await this.DownLoadFromSasToLocalAsync(remoteOnnxModelLoaction, localOnnxModelLocation, cancellationToken);
                var localLabelLocation = await this.DownLoadFromSasToLocalAsync(remoteOnnxModelMapLocation, localOnnxModelMapLocation, cancellationToken);

                this.logger?.Info(localOnnxLocation);
                this.logger?.Info(localLabelLocation);
                this.logger?.Info($"Done.");
                return this.GetBestModelAndPipeline(mLContext, this.bestOnnxModelLocation);
            }
            catch (OperationCanceledException)
            {
                await this.CancelAsync();
                throw;
            }
            catch (Exception e)
            {
                this.logger?.Info($"{e.Message}");
                throw;
            }
        }

        public async Task CancelAsync()
        {
            if (this.parentRun != null)
            {
                this.logger?.Info($"Attempting to cancel parent run {this.parentRun.Id}");
                await this.parentRun.CancelAsync();
                this.logger?.Info($"Cancel parentRun successfully");
            }
        }

        public AutoMLRunnerResult GetBestModelAndPipeline(MLContext mlContext, string modelFile)
        {
            // Deserializatin JSON
            List<string> map = default;
            Dictionary<int, string> keyMap = new Dictionary<int, string>();

            using (StreamReader sr = new StreamReader(this.bestOnnxModelLabelLocation))
            {
                var json = sr.ReadLine();
                map = JsonConvert.DeserializeObject<List<string>>(json);
            }

            LabelMapping.Label = map.ToArray();

            var trainer = mlContext.Transforms.ApplyOnnxModel(modelFile: modelFile, outputColumnName: "output1", inputColumnName: "input1");
            var transformer = mlContext.Transforms.LoadImages("ImageSource_featurized", null, "ImageSource")
                             .Append(mlContext.Transforms.ResizeImages("ImageSource_featurized", 224, 224, "ImageSource_featurized"))
                             .Append(mlContext.Transforms.ExtractPixels("ImageSource_featurized", "ImageSource_featurized"))
                             .Append(mlContext.Transforms.CustomMapping<NormalizeInput, NormalizeOutput>(
                                          (input, output) => NormalizeMapping.Mapping(input, output),
                                          contractName: nameof(NormalizeMapping)));

            // fit to get ITransformer
            var pipeline = transformer.Append(trainer)
                            .Append(mlContext.Transforms.CustomMapping<LabelMappingInput, LabelMappingOutput>(
                                          (input, output) => LabelMapping.Mapping(input, output),
                                          contractName: nameof(LabelMapping)));

            var textLoaderOption = new Microsoft.ML.Data.TextLoader.Options()
            {
                HasHeader = true,
                Separators = new char[] { '\t' },
                Columns = new Microsoft.ML.Data.TextLoader.Column[] {
                    new Microsoft.ML.Data.TextLoader.Column("Label", Microsoft.ML.Data.DataKind.String, 0),
                    new Microsoft.ML.Data.TextLoader.Column("ImageSource", Microsoft.ML.Data.DataKind.String, 1),
                },
            };
            var textLoader = mlContext.Data.CreateTextLoader(textLoaderOption);
            var trainData = textLoader.Load(this.localTrainingFileName);
            var bestModel = pipeline.Fit(trainData);

            // construct pipeline
            var onnxPipeLineNode = new PipelineNode(
                nameof(SpecialTransformer.ApplyOnnxModel),
                PipelineNodeType.Transform,
                new[] { "input.1" },
                new[] { "output.1" },
                new Dictionary<string, object>()
                {
                    { "outputColumnNames", "output1" },
                    { "inputColumnNames", "input1" },
                    { "modelFile", "awesomeModel.onnx" },   // it doesn't matter what modelFile is
                });
            var loadImageNode = new PipelineNode(EstimatorName.ImageLoading.ToString(), PipelineNodeType.Transform, "ImageSource", "ImageSource_featurized");
            var resizeImageNode = new PipelineNode(
                nameof(SpecialTransformer.ResizeImage),
                PipelineNodeType.Transform,
                "ImageSource_featurized",
                "ImageSource_featurized",
                new Dictionary<string, object>()
                {
                    { "imageWidth", 224 },
                    { "imageHeight", 224 },
                });
            var extractPixelsNode = new PipelineNode(nameof(SpecialTransformer.ExtractPixel), PipelineNodeType.Transform, "ImageSource_featurized", "ImageSource_featurized");
            var normalizeMapping = new PipelineNode(nameof(SpecialTransformer.NormalizeMapping), PipelineNodeType.Transform, string.Empty, string.Empty);
            var labelMapping = new PipelineNode(nameof(SpecialTransformer.LabelMapping), PipelineNodeType.Transform, string.Empty, string.Empty);
            var bestPipeLine = new Pipeline(new PipelineNode[]
            {
                loadImageNode,
                resizeImageNode,
                extractPixelsNode,
                normalizeMapping,
                onnxPipeLineNode,
                labelMapping,
            });
            return new AutoMLRunnerResult()
            {
                BestModel = bestModel,
                BestPipeline = bestPipeLine,
            };
        }

        public async Task<string> DownLoadFromSasToLocalAsync(RemoteSasLocation src, LocalLocation dst, CancellationToken ct)
        {
            try
            {
                return await this.ModelBuilderService.AzCopyDownloadAsync(src, dst, ct);
            }
            catch (Exception)
            {
                var res = await this.ModelBuilderService.ShowYesNoMessageBoxAsync("Download Error", "There's an error when downloading file from Azure Blob, would you like to try again?", ct);
                if (res)
                {
                    return await this.DownLoadFromSasToLocalAsync(src, dst, ct);
                }
                else
                {
                    throw;
                }
            }
        }

        internal struct AutoMLRunnerResult
        {
            public ITransformer BestModel;
            public Pipeline BestPipeline;
        }

        private void AzCopyClient_JobStatusHandler(object sender, AZCopyMessageBase e)
        {
            this.logger?.Info(e.MessageContent);
        }

        private void OnRunStarted(object sender, string url)
        {
            var eventArgs = new RemoteRunStartedEventArgs()
            {
                RemoteURL = url,
            };

            this.RunStarted.Invoke(this, eventArgs);
        }

        private void OnAlgorithmIterationEvent(object sender, AlgorithmIterationEventArgs e)
        {
            this.AlgorithmIterationCompleted?.Invoke(this, e);
        }

        private RunConfigurationBase GetTrainingRunConfig(Workspace workspace, ComputeTarget ct, TimeSpan maxExplorationTime, string taskType, string trainingFileName, string labelColumnName)
        {
            var datastores = workspace.Datastores.List().Where(ds => ds.DatastoreName.ToLowerInvariant() == "workspaceblobstore"); // *

            if (datastores.Count() == 0)
            {
                throw new ArgumentException($"Didn't find a default file datastore in workspace {workspace.Name}.");
            }

            var datastore = datastores.First();

            // get default workspace storage container name
            var defaultStorageContainer = workspace.StorageAccountArmId.Split('/').LastOrDefault();

            if (string.IsNullOrWhiteSpace(defaultStorageContainer))
            {
                throw new ArgumentException($"Default storage container is not defined for workspace {workspace.Name}");
            }

            var autoMLSettings = new AutoMLSettings(
               iterationTimeoutInMin: (int)maxExplorationTime.TotalMinutes,
               iterations: 1,
               primaryMetric: "accuracy",
               preprocess: true,
               nCrossValidations: 5);

            var rootFolder = string.Empty;

            trainingFileName = string.IsNullOrWhiteSpace(rootFolder) ? trainingFileName : rootFolder + '/' + trainingFileName;

            autoMLSettings.TaskType = "image-classification";
            autoMLSettings.EnableOnnxCompatibleModels = false;
            autoMLSettings.EnableTensorFlow = true;
            autoMLSettings.EnableDnn = true;
            autoMLSettings.ImagesFolder = rootFolder;
            autoMLSettings.LabelsFile = trainingFileName;

            autoMLSettings.Epochs = 2;
            autoMLSettings.ComputeTarget = ct.Name;

            var autoMLConfig = new AutoMLConfiguration(
                autoMLSettings: autoMLSettings,
                projectFolder: null,
                task: taskType);

            // autoMLConfig.DockerConfiguration.Enabled = true;
            autoMLConfig.PipPackages.Add("azureml-sdk[automl]");
            autoMLConfig.ComputeTarget = ct;

            // Construct a data reference
            var drc = new DataReferenceConfiguration();
            drc.DataStoreName = datastore.DatastoreName;
            drc.Mode = DataStoreMode.Mount;

            // drc.PathOnDataStore = string.Empty;
            autoMLConfig.PythonVersion = new Version(3, 6, 5);
            autoMLConfig.DockerConfiguration.BaseImage = "mcr.microsoft.com/azureml/base-gpu:intelmpi2018.3-cuda10.0-cudnn7-ubuntu16.04";

            autoMLConfig.PythonVersion = new Version(3, 6, 5);
            autoMLConfig.ScriptFile = new FileInfo("train.py");

            autoMLConfig.DataReferences = new Dictionary<string, DataReferenceConfiguration>()
            {
                { "default", new DataReferenceConfiguration() { DataStoreName = datastore.DatastoreName, Mode = DataStoreMode.Mount } },
                { "labels_file_root", new DataReferenceConfiguration() { DataStoreName = datastore.DatastoreName, Mode = DataStoreMode.Mount } },
            };

            return autoMLConfig;
        }

        private string GetGetDataPath(string projectFolder)
        {
            if (!Directory.Exists(projectFolder))
            {
                Directory.CreateDirectory(projectFolder);
            }

            return Path.Combine(projectFolder, "get_data.py");
        }
    }
}
