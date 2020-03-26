// <copyright file="WorkspaceInformation.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

using Microsoft.ML.ModelBuilder.AutoMLService.Contract.AzureResourceObjects;

namespace Microsoft.ML.ModelBuilder.AutoMLService
{
    public class WorkspaceInformation : AzureResource
    {
        public Sku Sku { get; set; }
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:File may only contain a single type", Justification = "<Pending>")]
    public class Sku
    {
        public string Name { get; set; }

        public string Tier { get; set; }

        public string Size { get; set; }

        public string Family { get; set; }

        public string Model { get; set; }

        public int? Capacity { get; set; }
    }
}
