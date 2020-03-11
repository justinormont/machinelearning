// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.DotNet.Cli.Telemetry;
using Microsoft.ML.CLI.Utilities;
using Microsoft.ML.ModelBuilder.AutoMLService.Contract;

namespace Microsoft.ML.CLI.Telemetry.Events
{
    internal class MLNetCommandEvent
    {
        public AutoMLServiceParamater AutoTrainCommandSettings { get; set; }

        public IEnumerable<string> CommandLineParametersUsed { get; set; }

        public void TrackEvent()
        {
            Telemetry.TrackEvent(
                "mlnet-command",
                new Dictionary<string, string>
                {
                    { "CommandLineParametersUsed", string.Join(",", this.CommandLineParametersUsed) },
                    { "FilenameHash", HashFilename(this.AutoTrainCommandSettings.InputFile) },
                    { "FileSizeBucket", GetFileSizeBucketStr(new FileInfo(this.AutoTrainCommandSettings.InputFile)) },
                    { "HasHeader", this.AutoTrainCommandSettings.HasHeader.ToString() },
                    { "IgnoredColumnsCount", this.AutoTrainCommandSettings.IgnoredColumnNames.ToList().Count.ToString() },
                    { "LearningTaskType", this.AutoTrainCommandSettings.Scenario },
                    { "MaxExplorationTime", this.AutoTrainCommandSettings.TrainTime.ToString() },
                    { "ValidFilenameHash", HashFilename(this.AutoTrainCommandSettings.ValidateFile) },
                    { "ValidFileSizeBucket", GetFileSizeBucketStr(new FileInfo(this.AutoTrainCommandSettings.ValidateFile)) },
                    { "TestFilenameHash", HashFilename(this.AutoTrainCommandSettings.TestFile) },
                    { "TestFileSizeBucket", GetFileSizeBucketStr(new FileInfo(this.AutoTrainCommandSettings.TestFile)) },
                });
        }

        private static string HashFilename(string filename)
        {
            return string.IsNullOrEmpty(filename) ? null : Sha256Hasher.Hash(filename);
        }

        private static double CalcFileSizeBucket(FileInfo fileInfo)
        {
            return Math.Pow(2, Math.Ceiling(Math.Log(fileInfo.Length, 2)));
        }

        private static string GetFileSizeBucketStr(FileInfo fileInfo)
        {
            if (fileInfo == null || !fileInfo.Exists)
            {
                return null;
            }
            return CalcFileSizeBucket(fileInfo).ToString();
        }
    }
}
