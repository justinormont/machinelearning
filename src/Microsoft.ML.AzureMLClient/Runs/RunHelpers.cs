// <copyright file="RunHelpers.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;

namespace Azure.MachineLearning.Services.Runs
{
    public static class RunHelpers
    {
        // SDK is limited to 25 MB
        public const long MaximumZipSizeBytes = 1024 * 1024 * 25;

        // Useful for executing source already in the container
        public static byte[] CreateEmptyZip()
        {
            using (var memoryStream = new MemoryStream())
            {
                using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
                {
                }
                return memoryStream.ToArray();
            }
        }

        public static byte[] CreateProjectZip(DirectoryInfo sourceFolder, List<string> excludedFiles = default(List<string>))
        {
            Throw.IfNull(sourceFolder, nameof(sourceFolder));
            Throw.IfDirectoryNotExists(sourceFolder, nameof(sourceFolder));

            return CreateProjectZip(sourceFolder.ToString(), excludedFiles);
        }

        public static byte[] CreateProjectZip(string sourceFolder, List<string> excludedFiles = default(List<string>))
        {
            Throw.IfNullOrEmpty(sourceFolder, nameof(sourceFolder));
            Throw.IfDirectoryNotExists(sourceFolder, nameof(sourceFolder));

            long folderSize = CalculateFolderSize(sourceFolder);
            if (IsFolderToBig(folderSize))
            {
                var msgFormat = "sourceFolder size of {0} exceeds maximum of {1}.  Use alternate submission instead.";
                var msg = string.Format(msgFormat, folderSize, MaximumZipSizeBytes);
                throw new InvalidOperationException(msg);
            }

            if (!sourceFolder.EndsWith(Path.DirectorySeparatorChar.ToString(), StringComparison.Ordinal))
            {
                sourceFolder += Path.DirectorySeparatorChar;
            }

            using (var memoryStream = new MemoryStream())
            {
                using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
                {
                    foreach (string file in Directory.EnumerateFiles(sourceFolder, "*", SearchOption.AllDirectories)
                        .Select(path => path.Replace(sourceFolder, string.Empty)))
                    {
                        // happily skip over an excluded file
                        if (excludedFiles != null && excludedFiles.Any(f => file.EndsWith(f)))
                        {
                            continue;
                        }

                        // Ensure that directory separators in the zip archive are always '/'
                        // The .NET zip library doesn't mind but Linux libraries tend to be stricter
                        // with the standard
                        string forwardSlashFile = file.Replace(Path.DirectorySeparatorChar, '/');
                        var zipFileEntry = archive.CreateEntry(forwardSlashFile);
                        using (BinaryWriter writer = new BinaryWriter(zipFileEntry.Open()))
                        {
                            writer.Write(File.ReadAllBytes(Path.Combine(sourceFolder, file)));
                        }
                    }
                }
                return memoryStream.ToArray();
            }
        }

        internal static bool IsFolderToBig(long folderSize)
        {
            return folderSize > MaximumZipSizeBytes;
        }

        internal static long CalculateFolderSize(string sourceFolder)
        {
            Throw.IfDirectoryNotExists(sourceFolder, nameof(sourceFolder));

            return Directory.GetFiles(sourceFolder, "*", SearchOption.AllDirectories).Sum(t => new FileInfo(t).Length);
        }
    }
}
