// <copyright file="RunDefinition.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Text;

/*
 * This file is copied from
 * Vienna/src/azureml-api/src/Execution/Contracts/RunDefinition.cs
 * This is a temporary solution until we can get the Swagger files updated
 * */

namespace Azure.MachineLearning.Services
{
    public class RunDefinition
    {
        /// <summary>
        /// Fully specified configuration information for the run. Even when that information
        /// is contained in configuration files within the project folder, the client collapses
        /// it all and inlines it into the run definition when submitting a run.
        /// </summary>
        public RunConfiguration Configuration { get; set; }

        /// <summary>
        /// Snapshots are user project folders that have been uploaded to the cloud for subsequent
        /// execution. This field is required when executing against cloud-based compute targets
        /// unless the run submission was against the API endpoint that takes a zipped project folder
        /// inline with the request.
        /// </summary>
        public Guid? SnapshotId { get; set; }

        /// <summary>
        /// Specifies that the run history entry for this execution should be scoped within
        /// an existing run as a child. Defaults to null, meaning the run has no parent.
        /// This is intended for first-party service integration, not third-party API users.
        /// </summary>
        /// <example>myexperiment_155000000001_0</example>
        public string ParentRunId { get; set; }

        /// <summary>
        /// Specifies the runsource property for this run. The default value is "experiment" if not specified.
        /// </summary>
        /// <example>experiment</example>
        public string RunType { get; set; }

        // Field for internal telemetry to track what component started this run.
        // Example values are "Aether", "HyperDrive", or "AutoML".
        // Not intended for users to ever set; this should not get documented.
        public string Attribution { get; set; }

        // Field for tracking client side telemetry related to this run. Because sending telemetry
        // from the python SDK or CLI is fraught with peril, we collect interesting information
        // on the client and then bundle it into the next appropriate service request for logging.
        // Not intended for users to ever set; this should not get documented.
        public Dictionary<string, object> TelemetryValues { get; set; }
    }
}
