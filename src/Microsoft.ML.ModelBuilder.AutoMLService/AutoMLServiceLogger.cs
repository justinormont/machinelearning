// <copyright file="AutoMLServiceLogger.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

using Microsoft.ML.ModelBuilder.AutoMLService.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.ML.ModelBuilder.AutoMLService
{
    internal class AutoMLServiceLogger : IAutoMLServiceLogger
    {
        private static readonly IAutoMLServiceLogger logger = new AutoMLServiceLogger();

        public event EventHandler<DataReceivedEventArgs> DiagnosticDataReceived;

        private AutoMLServiceLogger() { }

        public static IAutoMLServiceLogger Instance
        {
            get => logger;
        }

        public AutoMLServiceLogLevel LogLevel { get; set; } = AutoMLServiceLogLevel.INFO;

        public void Debug(string msg)
        {
            this.Log(AutoMLServiceLogLevel.DEBUG, msg);
        }

        public void Error(string msg)
        {
            this.Log(AutoMLServiceLogLevel.ERROR, msg);
        }

        public void Info(string msg)
        {
            this.Log(AutoMLServiceLogLevel.INFO, msg);
        }

        public void Trace(string msg)
        {
            this.Log(AutoMLServiceLogLevel.TRACE, msg);
        }

        public void Warn(string msg)
        {
            this.Log(AutoMLServiceLogLevel.WARN, msg);
        }

        private void Log(AutoMLServiceLogLevel loglevel, string msg)
        {
            if (loglevel <= this.LogLevel)
            {
                this.DiagnosticDataReceived?.Invoke(this, new DataReceivedEventArgs()
                {
                    LogLevel = loglevel,
                    Data = msg,
                });
            }
        }
    }
}
