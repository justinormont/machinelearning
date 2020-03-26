// <copyright file="AutoMLConfiguration.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Azure.MachineLearning.Services.Runs;

namespace Azure.MachineLearning.Services.AutoML
{
    public class AutoMLConfiguration : RunConfigurationBase
    {
        public AutoMLConfiguration(
            AutoMLSettings autoMLSettings,
            string task,
            DirectoryInfo projectFolder = null)
        {
            Throw.IfNull(autoMLSettings, nameof(autoMLSettings));
            Throw.IfNullOrEmpty(task, nameof(task));

            if (projectFolder != null)
            {
                Throw.IfDirectoryNotExists(projectFolder, nameof(projectFolder));
                CheckPathForDataScript(projectFolder);
            }

            AutoMLSettings = autoMLSettings;
            Task = task;
            Path = projectFolder;

            SetPythonDefaults();
            ValidateTask();
        }

        public AutoMLSettings AutoMLSettings { get; private set; }

        public string Task { get; private set; }

        public DirectoryInfo Path { get; private set; }

        public FileInfo DataScript { get; private set; }

        public override async Task<Run> SubmitRunAsync(
            RunOperations runOps,
            Dictionary<string, List<string>> customHeaders = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var autoMLOperations = new AutoMLOperations(runOps.ServiceContext);

            string parentRunId = await autoMLOperations.CreateParentRunAsync(
                this,
                runOps.ExperimentName,
                customHeaders,
                cancellationToken).ConfigureAwait(false);

            await autoMLOperations.PostRemoteSnapshotRunAsync(
                parentRunId,
                runOps.ExperimentName,
                this,
                customHeaders,
                cancellationToken).ConfigureAwait(false);

            var runOperations = new RunOperations(runOps.ServiceContext, runOps.ExperimentName);
            return await runOperations.GetAsync(parentRunId, customHeaders, cancellationToken).ConfigureAwait(false);
        }

        internal RunDefinition BuildChildRunDefinition(string parentRunId, Guid? snapshotId)
        {
            var runDef = new RunDefinition
            {
                Configuration = ConstructRunConfiguration(),
                ParentRunId = parentRunId,
                Attribution = "AutoML",
                SnapshotId = snapshotId,
            };

            return runDef;
        }

        private void CheckPathForDataScript(DirectoryInfo projectFolder)
        {
            var fileToCheck = System.IO.Path.Combine(projectFolder.FullName, "get_data.py");

            if (!File.Exists(fileToCheck))
            {
                throw new FileNotFoundException(
                    "The data script \"get_data.py\" must be present in the specified directory.");
            }
        }

        private void ValidateTask()
        {
            if (Task.ToLowerInvariant() == "classification")
            {
                if (!AutoMLMetricConstants.ClassificationMetrics.Contains(AutoMLSettings.PrimaryMetric))
                {
                    throw new ArgumentException($"Primary metric {AutoMLSettings.PrimaryMetric} is not a valid classification metric.");
                }
            }
            else if (Task.ToLowerInvariant() == "regression")
            {
                if (!AutoMLMetricConstants.RegressionMetrics.Contains(AutoMLSettings.PrimaryMetric))
                {
                    throw new ArgumentException($"Primary metric {AutoMLSettings.PrimaryMetric} is not a valid regression metric.");
                }
            }
            else
            {
                throw new ArgumentException($"Task type {Task} is not a valid task for an AutoMLConfiguration.");
            }
        }
    }
}