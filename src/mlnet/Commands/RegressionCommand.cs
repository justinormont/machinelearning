// <copyright file="RegressionCommand.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.ML.ModelBuilder.AutoMLService;

namespace Microsoft.ML.CLI.Commands
{
    internal class RegressionCommand : AutoMLCommand, ILocalAutoMLTrainParameters
    {
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
                return this.LabelCol.IsSTR ? this.LabelCol.STR : $"col{this.LabelCol.INT.ToString()}";
            }
            set => throw new NotImplementedException();
        }

        public string UserColumn { get ; set; }

        public string ItemColumn { get; set; }

        public string Scenario
        {
            get
            {
                return AutoMLSharedServiceConstants.Regression;
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
                return this.IgnoreCols.Select(x => x.IsSTR ? x.STR : $"col{x.INT.ToString()}");
            }
            set => throw new NotImplementedException();
        }

        public bool IsAzureTraining { get => false; set => throw new NotImplementedException(); }

        public override string CommandName => AutoMLSharedServiceConstants.Regression;

        public override string Description => "Create a custom ML.NET model for regression. ";
    }
}
