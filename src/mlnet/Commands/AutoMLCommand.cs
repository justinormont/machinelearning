// <copyright file="ClassificationCommand.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

using AngleSharp.Dom.Css;
using Microsoft.ProgramSynthesis.Compound.Split;
using Microsoft.ProgramSynthesis.Utils;
using NLog;
using System.IO;

namespace Microsoft.ML.CLI.Commands
{
    // TODO
    // use Cache type defined in AutoML library
    internal enum CacheType
    {
        On = 0,
        Off = 1,
        Auto = 2,
    }

    /// <summary>
    /// Base Class for Classification/regression/recommendation
    /// </summary>
    internal abstract class AutoMLCommand: MLCommand
    {
        private bool hasHeader = true;
        private NLog.Logger logger = LogManager.GetCurrentClassLogger();

        [CommandArgument("--cache", nameof(Strings.CacheHelperText), CacheType.Auto)]
        public CacheType Cache { get; set; }

        [CommandArgument("--dataset", nameof(Strings.DatasetHelperText), required: true)]
        public FileInfo Dataset { get; set; }

        [CommandArgument("--has-header", nameof(Strings.HeaderHelperText), true)]
        public bool HasHeader
        {
            get
            {
                return hasHeader;
            }
            set
            {
                hasHeader = value;
                if (this.InputFileHelper.HasHeader(this.Dataset.FullName) ^ hasHeader)
                {
                    logger.Warn(string.Format(Strings.HeaderDetectionWarning, (!value).ToString(), value.ToString()));
                }
            }
        }

        [CommandArgument("--test-dataset", nameof(Strings.TestDatasetHelperText))]
        public FileInfo TestDataset { get; set; }

        [CommandArgument("--train-time", nameof(Strings.TrainTimeHelperText), 10)]
        public int TrainTime { get; set; }

        [CommandArgument("--validation-dataset", nameof(Strings.ValidationDatasetHelperText))]
        public FileInfo ValidationDataset { get; set; }

        protected ISimpleDelimiterFileHelper InputFileHelper { get; set; } = SimpleDelimiterFileHelper.Instance;
    }
}
