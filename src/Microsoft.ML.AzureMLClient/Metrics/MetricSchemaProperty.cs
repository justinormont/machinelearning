// <copyright file="MetricSchemaProperty.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

using Azure.MachineLearning.Services.GeneratedOld.Models;

namespace Azure.MachineLearning.Services.Metrics
{
    public class MetricSchemaProperty
    {
        public MetricSchemaProperty(MetricSchemaPropertyDto metricSchemaPropertyData)
        {
            this.PropertyId = metricSchemaPropertyData.PropertyId;
            this.Name = metricSchemaPropertyData.Name;
            this.Type = metricSchemaPropertyData.Type;
        }

        public string PropertyId { get; private set; }

        public string Name { get; private set; }

        public string Type { get; private set; }
    }
}
