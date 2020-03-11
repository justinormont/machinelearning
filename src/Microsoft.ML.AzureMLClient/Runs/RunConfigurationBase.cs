// <copyright file="RunConfigurationBase.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

using Azure.MachineLearning.Services.Compute;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Azure.MachineLearning.Services.Runs
{
    public abstract class RunConfigurationBase
    {
        private int nodeCount;
        private Version pythonVer;
        private TimeSpan? maxRunTime;

        public ComputeTarget ComputeTarget { get; set; }

        public Version PythonVersion
        {
            get
            {
                return this.pythonVer;
            }

            set
            {
                if (value < RunConstants.MinPythonVersion)
                {
                    var msg = string.Format("Must have Python version >= {0}", RunConstants.MinPythonVersion.ToString());
                    throw new ArgumentOutOfRangeException(msg);
                }

                this.pythonVer = value;
            }
        }

        public bool AutoPrepareEnvironment { get; set; }

        public bool UserManagedDependencies { get; set; }

        public string InterpreterPath { get; set; }

        public List<string> PipPackages { get; set; } = new List<string>();

        public List<string> Arguments { get; set; } = new List<string>();

        public FileInfo ScriptFile { get; set; }

        public Guid? ParentSnapshotId { get; set; }

        public Dictionary<string, DataReferenceConfiguration> DataReferences { get; set; } = new Dictionary<string, DataReferenceConfiguration>();

        public EnvironmentDefinition.DockerSection DockerConfiguration { get; set; }

        public Dictionary<string, string> EnvironmentVariables { get; set; } = new Dictionary<string, string>();

        public int NodeCount
        {
            get
            {
                return this.nodeCount;
            }

            set
            {
                if (value < 1)
                {
                    throw new ArgumentOutOfRangeException("nodeCount must be positive");
                }
                this.nodeCount = value;
            }
        }

        public TimeSpan? MaximumRunDuration
        {
            get
            {
                return this.maxRunTime;
            }

            set
            {
                if (value <= TimeSpan.FromSeconds(0))
                {
                    throw new ArgumentOutOfRangeException("Must set MaximumRunDuration to be positive");
                }
                this.maxRunTime = value;
            }
        }

        // Each type of run (Estimator, AutoML etc.) needs to know how to
        // submit itself to the service
        public abstract Task<Run> SubmitRunAsync(
            RunOperations runOperations,
            Dictionary<string, List<string>> customHeaders = null,
            CancellationToken cancellationToken = default(CancellationToken));

        protected RunConfiguration ConstructRunConfiguration()
        {
            var rc = new RunConfiguration();

            if (string.IsNullOrEmpty(this.InterpreterPath))
            {
                throw new MachineLearningServiceException("InterpreterPath can't be null");
            }

            rc.Script = this.ScriptFile?.Name;
            rc.Arguments = this.Arguments;

            rc.DataReferences = this.DataReferences;

            rc.Environment = new EnvironmentDefinition();
            rc.Environment.Docker = this.DockerConfiguration;

            rc.Environment.EnvironmentVariables = this.EnvironmentVariables;

            rc.Framework = Framework.Python;
            rc.Environment.Python.UserManagedDependencies = true;
            rc.Environment.Python = new EnvironmentDefinition.PythonSection();
            rc.Environment.Python.InterpreterPath = this.InterpreterPath;
            rc.Environment.Python.UserManagedDependencies = this.UserManagedDependencies;
            rc.Environment.Python.CondaDependencies = this.ConstructCondaDependenciesJSON();

            rc.History = new HistoryConfiguration();
            rc.History.OutputCollection = true;

            rc.Target = this.ComputeTarget.Name;
            rc.NodeCount = 1;

            if (this.MaximumRunDuration.HasValue)
            {
                rc.MaxRunDurationSeconds = (int)this.MaximumRunDuration.Value.TotalSeconds;
            }

            rc.AutoPrepareEnvironment = this.AutoPrepareEnvironment;

            return rc;
        }

        protected void SetPythonDefaults()
        {
            this.PythonVersion = RunConstants.DefaultPythonVersion;

            this.DockerConfiguration = new EnvironmentDefinition.DockerSection();
            this.DockerConfiguration.Enabled = true;
            this.DockerConfiguration.BaseImage = RunConstants.DefaultDockerImage;
            this.DockerConfiguration.BaseImageRegistry = new ContainerRegistry();

            this.UserManagedDependencies = false;
            this.InterpreterPath = RunConstants.DefaultPythonInterpeterPath;
            this.AutoPrepareEnvironment = true;
            this.NodeCount = 1;
        }

        private JObject ConstructCondaDependenciesJSON()
        {
            string _condaDependencies = @"
            {
                'name': 'project_environment',
                'dependencies': [
                    'python=3.6.2',
                    {
                        'pip': [
                            '--index-url https://azuremlsdktestpypi.azureedge.net/sdk-release/master/588E708E0DF342C4A80BD954289657CF',
                            '--extra-index-url https://pypi.python.org/simple',
                            'azureml-sdk[automl]==0.1.0.3603131.*',
                            'pytorch-ignite',
                            'pretrainedmodels'
                        ]
                    },
                    'numpy',
                    'torch',
                    'torchvision'
                ],
                'channels': [
                'conda-forge'
                ]
            }";

            var condaDeps = JsonConvert.DeserializeObject<JToken>(_condaDependencies);

            return (JObject)condaDeps;
        }        
    }
}