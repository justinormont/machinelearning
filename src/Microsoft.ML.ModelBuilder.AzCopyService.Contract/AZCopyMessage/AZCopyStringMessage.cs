// <copyright file="AZCopyStringMessage.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace Microsoft.ML.ModelBuilder.AzCopyService.Contract
{
    public class AZCopyStringMessage : AZCopyMessageBase
    {
        public AZCopyStringMessage(string message)
        {
            this.MessageContent = message;
        }

        public override string MessageType => "StringError";
    }
}
