// <copyright file="RunMetric.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;

using Azure.MachineLearning.Services.GeneratedOld.Models;

namespace Azure.MachineLearning.Services.Metrics
{
    public class RunMetric : IRefreshableFromDto<RunMetricDto>
    {
        public RunMetric(RunMetricDto runMetricDto)
        {
            Throw.IfNull(runMetricDto, nameof(runMetricDto));
            this.RefreshFromDto(runMetricDto);
        }

        public string RunId { get; private set; }

        public Guid? MetricId { get; private set; }

        public string MetricType { get; private set; }

        public DateTime? CreatedUtc { get; private set; }

        public string Name { get; set; }

        public string Description { get; private set; }

        public string Label { get; private set; }

        public string DataLocation { get; private set; }

        public int? NumCells { get; set; }

        public IList<IDictionary<string, object>> Cells { get; private set; }

        public MetricSchema Schema { get; private set; }

        public DateTime LastRefreshFromDto { get; private set; }

        public void RefreshFromDto(RunMetricDto runMetricDto)
        {
            this.RunId = runMetricDto.RunId;
            this.MetricId = runMetricDto.MetricId;
            this.MetricType = runMetricDto.MetricType;
            this.CreatedUtc = runMetricDto.CreatedUtc;
            this.Name = runMetricDto.Name;
            this.Description = runMetricDto.Description;
            this.Label = runMetricDto.Label;
            this.DataLocation = runMetricDto.DataLocation;

            this.NumCells = runMetricDto.NumCells;
            this.Cells = runMetricDto.Cells;

            if (runMetricDto.Schema != null)
            {
                this.Schema = new MetricSchema(runMetricDto.Schema);
            }

            this.LastRefreshFromDto = DateTime.UtcNow;
        }
    }
}
