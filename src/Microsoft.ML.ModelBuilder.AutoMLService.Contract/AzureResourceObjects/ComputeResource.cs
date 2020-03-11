// <copyright file="ComputeResource.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using Microsoft.ML.ModelBuilder.AutoMLService.Contract.AzureResourceObjects;

namespace Microsoft.ML.ModelBuilder.AutoMLService
{
    public class ComputeResource : AzureResource
    {
        public ComputeResourceProperties Properties { get; set; }
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:File may only contain a single type", Justification = "<CodeReadability>")]
    public class ComputeResourceProperties
    {
        public DateTime CreatedOn { get; set; }

        public string ComputeType { get; set; }

        public string ProvisioningState { get; set; }

        public List<WebRequestError> ProvisioningErrors { get; set; }

        public ComputeResourceSubProperties Properties { get; set; }
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:File may only contain a single type", Justification = "<CodeReadability>")]
    public class ComputeResourceSubProperties
    {
        public string VmSize { get; set; }

        public string VmPriority { get; set; }
    }
}
