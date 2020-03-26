// <copyright file="DefaultWebservice.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

using Azure.MachineLearning.Services.GeneratedOld.Models;

namespace Azure.MachineLearning.Services.Webservices
{
    public class DefaultWebservice : Webservice
    {
        public DefaultWebservice(
            ServiceContext serviceContext,
            ServiceResponseBase serviceResponse,
            string operationId = null)
        {
            Throw.IfNull(serviceContext, nameof(serviceContext));
            Throw.IfNull(serviceResponse, nameof(serviceResponse));

            ServiceContext = serviceContext;
            Name = serviceResponse.Name;
            ServiceId = serviceResponse.Id;
            OperationId = operationId;
        }

        public string ScoringUri { get; private set; }
    }
}
