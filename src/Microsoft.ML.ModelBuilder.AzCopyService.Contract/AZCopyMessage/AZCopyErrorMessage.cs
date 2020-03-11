// <copyright file="AZCopyErrorMessage.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace Microsoft.ML.ModelBuilder.AzCopyService.Contract
{
    public sealed class AZCopyErrorMessage : AZCopyMessageBase
    {
        // For StreamJson-RPC only
        public AZCopyErrorMessage()
        {
        }

        public override string MessageType => "Error";
    }
}
