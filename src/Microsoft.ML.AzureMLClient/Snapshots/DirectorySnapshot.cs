// <copyright file="DirectorySnapshot.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Azure.MachineLearning.Services.Snapshots
{
    public static class DirectorySnapshot
    {
        public static GeneratedOld.Models.FlatDirTreeNodeListDto HashDirectoryTree(DirectoryInfo rootDirectory)
        {
            Throw.IfDirectoryNotExists(rootDirectory, nameof(rootDirectory));
            var result = new GeneratedOld.Models.FlatDirTreeNodeListDto();
            result.Files = new List<GeneratedOld.Models.FlatDirTreeNode>();

            var directoryStack = new Stack<Tuple<string, DirectoryInfo>>();
            int nextId = 0;
            directoryStack.Push(Tuple.Create<string, DirectoryInfo>(null, rootDirectory));
            do
            {
                var current = directoryStack.Pop();

                var nextDir = new GeneratedOld.Models.FlatDirTreeNode();
                nextDir.Id = nextId.ToString();
                nextDir.ParentId = current.Item1;
                nextDir.IsFile = false;
                nextDir.Name = current.Item2.Name;
                result.Files.Add(nextDir);

                var dirList = new List<DirectoryInfo>(current.Item2.GetDirectories());
                dirList.Sort((x, y) => string.Compare(x.Name, y.Name));
                foreach (var d in dirList)
                {
                    directoryStack.Push(Tuple.Create(nextDir.Id, d));
                }

                nextId++;
                var fileList = new List<FileInfo>(current.Item2.GetFiles());
                fileList.Sort((x, y) => string.Compare(x.Name, y.Name));
                foreach (var f in fileList)
                {
                    var nextFile = new GeneratedOld.Models.FlatDirTreeNode();
                    nextFile.Name = f.Name;
                    nextFile.ParentId = nextDir.Id;
                    nextFile.Id = nextId.ToString();
                    nextFile.IsFile = true;
                    nextFile.Hash = DirectorySnapshot.ComputeFileHash(f);
                    result.Files.Add(nextFile);
                    nextId++;
                }
            }
            while (directoryStack.Count != 0);

            return result;
        }

        public static string ComputeFileHash(FileInfo fileToHash)
        {
            string result = string.Empty;
            using (var myHasher = SHA512.Create())
            using (FileStream stream = fileToHash.Open(FileMode.Open, FileAccess.Read))
            {
                stream.Position = 0;
                byte[] hashBytes = myHasher.ComputeHash(stream);
                var sb = new StringBuilder();
                foreach (byte b in hashBytes)
                {
                    sb.AppendFormat("{0:x2}", b);
                }
                result = sb.ToString();
            }
            return result;
        }
    }
}
