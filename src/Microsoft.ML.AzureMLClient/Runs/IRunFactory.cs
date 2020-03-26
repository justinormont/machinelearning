// <copyright file = "IRunFactory.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace Azure.MachineLearning.Services.Runs
{
    public interface IRunFactory
    {
        Run Create(
            ServiceContext serviceContext,
            string experimentName,
            FactoryManager<IRunFactory> factories,
            GeneratedOld.Models.RunDto runDto);
    }
}
