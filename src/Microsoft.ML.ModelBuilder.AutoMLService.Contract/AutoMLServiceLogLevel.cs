// <copyright file="AutoMLServiceLogLevel.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.ML.ModelBuilder.AutoMLService.Contract
{
    public enum AutoMLServiceLogLevel
    {
        ERROR = 0,
        WARN = 1,
        INFO = 2,
        DEBUG = 3,
        TRACE = 4,
    }
}
