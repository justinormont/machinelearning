// <copyright file="CommandArgumentAttribute.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Linq;
using Tensorflow;

namespace Microsoft.ML.CLI.Commands
{
    internal class CommandArgumentAttribute : Attribute
    {
        public CommandArgumentAttribute(string[] alias, string helperText, object defaultValue = null, bool required = false)
        {
            this.Alias = alias.ToList();
            this.HelperText = helperText;
            this.DefaultValue = defaultValue;
            this.Required = required;
        }

        public CommandArgumentAttribute(string alias, string helperText, object defaultValue = null, bool required = false)
        {
            this.Alias = new List<string>() { alias };
            this.HelperText = this.GetHelperText(helperText);
            this.DefaultValue = defaultValue;
            this.Required = required;
        }

        public List<string> Alias { get; set; }

        public string HelperText { get; set; }

        public object DefaultValue { get; set; }

        public bool Required { get; set; }

        public string GetHelperText(string name)
        {
            try
            {
                string helpText = typeof(Strings).GetProperty(name, System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic).GetValue(new Strings()) as string;
                return helpText;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }
    }
}
