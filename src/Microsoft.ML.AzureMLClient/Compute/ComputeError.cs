// <copyright file="ComputeError.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.Linq;

namespace Azure.MachineLearning.Services.Compute
{
    public class ComputeError
    {
        public ComputeError(GeneratedOld.Models.MLCErrorResponse errorResponseDto)
        {
            Throw.IfNull(errorResponseDto, nameof(errorResponseDto));

            this.Code = errorResponseDto.Code;
            this.Message = errorResponseDto.Message;
            if (errorResponseDto.Details != null)
            {
                this.Details = errorResponseDto.Details.Select(
                    x => new ComputeErrorDetail(x)).ToList();
            }
        }

        public string Code { get; private set; }

        public string Message { get; private set; }

        public IList<ComputeErrorDetail> Details { get; private set; }
    }
}
