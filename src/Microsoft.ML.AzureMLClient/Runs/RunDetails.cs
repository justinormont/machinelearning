// <copyright file="RunDetails.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.Linq;

namespace Azure.MachineLearning.Services.Runs
{
    public class RunDetails
    {
        public RunDetails(Run run, GeneratedOld.Models.RunDetailsDto runDetailsDto)
        {
            Throw.IfNull(run, nameof(run));
            Throw.IfNull(runDetailsDto, nameof(runDetailsDto));

            this.Run = run;
            this.Target = runDetailsDto.Target;
            this.Status = runDetailsDto.Status;
            this.StartTimeUtc = runDetailsDto.StartTimeUtc;
            this.EndTimeUtc = runDetailsDto.EndTimeUtc;
            if (runDetailsDto.Error != null)
            {
                this.Error = new ErrorResponse(runDetailsDto.Error);
            }
            if (runDetailsDto.Warnings != null)
            {
                this.Warnings = runDetailsDto.Warnings.Select(x => new RunDetailsWarning(x)).ToList();
            }
            this.Properties = runDetailsDto.Properties;
            // Probably should parse into the raw contract taken from Vienna
            // However see:
            // https://msdata.visualstudio.com/Vienna/_workitems/edit/417982
            this.RunDefinition = runDetailsDto.RunDefinition;
            this.LogFiles = runDetailsDto.LogFiles;
        }

        public Run Run { get; private set; }

        public string Target { get; private set; }

        public string Status { get; private set; }

        public System.DateTime? StartTimeUtc { get; private set; }

        public System.DateTime? EndTimeUtc { get; private set; }

        public ErrorResponse Error { get; private set; }

        public IList<RunDetailsWarning> Warnings { get; private set; }

        public IDictionary<string, string> Properties { get; private set; }

        public object RunDefinition { get; private set; }

        public IDictionary<string, string> LogFiles { get; private set; }
    }
}
