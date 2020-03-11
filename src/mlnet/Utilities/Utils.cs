// <copyright file="Utils.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

using System;
using System.IO;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Formatting;
using Microsoft.ML.AutoML;
using Microsoft.ML.Data;
using Microsoft.ML.ModelBuilder.AutoMLService.Contract;
using Microsoft.ProgramSynthesis.Compound.Split;
using Microsoft.ProgramSynthesis.Compound.Split.Constraints;
using NLog;

namespace Microsoft.ML.CLI.Utilities
{
    internal class Utils
    {
        internal static LogLevel GetVerbosity(AutoMLServiceLogLevel verbosity)
        {
            switch (verbosity)
            {
                case AutoMLServiceLogLevel.ERROR:
                    return LogLevel.Error;
                case AutoMLServiceLogLevel.WARN:
                    return LogLevel.Warn;
                case AutoMLServiceLogLevel.DEBUG:
                    return LogLevel.Debug;
                case AutoMLServiceLogLevel.INFO:
                    return LogLevel.Info;
                case AutoMLServiceLogLevel.TRACE:
                    return LogLevel.Trace;
                default:
                    return LogLevel.Info;
            }
        }

        // Steal from Model Builder
        internal static ProgramProperties GetDataProperties(string fileName)
        {
            // Create a new learning session
            var session = new Session();

            // Add a constraint to learn CSV or FW program
            session.Constraints.Add(new SimpleDelimiterOrFixedWidth());

            // Add a constraint for time limit
            session.Constraints.Add(new TimeLimit(TimeSpan.FromSeconds(10.0)));

            Microsoft.ProgramSynthesis.Compound.Split.Program program;
            using (var file = File.OpenText(fileName))
            {
                // Add inputs to learn from (number of lines to learn from is optional and defaults to 200)
                session.AddInput(file, linesToRead: 200);

                // Learn a program
                program = session.Learn();
                if (program is null)
                {
                    var msg = string.Format(Strings.TextFileError, fileName);
                    throw new Exception(msg);
                }
            }

            return program.Properties;
        }

        internal static CacheBeforeTrainer GetCacheSettings(string input)
        {
            switch (input)
            {
                case "on": return CacheBeforeTrainer.On;
                case "off": return CacheBeforeTrainer.Off;
                case "auto": return CacheBeforeTrainer.Auto;
                default:
                    throw new ArgumentException($"{nameof(input)} is invalid", nameof(input));
            }
        }
    }
}
