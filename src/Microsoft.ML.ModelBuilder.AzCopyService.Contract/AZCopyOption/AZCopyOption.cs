// <copyright file="AZCopyOption.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace Microsoft.ML.ModelBuilder.AzCopyService.Contract
{
    public class AZCopyOption : CommandArgsBase
    {
        [CLIArgumentName("blob-type", true)]
        public string BlobType { get; set; }

        [CLIArgumentName("block-blob-tier", true)]
        public string BlockBlobTier { get; set; }

        [CLIArgumentName("block-size-mb")]
        public string BlockSizeMb { get; set; }

        [CLIArgumentName("cache-control", true)]
        public string CacheControl { get; set; }

        [CLIArgumentName("check-length", false)]
        public string CheckLength { get; set; }

        [CLIArgumentName("check-md5", true)]
        public string CheckMD5 { get; set; }

        [CLIArgumentName("follow-symlinks", false)]
        public string FollowSymlinks { get; set; }

        [CLIArgumentName("recursive", false)]
        public string Recursive { get; set; }

        [CLIArgumentName("include-pattern", true)]
        public string IncludePattern { get; set; }

        [CLIArgumentName("overwrite", true)]
        public string Overwrite { get; set; }
    }
}