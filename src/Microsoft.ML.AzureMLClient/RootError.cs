// <copyright file="RootError.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.Linq;

namespace Azure.MachineLearning.Services
{
    public class RootError
    {
        public RootError(GeneratedOld.Models.RootError rootErrorDto)
        {
            this.Code = rootErrorDto.Code;
            this.Message = rootErrorDto.Message;
            this.Target = rootErrorDto.Target;
            if (rootErrorDto.Details != null)
            {
                this.Details = rootErrorDto.Details.Select(x => new ErrorDetails(x)).ToList();
            }
            if (rootErrorDto.InnerError != null)
            {
                this.InnerError = new InnerErrorResponse(rootErrorDto.InnerError);
            }
            if (rootErrorDto.DebugInfo != null)
            {
                this.DebugInfo = new DebugInfoResponse(rootErrorDto.DebugInfo);
            }
        }

        public RootError(Generated.Models.RootError rootErrorDto)
        {
            this.Code = rootErrorDto.Code;
            this.Message = rootErrorDto.Message;
            this.Target = rootErrorDto.Target;
            if (rootErrorDto.Details != null)
            {
                this.Details = rootErrorDto.Details.Select(x => new ErrorDetails(x)).ToList();
            }
            if (rootErrorDto.InnerError != null)
            {
                this.InnerError = new InnerErrorResponse(rootErrorDto.InnerError);
            }
            /*if (rootErrorDto.DebugInfo != null)
            {
                this.DebugInfo = new DebugInfoResponse(rootErrorDto.DebugInfo);
            }*/
        }

        public string Code { get; private set; }

        public string Message { get; private set; }

        public string Target { get; private set; }

        public IList<ErrorDetails> Details { get; private set; }

        public InnerErrorResponse InnerError { get; private set; }

        public DebugInfoResponse DebugInfo { get; private set; }
    }
}
