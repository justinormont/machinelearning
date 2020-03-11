// <copyright file="WebserviceDeploymentConfigurationBase.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

using Azure.MachineLearning.Services.Compute;
using Azure.MachineLearning.Services.GeneratedOld.Models;

namespace Azure.MachineLearning.Services.Webservices
{
    public abstract class WebserviceDeploymentConfigurationBase
    {
        internal abstract ServiceCreateRequest ToServiceCreateRequest(
            string name,
            ComputeTarget computeTarget,
            Images.Image image);
    }
}
