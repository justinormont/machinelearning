// <copyright file="IModelBuilderService.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

using System.Threading;
using System.Threading.Tasks;
using Microsoft.ML.ModelBuilder.AzCopyService.Contract;

namespace Microsoft.ML.ModelBuilder.AutoMLService.Contract
{
    public interface IModelBuilderService : ITokenRefresh, IAZCopyDownload
    {
        Task<bool> ShowYesNoMessageBoxAsync(string title, string content, CancellationToken ct);
    }

    public interface ITokenRefresh
    {
        Task<string> RefreshTokenAsync(CancellationToken ct);
    }

    public interface IAZCopyDownload
    {
        Task<string> AzCopyDownloadAsync(RemoteSasLocation source, LocalLocation dst, CancellationToken ct);
    }
}
