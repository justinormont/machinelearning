// <copyright file="Throw.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

using System;
using System.IO;

namespace Azure.MachineLearning.Services
{
    public static class Throw
    {
        public static void IfNull<T>(T target, string name)
        {
            if (target == null)
            {
                throw new ArgumentNullException(name);
            }
        }

        public static void IfNullOrEmpty(string target, string name)
        {
            Throw.IfNull(target, name);

            if (string.IsNullOrEmpty(target))
            {
                var message = string.Format("String must not be empty");
                throw new ArgumentException(message, name);
            }
        }

        public static void IfDirectoryNotExists(string target, string name)
        {
            if (!Directory.Exists(target))
            {
                var message = string.Format("Folder {0} specified in {1} does not exist.", target, name);
                throw new DirectoryNotFoundException(message);
            }
        }

        public static void IfValueNotPositive(int? target, string name)
        {
            if (target != null && target <= 0)
            {
                throw new ArgumentException(
                    string.Format("Invalid value. {0} must be greater than 0.", name));
            }
        }

        public static void IfDirectoryNotExists(DirectoryInfo target, string name)
        {
            if (!target.Exists)
            {
                var message = string.Format("Folder {0} specified in {1} does not exist.", target, name);
                throw new DirectoryNotFoundException(message);
            }
        }

        public static void IfFileNotExists(FileInfo target)
        {
            if (!target.Exists)
            {
                var message = string.Format("File {0} does not exist", target.FullName);
                throw new FileNotFoundException(message);
            }
        }
    }
}