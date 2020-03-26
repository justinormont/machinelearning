// <copyright file = "IModelFactory.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace Azure.MachineLearning.Services.Models
{
    public interface IModelFactory
    {
        Model Create(ServiceContext serviceContext, GeneratedOld.Models.Model modelDto);
    }
}
