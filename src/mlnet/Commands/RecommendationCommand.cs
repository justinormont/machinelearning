// <copyright file="RecommendationCommand.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.ML.ModelBuilder.AutoMLService;

namespace Microsoft.ML.CLI.Commands
{
    internal class RecommendationCommand : AutoMLCommand, ILocalAutoMLTrainParameters
    {
        public override string CommandName => "recommend";

        public override string Description => "Create a custom ML.NET model for recommendation.";

        [CommandArgument("--item-col", nameof(Strings.ItemColHelperText))]
        public IntOrString ItemCol { get; set; }

        [CommandArgument("--rating-col", nameof(Strings.RatingColHelperText))]
        public IntOrString RatingCol { get; set; }

        [CommandArgument("--user-col", nameof(Strings.UserColHelperText))]
        public IntOrString UserCol { get; set; }

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
                return this.RatingCol.IsSTR? this.RatingCol.STR : $"col{this.RatingCol.INT.ToString()}";
            }
            set => throw new NotImplementedException();
        }

        public string UserColumn
        {
            get
            {
                return this.UserCol.IsSTR ? this.UserCol.STR : $"col{this.UserCol.INT.ToString()}";
            }
            set => throw new NotImplementedException();
        }

        public string ItemColumn
        {
            get
            {
                return this.ItemCol.IsSTR ? this.ItemCol.STR : $"col{this.ItemCol.INT.ToString()}";
            }
            set => throw new NotImplementedException();
        }

        public string Scenario
        {
            get
            {
                return AutoMLSharedServiceConstants.Recommendation;
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

        // TODO
        // Update this to the other columns except user, item and rating
        public IEnumerable<string> IgnoredColumnNames { get; set; } = new List<string>();

        public bool IsAzureTraining { get => false; set => throw new NotImplementedException(); }
    }
}
