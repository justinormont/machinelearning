// <copyright file="RemoteRunStartedEventArgs.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

using System;

namespace Microsoft.ML.ModelBuilder.AutoMLService.Contract
{
    public class RemoteRunStartedEventArgs : EventArgs
    {
        public string RemoteURL { get; set; }
    }
}
