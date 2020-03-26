// <copyright file="FilterBuilder.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;

namespace Azure.MachineLearning.Services.Utils
{
    public static class FilterBuilder
    {
        public static string BuildRunMetricFilter(IEnumerable<string> runIds)
        {
            Throw.IfNull(runIds, nameof(runIds));

            if (runIds.Count() == 0)
            {
                throw new ArgumentException("No runIds were specified for the requested filter.");
            }

            if (runIds.Any(runId => string.IsNullOrEmpty(runId)))
            {
                throw new ArgumentException("Received null runId.");
            }

            var modifiedRunIds = runIds.Select(runId => $"RunId eq {runId}");
            return string.Join(" or ", modifiedRunIds);
        }

        public static string BuildRunMetricFilter(string runId)
        {
            Throw.IfNullOrEmpty(runId, nameof(runId));
            return BuildRunMetricFilter(new List<string>() { runId });
        }
    }
}
