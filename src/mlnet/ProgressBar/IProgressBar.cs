// <copyright file="IProgressBar.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

using System;

namespace Microsoft.ML.CLI.ShellProgressBar
{
    public interface IProgressBar : IDisposable
    {
        int MaxTicks { get; set; }

        string Message { get; set; }

        double Percentage { get; }

        int CurrentTick { get; }

        ConsoleColor ForeGroundColor { get; }

        void Tick(string message = null);

        ChildProgressBar Spawn(int maxTicks, string message, ProgressBarOptions options = null);
    }
}
