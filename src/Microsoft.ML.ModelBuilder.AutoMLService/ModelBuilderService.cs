// <copyright file="ModelBuilderService.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

using System.Threading;
using System.Threading.Tasks;
using Microsoft.ML.ModelBuilder.AutoMLService.Contract;
using Microsoft.ML.ModelBuilder.AzCopyService.Contract;
using StreamJsonRpc;

namespace Microsoft.ML.ModelBuilder.AutoMLService
{
    public class ModelBuilderService : IModelBuilderService
    {
        private JsonRpc rpc;

        public ModelBuilderService(JsonRpc rpc)
        {
            this.rpc = rpc;
        }

        public async Task<string> RefreshTokenAsync(CancellationToken ct)
        {
            return await this.rpc?.InvokeWithCancellationAsync<string>(nameof(this.RefreshTokenAsync), null, ct);
        }

        public async Task<string> AzCopyDownloadAsync(RemoteSasLocation source, LocalLocation dst, CancellationToken ct)
        {
            return await this.rpc?.InvokeWithCancellationAsync<string>(nameof(this.AzCopyDownloadAsync), new object[] { source, dst }, ct);
        }

        public async Task<bool> ShowYesNoMessageBoxAsync(string title, string content, CancellationToken ct)
        {
            return await this.rpc?.InvokeWithCancellationAsync<bool>(nameof(this.ShowYesNoMessageBoxAsync), new object[] { title, content }, ct);
        }
    }
}
