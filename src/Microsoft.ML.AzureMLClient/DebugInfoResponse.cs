// <copyright file="DebugInfoResponse.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

using System.Collections.Generic;

namespace Azure.MachineLearning.Services
{
    public class DebugInfoResponse
    {
        public DebugInfoResponse(GeneratedOld.Models.DebugInfoResponse debugInfoResponse)
        {
            this.Type = debugInfoResponse.Type;
            this.Message = debugInfoResponse.Message;
            this.StackTrace = debugInfoResponse.StackTrace;
            if (debugInfoResponse.InnerException != null)
            {
                this.InnerException = new DebugInfoResponse(debugInfoResponse.InnerException);
            }
            this.Data = debugInfoResponse.Data;
            if (debugInfoResponse.ErrorResponse != null)
            {
                this.ErrorResponse = new ErrorResponse(debugInfoResponse.ErrorResponse);
            }
        }

        /*public DebugInfoResponse(Generated.Models.DebugInfoResponse debugInfoResponse)
        {
            this.Type = debugInfoResponse.Type;
            this.Message = debugInfoResponse.Message;
            this.StackTrace = debugInfoResponse.StackTrace;
            if (debugInfoResponse.InnerException != null)
            {
                this.InnerException = new DebugInfoResponse(debugInfoResponse.InnerException);
            }
            this.Data = debugInfoResponse.Data;
            if (debugInfoResponse.ErrorResponse != null)
            {
                this.ErrorResponse = new ErrorResponse(debugInfoResponse.ErrorResponse);
            }
        }*/

        public string Type { get; private set; }

        public string Message { get; private set; }

        public string StackTrace { get; private set; }

        public DebugInfoResponse InnerException { get; private set; }

        public IDictionary<string, object> Data { get; private set; }

        public ErrorResponse ErrorResponse { get; private set; }
    }
}
