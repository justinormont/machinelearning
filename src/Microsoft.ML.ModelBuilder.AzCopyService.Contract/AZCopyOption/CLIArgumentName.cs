// <copyright file="CLIArgumentName.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace Microsoft.ML.ModelBuilder.AzCopyService.Contract
{
    public class CLIArgumentName : System.Attribute
    {
        public CLIArgumentName(string name, bool useQuotes = false)
        {
            this.ArgumentName = name;
            this.UseQuotes = useQuotes;
        }

        public string ArgumentName { get; private set; }

        public bool UseQuotes { get; private set; }
    }
}
