// <copyright file="AZCopyClient.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

using Microsoft.ML.ModelBuilder.AzCopyService.Contract;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.ML.ModelBuilder.AzCopyService
{
    public class AZCopyClient : IAZCopyClient
    {
        private Process process = default;

        public event EventHandler<AZCopyMessageBase> JobStatusHandler;

        public async Task CopyAsync(IAZCopyLocation src, IAZCopyLocation dst, AZCopyOption option, CancellationToken ct = default)
        {
            // Local location must be in quotes. It could have spaces in the name and the CLI would interpret as separate parameters.
            var args = $"copy \"{src.LocationToString()}\" {dst.LocationToString()} {option.ToCommandLineString()} --output-type=json --cancel-from-stdin";
            await this.StartAZCopy(args, ct);
        }

        public async Task DeleteAsync(IAZCopyLocation dst, AZDeleteOption option, CancellationToken ct = default)
        {
            var args = $"rm {dst.LocationToString()} {option.ToCommandLineString()} --output-type=json --cancel-from-stdin";
            await this.StartAZCopy(args, ct);
        }

        private static string GetAzCopyPath()
        {
            var assemblyFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string azCopyPath = Path.Combine(assemblyFolder, "azcopy.exe");
            return azCopyPath;
        }

        private async Task StartAZCopy(string args, CancellationToken ct = default, Dictionary<string, string> envs = default)
        {
            string azCopyPath = GetAzCopyPath();
            var procInfo = new ProcessStartInfo(azCopyPath);
            procInfo.Arguments = args;
            procInfo.RedirectStandardOutput = true;
            procInfo.RedirectStandardError = true;
            procInfo.RedirectStandardInput = true;

            if (envs != null)
            {
                foreach (var kv in envs)
                {
                    procInfo.Environment.Add(kv);
                }
            }

            // set Environment Info for OAuth Location
            // only for test output
            procInfo.UseShellExecute = false;
            await Task.Run(() =>
            {
                this.process = Process.Start(procInfo);

                // cancellation
                ct.Register(() => this.process.StandardInput.WriteLine("cancel"));

                this.process.OutputDataReceived += this.Process_OutputDataReceived;
                this.process.ErrorDataReceived += this.Process_OutputDataReceived;

                this.process.BeginOutputReadLine();
                this.process.BeginErrorReadLine();

                this.process.WaitForExit();
            });
        }

        private void Process_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (e.Data != null)
            {
                var message = AZCopyMessageFactory.CreateFromJson(e.Data);
                this.JobStatusHandler?.Invoke(sender, message);
            }
        }
    }
}
