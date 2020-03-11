// <copyright file="GroupAttribute.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

using System;

namespace Microsoft.ML.CLI.Commands
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    internal class GroupAttribute : Attribute
    {
        public GroupAttribute(bool required)
        {
            this.Required = required;
        }

        public GroupAttribute(int id, bool required = false)
        {
            this.ID = id;
            this.Required = required;
        }

        /// <summary>
        /// Gets or sets ID for Argument.
        /// RequiredArguemnts with the same ID should appear exactly once in total.
        /// Default is 0.
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether gets or Sets Required For Argument.
        /// If true, then arguments in the same group must appear exactly one time in total.
        /// If false, then arguments in the same group must appear less or equal to one time in total.
        /// </summary>
        public bool Required { get; set; }
    }
}
