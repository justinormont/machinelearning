// <copyright file="DefaultModel.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace Azure.MachineLearning.Services.Models
{
    public class DefaultModel : Model
    {
        public DefaultModel(ServiceContext serviceContext, GeneratedOld.Models.Model modelDto)
            : base(serviceContext, modelDto)
        {
            // Nothing to do
        }
    }
}
