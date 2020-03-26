// <copyright file="AutoMLServiceParamater.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

using System.Collections.Generic;
using Microsoft.ML.ModelBuilder.AzCopyService.Contract;

namespace Microsoft.ML.ModelBuilder.AutoMLService.Contract
{
    public class AutoMLServiceParamater : IAutoMLServiceParamater
    {

        // For ServiceHub
        public AutoMLServiceParamater() { }

        public AutoMLServiceParamater(ILocalAutoMLTrainParameters paramater)
        {
            var properties = typeof(ILocalAutoMLTrainParameters).GetProperties();
            foreach (var property in properties)
            {
                var value = property.GetValue(paramater);
                property.SetValue(this, value);
            }
        }

        public bool IsAzureTraining { get; set; }

        public string OnnxModelPath { get; set; }

        public string OnnxModelLabelPath { get; set; }

        public string TaskType { get; set; }

        public string SubscriptionId { get; set; }

        public string ResourceGroup { get; set; }

        public string ExperimentName { get; set; }

        public string WorkspaceName { get; set; }

        public string ComputeTarget { get; set; }

        public string InputFile { get; set; }

        // test file going to used in CodeGenerator setting
        public string TestFile { get; set; }

        public string ValidateFile { get; set; }

        public string LabelColumn { get; set; }

        public string UserColumn { get; set; }

        public string ItemColumn { get; set; }

        public string Scenario { get; set; }

        public string TempOutputDirectory { get; set; }

        public int TrainTime { get; set; }

        public string Name { get; set; }

        public AutoMLServiceLogLevel Verbosity { get; set; }

        public string ModelPath { get; set; }

        public IEnumerable<string> IgnoredColumnNames { get; set; } = new List<string>();

        public string RemoteInputFile { get; set; }

        public string RemoteLabelColumn { get; set; }

        public string StablePackageVersion { get; set; }

        public string UnstablePackageVersion { get; set; }

        public RemoteSasLocation TrainArtifactLoaction { get; set; }

        public bool HasHeader { get; set; }
    }
}
