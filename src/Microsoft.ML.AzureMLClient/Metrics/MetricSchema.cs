// <copyright file="MetricSchema.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.Linq;

using Azure.MachineLearning.Services.GeneratedOld.Models;

namespace Azure.MachineLearning.Services.Metrics
{
    public class MetricSchema
    {
        public MetricSchema(MetricSchemaDto metricSchemaDTO)
        {
            this.NumProperties = metricSchemaDTO.NumProperties;
            if (metricSchemaDTO.Properties != null)
            {
                this.Properties = metricSchemaDTO.Properties.Select(x => new MetricSchemaProperty(x)).ToList();
            }
        }

        public int? NumProperties { get; set; }

        public IList<MetricSchemaProperty> Properties { get; set; }
    }
}
