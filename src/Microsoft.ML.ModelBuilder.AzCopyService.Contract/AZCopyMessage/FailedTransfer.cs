// <copyright file="FailedTransfer.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace Microsoft.ML.ModelBuilder.AzCopyService.Contract
{
    /// <summary>
    /// type for MessageContent.FailedTransfers and MessageContent.SkippedTransfers.
    /// </summary>
    public class FailedTransfer
    {
        public string Src { get; set; }

        public string Dst { get; set; }

        public string TransferStatus { get; set; }

        public int ErrorCode { get; set; }
    }
}
