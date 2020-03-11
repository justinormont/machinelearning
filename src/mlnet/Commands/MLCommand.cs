using Microsoft.ML.ModelBuilder.AutoMLService.Contract;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Microsoft.ML.CLI.Commands
{
    /// <summary>
    /// abstract class for Machine Learning Command
    /// </summary>
    internal abstract class MLCommand : CommandBase
    {
        [CommandArgument(new string[] { "-v", "--verbosity" }, nameof(Strings.VerbosityHelperText), AutoMLServiceLogLevel.INFO)]
        public AutoMLServiceLogLevel Verbosity { get; set; }

        [CommandArgument("--log-file-path", nameof(Strings.LogFilePathHelperText))]
        public string LogFilePath { get; set; }

        /// <summary>
        /// Experiment Name
        /// </summary>
        [CommandArgument("--name", nameof(Strings.ClassificationNameHelperText), "SampleClassification")]
        public string Name { get; set; }

        /// <summary>
        /// Output location
        /// </summary>
        [CommandArgument(new string[] { "--output", "-o" }, nameof(Strings.OutputHelperText))]
        public DirectoryInfo Output { get; set; } = new DirectoryInfo(Directory.GetCurrentDirectory());

        /// <summary>
        /// CodeGen StablePackageVersion
        /// </summary>
        public string StablePackageVersion
        {
            get => "1.5.0-preview";
            set => throw new NotImplementedException();
        }

        /// <summary>
        /// CodeGen UnstablePackageVersion
        /// </summary>
        public string UnstablePackageVersion
        {
            get => "0.17.0-preview";
            set => throw new NotImplementedException();
        }
    }
}
