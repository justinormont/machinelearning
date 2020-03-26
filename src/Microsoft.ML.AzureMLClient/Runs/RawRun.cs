// <copyright file="RawRun.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Rest;

namespace Azure.MachineLearning.Services.Runs
{
    /// <summary>
    /// This class grants access to the wire contract for
    /// submitting a run to the experimentation service (the
    /// controller is hardwired in SubmitRunInternalAsync
    /// and SubmitSnapshotRunInternalAsync).
    /// HIC SVNT DRACONES
    /// </summary>
    public class RawRun : RunConfigurationBase
    {
        public RawRun()
        {
            this.RunDefinition = new RunDefinition();
            this.RunDefinition.Configuration = new RunConfiguration();
        }

        public RunDefinition RunDefinition { get; set; }

        public DirectoryInfo RunDirectory { get; set; }

        public override async Task<Run> SubmitRunAsync(
            RunOperations runOperations,
            Dictionary<string, System.Collections.Generic.List<string>> customHeaders = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            Throw.IfNull(this.RunDefinition, nameof(RunDefinition));

            HttpOperationResponse<GeneratedOld.Models.RunStatus> runStatus = null;
            if (!this.RunDefinition.SnapshotId.HasValue && (this.RunDirectory != null))
            {
                Throw.IfDirectoryNotExists(this.RunDirectory, nameof(RunDirectory));

                byte[] projectBytes = RunHelpers.CreateProjectZip(this.RunDirectory);

                runStatus = await runOperations.SubmitRunInternalAsync(
                    this.RunDefinition,
                    projectBytes,
                    customHeaders,
                    cancellationToken).ConfigureAwait(false);
            }
            else if (this.RunDefinition.SnapshotId.HasValue && (this.RunDirectory == null))
            {
                runStatus = await runOperations.SubmitSnapshotRunInternalAsync(
                    this.RunDefinition, customHeaders, cancellationToken).ConfigureAwait(false);
            }
            else
            {
                throw new ArgumentException("Must specify either SnapshotId or RunDirectory");
            }

            Run run = await runOperations.GetAsync(runStatus.Body.RunId).ConfigureAwait(false);

            return run;
        }
    }
}
