// <copyright file="UploadArtifactResult.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

using System;

namespace Azure.MachineLearning.Services.Artifacts
{
    public class UploadArtifactResult
    {
        public Uri DependencyUri { get; set; } = null;

        public string DependencyName { get; set; } = null;
    }
}
