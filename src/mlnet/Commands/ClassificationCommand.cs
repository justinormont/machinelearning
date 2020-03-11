// <copyright file="ClassificationCommand.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Parsing;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.ML.ModelBuilder.AutoMLService;
using Microsoft.ML.ModelBuilder.AutoMLService.Contract;

namespace Microsoft.ML.CLI.Commands
{
    internal class ClassificationCommand : AutoMLCommand, ILocalAutoMLTrainParameters
    {
        public override string CommandName => "classification";

        public override string Description => "Create a custom ML.NET model for classification";

        [CommandArgument("--ignore-cols", nameof(Strings.IgnoreColsHelperText))]
        public IEnumerable<IntOrString> IgnoreCols { get; set; } = new List<IntOrString>();

        [CommandArgument("--label-col", nameof(Strings.LabelColHelperText), null, required: true)]
        public IntOrString LabelCol { get; set; }

        public string InputFile
        {
            get
            {
                return this.Dataset?.FullName;
            }
            set => throw new NotImplementedException();
        }

        public string TestFile
        {
            get
            {
                return this.TestDataset?.FullName;
            }
            set => throw new NotImplementedException();
        }

        public string ValidateFile
        {
            get
            {
                return this.ValidationDataset?.FullName;
            }
            set => throw new NotImplementedException();
        }

        public string LabelColumn
        {
            get
            {
                var rawColumns = this.InputFileHelper.RawColumnNames(this.InputFile);
                return this.LabelCol.IsSTR ? this.LabelCol.STR : rawColumns[this.LabelCol.INT];
            }
            set => throw new NotImplementedException();
        }

        public string UserColumn { get; set; }

        public string ItemColumn { get; set; }

        public string Scenario
        {
            get
            {
                return AutoMLSharedServiceConstants.MulticlassClassification;
            }
            set => throw new NotImplementedException();
        }

        public string TempOutputDirectory
        {
            get
            {
                return Path.Combine(this.Output?.FullName, this.Name);
            }
            set => throw new NotImplementedException();
        }

        public string ModelPath
        {
            get
            {
                return Path.Combine(this.Output?.FullName, this.Name, $"{this.Name}.Model", "MLModel.zip");
            }
            set => throw new NotImplementedException();
        }

        public IEnumerable<string> IgnoredColumnNames
        {
            get
            {
                var rawColumns = this.InputFileHelper.RawColumnNames(this.InputFile);
                return this.IgnoreCols.Select(x => x.IsSTR? x.STR: rawColumns[x.INT]);
            }
            set => throw new NotImplementedException();
        }

        public bool IsAzureTraining { get => false; set => throw new NotImplementedException(); }
    }
}
