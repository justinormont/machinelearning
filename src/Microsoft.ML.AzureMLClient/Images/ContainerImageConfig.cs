// <copyright file="ContainerImageConfig.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

using Azure.MachineLearning.Services.Artifacts;
using Azure.MachineLearning.Services.GeneratedOld.Models;

namespace Azure.MachineLearning.Services.Images
{
    /// <summary>
    /// Copied to match the Python specification. Hand-copied until we can get a better swagger story for this file.
    /// https://docs.microsoft.com/en-us/python/api/azureml-core/azureml.core.image.container.containerimageconfig?view=azure-ml-py
    /// </summary>
    public class ContainerImageConfig
    {
        public ContainerImageConfig(
            string executionScript = null,
            string runtime = null,
            string condaFile = null,
            string dockerFile = null,
            string schemaFile = null,
            IList<FileInfo> dependencies = null,
            bool? enableGpu = null,
            IDictionary<string, string> tags = null,
            IDictionary<string, string> properties = null,
            string description = null,
            string baseImage = null,
            ContainerRegistry containerRegistry = null)
        {
            ExecutionScript = executionScript;
            Runtime = runtime;
            CondaFile = condaFile;
            DockerFile = dockerFile;
            SchemaFile = schemaFile;
            EnableGpu = enableGpu;
            Description = description;
            BaseImage = baseImage;
            BaseImageRegistry = containerRegistry;

            if (dependencies != null)
            {
                Depdendencies = dependencies;
            }

            if (tags != null)
            {
                Tags = tags;
            }

            if (properties != null)
            {
                Properties = properties;
            }

            ValidateConfiguration();
        }

        public string ExecutionScript { get; private set; } = null;

        public string Runtime { get; private set; } = null;

        public string CondaFile { get; private set; } = null;

        public string DockerFile { get; private set; } = null;

        public string SchemaFile { get; private set; } = null;

        public IList<FileInfo> Depdendencies { get; private set; } =
            new List<FileInfo>();

        public bool? EnableGpu { get; private set; }

        public IDictionary<string, string> Tags { get; private set; } =
            new Dictionary<string, string>();

        public IDictionary<string, string> Properties { get; private set; } =
            new Dictionary<string, string>();

        public string Description { get; private set; } = null;

        public string BaseImage { get; private set; } = null;

        public ContainerRegistry BaseImageRegistry { get; private set; } = null;

        internal async Task<DockerImageRequest> ToDockerImageRequestAsync(
            ServiceContext serviceContext,
            string name,
            IEnumerable<Azure.MachineLearning.Services.Models.Model> models,
            Dictionary<string, List<string>> customHeaders,
            CancellationToken cancellationToken)
        {
            var dockerRequest = new DockerImageRequest
            {
                Name = name,
                Description = this.Description,
                TargetRuntime = new TargetRuntime(
                    runtimeType: Runtime,
                    targetArchitecture: ContainerConstants.TargetArchitecture,
                    properties: new Dictionary<string, string>()),
                ImageType = "Docker",
                ImageFlavor = ContainerConstants.ContainerImageFlavor,
                ModelIds = new List<string>(),
                Assets = new List<ImageAsset>(),
            };

            foreach (var model in models)
            {
                dockerRequest.ModelIds.Add(model.Id);
            }

            if (dockerRequest.ModelIds.Count == 0)
            {
                throw new ArgumentException(
                    "No modelIds were specified. At least one model must be specified for the image.");
            }

            await UploadAndConfigureAssetsAsync(
                dockerRequest,
                serviceContext,
                customHeaders,
                cancellationToken).ConfigureAwait(false);

            return dockerRequest;
        }

