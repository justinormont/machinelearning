// <copyright file="IAZCopyLocation.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.ML.ModelBuilder.AzCopyService.Contract
{
    public interface IAZCopyLocation
    {
        string Path { get; set; }

        bool UseWildCard { get; set; }

        string LocationToString();
    }
}
