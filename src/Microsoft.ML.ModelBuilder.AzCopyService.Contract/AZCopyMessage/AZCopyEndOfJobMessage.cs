﻿// <copyright file="AZCopyEndOfJobMessage.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

using System.Collections.Generic;

namespace Microsoft.ML.ModelBuilder.AzCopyService.Contract
{
    public class AZCopyEndOfJobMessage : AZCopyMessageBase
    {
        // For StreamJson-RPC only
        public AZCopyEndOfJobMessage()
        {
        }

        public override string MessageType => "EndOfJob";

        public string ErrorMsg { get; set; }

        public string JobStatus { get; set; }

        public int TotalTransfers { get; set; }

        public int TransfersCompleted { get; set; }

        public int TransfersFailed { get; set; }

        public int TransfersSkipped { get; set; }

        public double BytesOverWire { get; set; }

        public double TotalBytesTransferred { get; set; }

        public double TotalBytesEnumerated { get; set; }

        public double TotalBytesExpected { get; set; }

        public float PercentComplete { get; set; }

        public List<FailedTransfer> FailedTransfers { get; set; }

        public List<FailedTransfer> SkippedTransfers { get; set; }
    }
}
