// <copyright file="IAZCopyServiceChannel.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

using System;

namespace Microsoft.ML.ModelBuilder.AzCopyService.Contract
{
    public interface IAZCopyServiceChannel
    {
        event EventHandler<AZCopyEndOfJobMessage> AZCopyEndOfJobHandler;

        event EventHandler<AZCopyErrorMessage> AZCopyErrorMessageHandler;

        event EventHandler<AZCopyInfoMessage> AZCopyInfoMessageHandler;

        event EventHandler<AZCopyInitMessage> AzCopyInitMessageHandler;

        event EventHandler<AZCopyProgressMessage> AzCopyProgressMessageHandler;
    }
}