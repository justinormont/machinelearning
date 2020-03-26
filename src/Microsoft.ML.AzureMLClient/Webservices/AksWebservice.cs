// <copyright file="AksWebservice.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

using Azure.MachineLearning.Services.GeneratedOld.Models;

namespace Azure.MachineLearning.Services.Webservices
{
    public class AksWebservice : Webservice
    {
        public AksWebservice(
            ServiceContext serviceContext,
            AKSServiceResponse serviceResponse,
            string operationId = null)
        {
            Throw.IfNull(serviceContext, nameof(serviceContext));
            Throw.IfNull(serviceResponse, nameof(serviceResponse));

            ServiceContext = serviceContext;
            Name = serviceResponse.Name;
            ServiceId = serviceResponse.Id;
            ScoringUri = serviceResponse.ScoringUri;
            OperationId = operationId;
        }

        public string ScoringUri { get; private set; }
    }
}
