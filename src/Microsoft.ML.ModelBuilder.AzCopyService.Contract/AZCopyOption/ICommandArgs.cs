// <copyright file="ICommandArgs.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace Microsoft.ML.ModelBuilder.AzCopyService.Contract
{
    public interface ICommandArgs
    {
        string ToCommandLineString();
    }
}
