// <copyright file="ErrorResponse.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

using System.Collections.Generic;

namespace Azure.MachineLearning.Services
{
    public class ErrorResponse
    {
        public ErrorResponse(GeneratedOld.Models.ErrorResponse errorResponse)
        {
            if (errorResponse.Error != null)
            {
                this.Error = new RootError(errorResponse.Error);
            }
            this.Correlation = errorResponse.Correlation;
        }

        public ErrorResponse(Generated.Models.ErrorResponse errorResponse)
        {
            if (errorResponse.Error != null)
            {
                this.Error = new RootError(errorResponse.Error);
            }
            this.Correlation = errorResponse.Correlation;
        }

        public RootError Error { get; private set; }

        public IDictionary<string, string> Correlation { get; private set; }
    }
}
