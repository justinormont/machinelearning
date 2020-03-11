// <copyright file="IAZCopyClient.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

using Microsoft.ML.ModelBuilder.AzCopyService.Contract;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.ML.ModelBuilder.AzCopyService
{
    internal interface IAZCopyClient : IAZCopyChannel
    {
        /// <summary>
        /// azcopy copy from local to remote.
        /// </summary>
        /// <param name="src">copy src.</param>
        /// <param name="dst">copy dst.</param>
        /// <param name="option">copy option.</param>
        /// <param name="ct">cancellation token.</param>
        /// <returns>empty task.</returns>
        Task CopyAsync(IAZCopyLocation src, IAZCopyLocation dst, AZCopyOption option, CancellationToken ct);

        /// <summary>
        /// azcopy delete blablabla.
        /// </summary>
        /// <param name="dst">delete src.</param>
        /// <param name="option">delete option.</param>
        /// <returns>empty task.</returns>
        Task DeleteAsync(IAZCopyLocation dst, AZDeleteOption option, CancellationToken ct);
    }
}
