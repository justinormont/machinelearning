// <copyright file="AZCopyInfoMessage.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace Microsoft.ML.ModelBuilder.AzCopyService.Contract
{
    public class AZCopyInfoMessage : AZCopyMessageBase
    {
        // For StreamJson-RPC only
        public AZCopyInfoMessage()
        {
        }

        public override string MessageType { get => "Info"; }
    }
}
