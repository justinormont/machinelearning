// <copyright file="ExecutionScriptReplacementTags.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace Azure.MachineLearning.Services.Images
{
    internal static class ExecutionScriptReplacementTags
    {
        // Specify where the schema file should be referenced.
        internal const string SchemaTag = "<schema_file>";

        // Specify where a reference to the user's script should be referenced.
        internal const string UserScriptTag = "<user_script>";

        // Specify if the contianer should use AzureML-specific logging statements.
        internal const string DebugScriptTag = "<log_debug_statements>";
    }
}
