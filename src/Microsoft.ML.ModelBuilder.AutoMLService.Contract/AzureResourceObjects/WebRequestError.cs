// <copyright file="ComputeError.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace Microsoft.ML.ModelBuilder.AutoMLService
{
    public class WebRequestError
    {
        public WebRequestErrorDetails Error { get; set; }
    }

#pragma warning disable SA1402 // File may only contain a single type
    public class WebRequestErrorDetails
#pragma warning restore SA1402 // File may only contain a single type
    {
        public string Code { get; set; }

        public string Message { get; set; }
    }
}
