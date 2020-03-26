// <copyright file="IAutoMLServiceLogger.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

using Microsoft.ML.ModelBuilder.AutoMLService.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.ML.ModelBuilder.AutoMLService
{
    internal interface IAutoMLServiceLogger
    {
        event EventHandler<DataReceivedEventArgs> DiagnosticDataReceived;

        AutoMLServiceLogLevel LogLevel { get; set; }

        void Debug(string msg);

        void Info(string msg);

        void Trace(string msg);

        void Warn(string msg);

        void Error(string msg);
    }
}