        private async Task UploadAndConfigureAssetsAsync(
            DockerImageRequest request,
            ServiceContext serviceContext,
            Dictionary<string, List<string>> customHeaders,
            CancellationToken cancellationToken)
        {
            // TODO: Consider doing these steps in parallel. To do this,
            // we would need to lock or use concurrent data structures
            // for some of the fields.
            await AddSdkToRequirementsAsync(
                request,
                serviceContext,
                customHeaders,
                cancellationToken).ConfigureAwait(false);

            if (ExecutionScript != null)
            {
                Depdendencies.Add(new FileInfo(ExecutionScript));
                await ModifyAndUploadExecutionScriptAsync(
                    request,
                    serviceContext,
                    customHeaders,
                    cancellationToken).ConfigureAwait(false);
            }
            if (DockerFile != null)
            {
                await UploadDockerFileAsync(
                    request,
                    serviceContext,
                    customHeaders,
                    cancellationToken).ConfigureAwait(false);
            }

            if (CondaFile != null)
            {
                await UploadCondaFileAsync(
                    request,
                    serviceContext,
                    customHeaders,
                    cancellationToken).ConfigureAwait(false);
            }

            if (SchemaFile != null)
            {
                Depdendencies.Add(new FileInfo(SchemaFile));
            }

            foreach (FileInfo dependency in Depdendencies)
            {
                await UploadDependenciesAsync(
                    request,
                    serviceContext,
                    dependency,
                    customHeaders,
                    cancellationToken).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Adds the Model Management SDK as a requirement to the image. The SDK is used for
        /// schema evaluation and exception handling.
        /// </summary>
        private async Task AddSdkToRequirementsAsync(
            DockerImageRequest dockerImageRequest,
            ServiceContext serviceContext,
            Dictionary<string, List<string>> customHeaders,
            CancellationToken cancellationToken)
        {
            // Create a temp file that contains the SDK requirements.
            // This will be uploaded to blob storage and the local copy is deleted.
            string tempFilePath = Path.Combine(
                Path.GetTempPath(),
                string.Format("requirements{0}.txt", Guid.NewGuid().ToString("N")));

            File.WriteAllText(tempFilePath, ContainerConstants.SdkRequirementsString);

            var artifactOperations = new ArtifactOperations(serviceContext);
            UploadArtifactResult requirementsOperation =
                await artifactOperations.UploadDependencyAsync(
                    new FileInfo(tempFilePath),
                    customHeaders: customHeaders,
                    cancellationToken: cancellationToken).ConfigureAwait(false);

            dockerImageRequest.TargetRuntime.Properties[ContainerConstants.PipRequirements] =
                requirementsOperation.DependencyUri.AbsoluteUri;
        }

        private async Task ModifyAndUploadExecutionScriptAsync(
            DockerImageRequest dockerImageRequest,
            ServiceContext serviceContext,
            Dictionary<string, List<string>> customHeaders,
            CancellationToken cancellationToken)
        {
            Throw.IfFileNotExists(new FileInfo(ExecutionScript));
            var artifactOperations = new ArtifactOperations(serviceContext);

            string tempFilePath = Path.Combine(
                Path.GetTempPath(),
                string.Format("main{0}.py", Guid.NewGuid().ToString("N")));

            await AddExecutionScriptAsAssetAsync(
                dockerImageRequest,
                serviceContext,
                customHeaders,
                cancellationToken).ConfigureAwait(false);

            string templateContents = GetPythonTemplateFromResourceFile();

            string updatedTemplate = templateContents
                .Replace(ExecutionScriptReplacementTags.UserScriptTag, ExecutionScript)
                .Replace(ExecutionScriptReplacementTags.SchemaTag, SchemaFile)
                .Replace(ExecutionScriptReplacementTags.DebugScriptTag, "true");

            File.WriteAllText(tempFilePath, updatedTemplate);

            await artifactOperations.UploadDependencyAsync(
                    new FileInfo(tempFilePath),
                    customHeaders: customHeaders,
                    cancellationToken: cancellationToken).ConfigureAwait(false);
        }

        private async Task UploadDockerFileAsync(
            DockerImageRequest dockerImageRequests,
            ServiceContext serviceContext,
            Dictionary<string, List<string>> customHeaders,
            CancellationToken cancellationToken)
        {
            var artifactOperations = new ArtifactOperations(serviceContext);
            UploadArtifactResult dockerResult =
                await artifactOperations.UploadDependencyAsync(
                    new FileInfo(DockerFile),
                    customHeaders: customHeaders,
                    cancellationToken: cancellationToken).ConfigureAwait(false);
            dockerImageRequests.DockerFileUri = dockerResult.DependencyUri.AbsoluteUri;
        }

        private async Task UploadCondaFileAsync(
            DockerImageRequest dockerImageRequest,
            ServiceContext serviceContext,
            Dictionary<string, List<string>> customHeaders,
            CancellationToken cancellationToken)
        {
            var artifactOperations = new ArtifactOperations(serviceContext);
            UploadArtifactResult condaUri =
                await artifactOperations.UploadDependencyAsync(
                    new FileInfo(CondaFile),
                    customHeaders: customHeaders,
                    cancellationToken: cancellationToken).ConfigureAwait(false);
            dockerImageRequest.TargetRuntime.Properties[ContainerConstants.CondaEnv] =
                condaUri.DependencyUri.AbsoluteUri;
        }

        private async Task UploadDependenciesAsync(
            DockerImageRequest dockerImageRequest,
            ServiceContext serviceContext,
            FileInfo dependency,
            Dictionary<string, List<string>> customHeaders,
            CancellationToken cancellationToken)
        {
            var artifactOperations = new ArtifactOperations(serviceContext);
            UploadArtifactResult uploadDependencyResult =
                await artifactOperations.UploadDependencyAsync(
                    dependency,
                    customHeaders: customHeaders,
                    cancellationToken: cancellationToken).ConfigureAwait(false);
            dockerImageRequest.Assets.Add(
                new ImageAsset
                {
                    Id = uploadDependencyResult.DependencyName,
                    Url = uploadDependencyResult.DependencyUri.AbsoluteUri,
                    MimeType = "application/octet-stream"
                });
        }

        private async Task AddExecutionScriptAsAssetAsync(
            DockerImageRequest dockerImageRequest,
            ServiceContext serviceContext,
            Dictionary<string, List<string>> customHeaders,
            CancellationToken cancellationToken)
        {
            var artifactOperations = new ArtifactOperations(serviceContext);
            UploadArtifactResult executionUploadResult = await artifactOperations.UploadDependencyAsync(
                new FileInfo(ExecutionScript),
                customHeaders: customHeaders,
                cancellationToken: cancellationToken).ConfigureAwait(false);

            dockerImageRequest.Assets.Add(
                new ImageAsset
                {
                    Id = ContainerConstants.DriverId,
                    Url = executionUploadResult.DependencyUri.AbsoluteUri,
                    MimeType = "application/x-python"
                });
            dockerImageRequest.DriverProgram = ContainerConstants.DriverId;
        }

        private void ValidateConfiguration()
        {
            ValidateExecutionFile();

            ValidateRuntime();

            ValidateUserFiles();

            if (Tags.Count != 0)
            {
                throw new NotImplementedException("Image Tags are not yet implemented.");
            }

            if (Properties.Count != 0)
            {
                throw new NotImplementedException("Image Properties are not yet implemented.");
            }
        }

        private void ValidateExecutionFile()
        {
            if (ExecutionScript == null)
            {
                throw new ArgumentException("ExecutionFile must be specified.");
            }

            var executionFile = new FileInfo(ExecutionScript);
            Throw.IfFileNotExists(executionFile);

            if (executionFile.Extension != ".py")
            {
                throw new ArgumentException(
                    string.Format(
                        "File {0} is not a valid execution script. Currently, only Python scripts are supported.",
                        ExecutionScript));
            }
        }

        private void ValidateRuntime()
        {
            if (string.IsNullOrEmpty(Runtime))
            {
                throw new ArgumentException(
                    "If an execution script is provided, then a runtime must also be provided.");
            }

            if (!ContainerConstants.SupportedRuntimes.Contains(Runtime.ToLowerInvariant()))
            {
                throw new ArgumentException(
                    string.Format(
                        "Runtime {0} is not supported. Supported runtimes include: ",
                        Runtime,
                        string.Join(", ", ContainerConstants.SupportedRuntimes)));
            }
        }

        private void ValidateUserFiles()
        {
            if (DockerFile != null)
            {
                Throw.IfFileNotExists(new FileInfo(DockerFile));
            }

            if (CondaFile != null)
            {
                Throw.IfFileNotExists(new FileInfo(CondaFile));
            }

            if (Depdendencies != null)
            {
                foreach (FileInfo dependency in Depdendencies)
                {
                    Throw.IfFileNotExists(dependency);
                }
            }

            if (SchemaFile != null)
            {
                Throw.IfFileNotExists(new FileInfo(SchemaFile));

                if (!IsSchemaInSubdirectory(
                    ExecutionScript,
                    SchemaFile))
                {
                    throw new ArgumentException(
                        "Schema file must be in either the same directory or a subdirectory of the execution script.");
                }
            }
        }

        private bool IsSchemaInSubdirectory(string executionPath, string schemaPath)
        {
            var executionDirectory = new DirectoryInfo(executionPath).Parent;
            var schemaDirectory = new DirectoryInfo(schemaPath).Parent;

            if (executionDirectory.FullName == schemaDirectory.FullName)
            {
                return true;
            }

            while (executionDirectory != null)
            {
                if (executionDirectory.Parent?.FullName == schemaDirectory.FullName)
                {
                    return true;
                }

                executionDirectory = executionDirectory.Parent;
            }

            return false;
        }

        private string GetPythonTemplateFromResourceFile()
        {
            var assembly = Assembly.GetExecutingAssembly();
            using (Stream stream =
                assembly.GetManifestResourceStream("Azure.MachineLearning.Services.main_template.txt"))
            {
                return new StreamReader(stream).ReadToEnd();
            }
        }
    }
}
