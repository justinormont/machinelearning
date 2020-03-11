// <copyright file="InnerErrorResponse.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace Azure.MachineLearning.Services
{
    public class InnerErrorResponse
    {
        public InnerErrorResponse(GeneratedOld.Models.InnerErrorResponse innerErrorResponse)
        {
            this.Code = innerErrorResponse.Code;
            if (innerErrorResponse.InnerError != null)
            {
                this.InnerError = new InnerErrorResponse(innerErrorResponse.InnerError);
            }
        }

        public InnerErrorResponse(Generated.Models.InnerErrorResponse innerErrorResponse)
        {
            this.Code = innerErrorResponse.Code;
            if (innerErrorResponse.InnerError != null)
            {
                this.InnerError = new InnerErrorResponse(innerErrorResponse.InnerError);
            }
        }

        public string Code { get; private set; }

        public InnerErrorResponse InnerError { get; private set; }
    }
}
