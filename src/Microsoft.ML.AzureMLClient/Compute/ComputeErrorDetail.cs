// <copyright file="ComputeErrorDetail.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace Azure.MachineLearning.Services.Compute
{
    public class ComputeErrorDetail
    {
        public ComputeErrorDetail(GeneratedOld.Models.MLCErrorDetail errorDetail)
        {
            Throw.IfNull(errorDetail, nameof(errorDetail));

            this.Code = errorDetail.Code;
            this.Message = errorDetail.Message;
        }

        public string Code { get; private set; }

        public string Message { get; private set; }
    }
}
