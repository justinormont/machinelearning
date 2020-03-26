// <copyright file="IAZCopyChannel.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

using Microsoft.ML.ModelBuilder.AzCopyService.Contract;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.ML.ModelBuilder.AzCopyService
{
    internal interface IAZCopyChannel
    {
        event EventHandler<AZCopyMessageBase> JobStatusHandler;
    }
}
