// <copyright file="FailureCounter.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Azure.MachineLearning.Services.Experiments;
using Azure.MachineLearning.Services.Runs;
using Azure.MachineLearning.Services.Workspaces;
using Microsoft.ML.ModelBuilder.AutoMLService.Contract;

namespace AzureML
{
    internal class FailureCounter
    {
        private readonly int maxFailures;
        private int failureCount;

        public FailureCounter(int maxFailures)
        {
            this.maxFailures = maxFailures;
        }

        public void RecordFailure(string description)
        {
            if (++this.failureCount >= this.maxFailures)
            {
                throw new Exception(description);
            }
        }

        public void RecordFailure(Exception ex)
        {
            if (++this.failureCount >= this.maxFailures)
            {
                throw ex;
            }
        }

        public async Task RecordFailureWithRetryWarningAsync(Exception ex, IModelBuilderService modelBuilderService, CancellationToken ct)
        {
            if (++this.failureCount >= this.maxFailures)
            {
                // TODO
                // This is just a simple workaround to add retry option, considering moving it into TrainStatus( add retry button there) in the future
                var content = $"A network issue has been detected. Please check your internet and try again. If this issue continues, please select \"No\" and report this issue.";
                if (!await modelBuilderService.ShowYesNoMessageBoxAsync("Polling Error", content, ct))
                {
                    // throw error
                    throw ex;
                }
                else
                {
                    // do nothing
                    return;
                }
            }
        }
    }
}
