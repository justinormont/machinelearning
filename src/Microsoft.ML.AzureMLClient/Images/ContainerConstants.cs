// <copyright file="ContainerConstants.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

using System.Collections.Generic;

namespace Azure.MachineLearning.Services.Images
{
    internal static class ContainerConstants
    {
        internal const string ContainerImageFlavor = "WebApiContainer";

        internal const string PipRequirements = "pipRequirements";

        internal const string CondaEnv = "condaEnvFile";

        internal const string DriverId = "driver";

        internal const string TargetArchitecture = "Amd64";

        internal const string SdkRequirementsString = "azureml-model-management-sdk==1.0.1b6.post1";

        internal static List<string> SupportedRuntimes { get; } = new List<string> { "python", "python-slim" };
    }
}
