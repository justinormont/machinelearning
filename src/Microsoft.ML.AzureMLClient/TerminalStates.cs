// <copyright file="TerminalStates.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

using System.Collections.Generic;

namespace Azure.MachineLearning.Services
{
    public static class TerminalStates
    {
        public static readonly List<string> States =
            new List<string> { "completed", "canceled", "cancelled", "failed", "succeeded" };
    }
}
