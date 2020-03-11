// <copyright file="SimpleScriptRun.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

using Azure.MachineLearning.Services.Snapshots;

namespace Azure.MachineLearning.Services.Runs
{
    public class SimpleScriptRun : RunConfigurationBase
    {
        public SimpleScriptRun()
        {
            SetPythonDefaults();
        }

        public override async Task<Run> SubmitRunAsync(
            RunOperations runOperations,
            Dictionary<string, List<string>> customHeaders = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            Throw.IfFileNotExists(this.ScriptFile);
            long folderSize = RunHelpers.CalculateFolderSize(this.ScriptFile.DirectoryName);
            bool isSnapshotRun = RunHelpers.IsFolderToBig(folderSize);

            var runConfig = this.ConstructRunConfiguration();

            var runDef = new RunDefinition();
            runDef.Configuration = runConfig;

            Microsoft.Rest.HttpOperationResponse<GeneratedOld.Models.RunStatus> runStatus = null;
            if (isSnapshotRun)
            {
                var snapshotClient = new SnapshotOperations(runOperations.ServiceContext);
                Guid snapshotId = await snapshotClient.SnapshotDirectoryAsync(
                    this.ScriptFile.Directory,
                    parentSnapshotId: this.ParentSnapshotId,
                    customHeaders: customHeaders,
                    cancellationToken: cancellationToken).ConfigureAwait(false);
                runDef.SnapshotId = snapshotId;
                runStatus = await runOperations.SubmitSnapshotRunInternalAsync(runDef, customHeaders, cancellationToken).ConfigureAwait(false);
            }
            else
            {
                byte[] projectBytes = this.GetZipFileBytes();
                runStatus = await runOperations.SubmitRunInternalAsync(
                    runDef,
                    projectBytes,
                    customHeaders,
                    cancellationToken).ConfigureAwait(false);
            }

            Run run = await runOperations.GetAsync(runStatus.Body.RunId, customHeaders, cancellationToken).ConfigureAwait(false);

            return run;
        }

        private byte[] GetZipFileBytes()
        {
            var parentDir = new DirectoryInfo(this.ScriptFile.DirectoryName);
            byte[] zipBytes = RunHelpers.CreateProjectZip(parentDir);

            return zipBytes;
        }
    }
}
