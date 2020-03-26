// <copyright file="AZCopyMessageBase.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace Microsoft.ML.ModelBuilder.AzCopyService.Contract
{
    public abstract class AZCopyMessageBase
    {
        // "Info" | "Init" | "Progress" | "Exit" | "Error" | "Prompt"
        public abstract string MessageType { get; }

        public string MessageContent { get; set; }

        public string TimeStamp { get; set; }

        // should always be empty
        public string PromptDetails { get; set; }
    }
}