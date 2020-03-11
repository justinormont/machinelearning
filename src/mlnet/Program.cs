// <copyright file="Program.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

using System;
using System.CommandLine.Builder;
using System.CommandLine.Invocation;
using System.CommandLine.Parsing;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Microsoft.ML.CLI.Commands;
using Microsoft.ML.CLI.Runners;
using Microsoft.ML.CLI.Telemetry.Events;
using Microsoft.ML.CLI.Utilities;
using Microsoft.ML.ModelBuilder.AutoMLService;
using Microsoft.ML.ModelBuilder.AutoMLService.Contract;
using NLog;
using NLog.Targets;

namespace Microsoft.ML.CLI
{
    public class Program
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public static void Main(string[] args)
        {
            Telemetry.Telemetry.Initialize();
            int exitCode = 1;
            Exception ex = null;
            string logFilePath = null;


            // initialize NLog
            var logconsole = LogManager.Configuration.FindTargetByName("logconsole");
            LogManager.Configuration.AddRule(LogLevel.Info, LogLevel.Fatal, logconsole);

            // set up AutoMLServiceLogger
            AutoMLServiceLogger.Instance.DiagnosticDataReceived += Instance_DiagnosticDataReceived;
            
            var stopwatch = Stopwatch.StartNew();

            var mlNetCommandEvent = new MLNetCommandEvent();

            AutoMLServiceParamater autoMLServiceParamater = default;

            var classificationHandler = CommandHandler.Create<ClassificationCommand>((option) =>
            {
                if (option != null)
                {
                    autoMLServiceParamater = new AutoMLServiceParamater(option);
                    logFilePath = option.LogFilePath;
                }
            });

            var regressionHandler = CommandHandler.Create<RegressionCommand>((option) =>
            {
                if (option != null)
                {
                    autoMLServiceParamater = new AutoMLServiceParamater(option);
                    logFilePath = option.LogFilePath;
                }
            });

            var recommendationHandler = CommandHandler.Create<RecommendationCommand>((option) =>
            {
                if (option != null)
                {
                    autoMLServiceParamater = new AutoMLServiceParamater(option);
                    logFilePath = option.LogFilePath;
                }
            });

            var classificationCommand = new ClassificationCommand().ToCommand(classificationHandler);
            var regressionCommand = new RegressionCommand().ToCommand(regressionHandler);
            var recommendationCommand = new RecommendationCommand().ToCommand(recommendationHandler);

            var parser = new CommandLineBuilder()
                         .AddCommand(classificationCommand)
                         .AddCommand(regressionCommand)
                         .AddCommand(recommendationCommand)
                         .UseDefaults()
                         .Build();

            var res = parser.InvokeAsync(args).Result;

            if (res != 0) // parse fail
            {
                // Flush pending telemetry logs
                Telemetry.Telemetry.Flush(TimeSpan.FromSeconds(3));
                Environment.Exit(exitCode);
            }

            // Send system info telemetry
            SystemInfoEvent.TrackEvent();

            // set log
            var verbosity = Utils.GetVerbosity(autoMLServiceParamater.Verbosity);
            LogManager.Configuration = LogManager.Configuration.Reload();
            logconsole = LogManager.Configuration.FindTargetByName("logconsole");
            LogManager.Configuration.AddRule(verbosity, LogLevel.Fatal, logconsole);
            var logfile = (FileTarget)LogManager.Configuration.FindTargetByName("logfile");
            if (logFilePath != null)
            {
                logfile.FileName = logFilePath;
            }


            try
            {
                var runner = new AutoMLRunner(autoMLServiceParamater);

                // Execute the runner
                runner.Execute();
                exitCode = 0;
            }
            catch (Exception e)
            {
                ex = e;
                logger.Log(LogLevel.Error, e.Message);
                logger.Log(LogLevel.Debug, e.ToString());
                logger.Log(LogLevel.Info, Strings.LookIntoLogFile);
                logger.Log(LogLevel.Error, Strings.Exiting);
            }

            logger.Log(LogLevel.Info, $"{Strings.SeeLogFileForMoreInfo}: {logfile.FileName}");
            // Flush pending telemetry logs
            Telemetry.Telemetry.Flush(TimeSpan.FromSeconds(3));
            Environment.Exit(exitCode);
        }

        private static void Instance_DiagnosticDataReceived(object sender, ModelBuilder.AutoMLService.DataReceivedEventArgs e)
        {
            switch (e.LogLevel)
            {
                case AutoMLServiceLogLevel.INFO:
                    logger.Info(e.Data);
                    break;
                case AutoMLServiceLogLevel.DEBUG:
                    logger.Debug(e.Data);
                    break;
                case AutoMLServiceLogLevel.ERROR:
                    logger.Error(e.Data);
                    break;
                case AutoMLServiceLogLevel.TRACE:
                    logger.Trace(e.Data);
                    break;
                case AutoMLServiceLogLevel.WARN:
                    logger.Warn(e.Data);
                    break;
            }
        }
    }
}