// <copyright file="IAutoMLTrainParameters.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

using Microsoft.ML.ModelBuilder.AutoMLService.Contract;
using System.Collections.Generic;

namespace Microsoft.ML.ModelBuilder.AutoMLService
{
    public interface ILocalAutoMLTrainParameters
    {
        string InputFile { get; set; }

        // test file going to used in CodeGenerator setting
        string TestFile { get; set; }

        string ValidateFile { get; set; }

        string LabelColumn { get; set; }

        // only for recommendation Scenario
        string UserColumn { get; set; }

        // only for recommendation Scenario
        string ItemColumn { get; set; }

        string Scenario { get; set; }

        string TempOutputDirectory { get; set; }

        int TrainTime { get; set; }

        string Name { get; set; }

        AutoMLServiceLogLevel Verbosity { get; set; }

        string ModelPath { get; set; }

        /// <summary>
        /// Gets or sets for CodeGen's StablePackageVersion.
        /// </summary>
        string StablePackageVersion { get; set; }

        /// <summary>
        /// Gets or sets for CodeGen's UnStablePackageVersion.
        /// </summary>
        string UnstablePackageVersion { get; set; }

        /// <summary>
        /// Gets or sets the names of the columns that should be ignored for training.
        /// </summary>
        IEnumerable<string> IgnoredColumnNames { get; set; }

        bool IsAzureTraining { get; set; }

        bool HasHeader { get; set; }
    }
}
