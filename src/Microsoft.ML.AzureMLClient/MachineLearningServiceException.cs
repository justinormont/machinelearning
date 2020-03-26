// <copyright file="MachineLearningServiceException.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

using System;

using Microsoft.Rest;

namespace Azure.MachineLearning.Services
{
    public class MachineLearningServiceException : RestException
    {
        public MachineLearningServiceException()
        {
        }

        public MachineLearningServiceException(string message)
            : base(message)
        {
        }

        public MachineLearningServiceException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public HttpRequestMessageWrapper Request { get; set; }

        public HttpResponseMessageWrapper Response { get; set; }

        public ErrorResponse Body { get; set; }

        internal static Exception FromErrorResponseException(Generated.Models.ErrorResponseException errorResponseException)
        {
            var result = new MachineLearningServiceException(
                errorResponseException.Message,
                errorResponseException.InnerException);
            result.Request = errorResponseException.Request;
            result.Response = errorResponseException.Response;
            result.Body = new ErrorResponse(errorResponseException.Body);

            return result;
        }

        internal static MachineLearningServiceException FromErrorResponseException(
            GeneratedOld.Models.ErrorResponseException errorResponseException)
        {
            var result = new MachineLearningServiceException(
                errorResponseException.Message,
                errorResponseException.InnerException);
            result.Request = errorResponseException.Request;
            result.Response = errorResponseException.Response;
            result.Body = new ErrorResponse(errorResponseException.Body);

            return result;
        }
    }
}
