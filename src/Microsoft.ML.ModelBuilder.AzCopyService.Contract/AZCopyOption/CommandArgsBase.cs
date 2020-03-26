﻿// <copyright file="CommandArgsBase.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Microsoft.ML.ModelBuilder.AzCopyService.Contract
{
    public class CommandArgsBase : ICommandArgs
    {
        /// <summary>
        /// Builds a set of command-line flags for the training parameters, in the form "--flagname value --flagname2 value 2".
        /// </summary>
        /// <returns>serialized CLI command.</returns>
        public string ToCommandLineString()
            => string.Join(" ", this.GetType().GetProperties().Select(p => this.BuildFlagForProperty(p)).Where(p => p != null));

        private string BuildFlagForProperty(PropertyInfo property)
        {
            if (property.IsCommandLineArgument())
            {
                var flagValue = property.GetFlagValue(this);
                if (flagValue == null)
                {
                    return null;
                }

                // for set flag case
                // avoid adding whitespace to the end
                if (flagValue == string.Empty)
                {
                    return property.GetPropertyAsCLIFlag();
                }

                return string.Join(" ", property.GetPropertyAsCLIFlag(), flagValue);
            }

            return null;
        }
    }
}
