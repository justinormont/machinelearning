using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.ML.ModelBuilder.AutoMLService.Contract.AzureResourceObjects
{
    public class ResourceGroup : AzureResource
    {
        public string Location { get; set; }
    }
}
