// <copyright file="RunDetailsWarning.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace Azure.MachineLearning.Services.Runs
{
    public class RunDetailsWarning
    {
        public RunDetailsWarning(GeneratedOld.Models.RunDetailsWarningDto rdw)
        {
            this.Source = rdw.Source;
            this.Message = rdw.Message;
        }

        public string Source { get; private set; }

        public string Message { get; private set; }
    }
}
