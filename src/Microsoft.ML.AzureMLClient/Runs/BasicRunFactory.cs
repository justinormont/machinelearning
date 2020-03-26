// <copyright file="BasicRunFactory.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

using Azure.MachineLearning.Services.GeneratedOld.Models;

namespace Azure.MachineLearning.Services.Runs
{
    public class BasicRunFactory : IRunFactory
    {
        public Run Create(
            ServiceContext serviceContext,
            string experimentName,
            FactoryManager<IRunFactory> factories,
            RunDto runDto)
        {
            Throw.IfNull(serviceContext, nameof(serviceContext));
            Throw.IfNullOrEmpty(experimentName, nameof(experimentName));
            Throw.IfNull(factories, nameof(factories));
            Throw.IfNull(runDto, nameof(runDto));
            return new Run(serviceContext, experimentName, factories, runDto);
        }
    }
}
