// <copyright file="RunConstants.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

using System;

namespace Azure.MachineLearning.Services.Runs
{
    internal static class RunConstants
    {
        internal static string DefaultDockerImage => "mcr.microsoft.com/azureml/base:0.2.3";

        internal static string DefaultCondaEnvironmentName => "project_environment";

        internal static string DefaultPythonInterpeterPath => "python";

        internal static string AutoMLDiscriminator => "automl";

        // Notes on Python versions can be seen at
        // https://docs.microsoft.com/en-us/azure/machine-learning/service/setup-create-workspace#create-an-isolated-python-environment
        internal static Version MinPythonVersion => new Version(3, 5, 2);

        internal static Version DefaultPythonVersion => new Version(3, 6, 5);
    }
}
