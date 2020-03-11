// <copyright file="IRemoteAutoMLTrainParameters.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

using Microsoft.ML.ModelBuilder.AzCopyService.Contract;

namespace Microsoft.ML.ModelBuilder.AutoMLService
{
    public interface IRemoteAutoMLTrainParameters : ILocalAutoMLTrainParameters
    {
        string OnnxModelPath { get; set; }

        string OnnxModelLabelPath { get; set; }

        string TaskType { get; set; }

        string SubscriptionId { get; set; }

        string ResourceGroup { get; set; }

        string ExperimentName { get; set; }

        string WorkspaceName { get; set; }

        string ComputeTarget { get; set; }

        string RemoteInputFile { get; set; }

        string RemoteLabelColumn { get; set; }

        RemoteSasLocation TrainArtifactLoaction { get; set; }
    }
}
