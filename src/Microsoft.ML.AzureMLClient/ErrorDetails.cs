// <copyright file="ErrorDetails.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace Azure.MachineLearning.Services
{
    public class ErrorDetails
    {
        public ErrorDetails(GeneratedOld.Models.ErrorDetails errorDetails)
        {
            this.Code = errorDetails.Code;
            this.Message = errorDetails.Message;
            this.Target = errorDetails.Target;
        }

        public ErrorDetails(Generated.Models.ErrorDetails errorDetails)
        {
            this.Code = errorDetails.Code;
            this.Message = errorDetails.Message;
            this.Target = errorDetails.Target;
        }

        public string Code { get; private set; }

        public string Message { get; private set; }

        public string Target { get; private set; }
    }
}
