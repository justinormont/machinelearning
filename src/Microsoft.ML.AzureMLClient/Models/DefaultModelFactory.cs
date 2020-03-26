// <copyright file="DefaultModelFactory.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace Azure.MachineLearning.Services.Models
{
    public class DefaultModelFactory : IModelFactory
    {
        public Model Create(ServiceContext serviceContext, GeneratedOld.Models.Model modelDto)
        {
            Throw.IfNull(serviceContext, nameof(serviceContext));
            Throw.IfNull(modelDto, nameof(modelDto));
            return new DefaultModel(serviceContext, modelDto);
        }
    }
}
