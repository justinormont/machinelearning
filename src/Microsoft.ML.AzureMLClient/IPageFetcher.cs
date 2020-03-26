// <copyright file="IPageFetcher.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Azure.MachineLearning.Services
{
    public interface IPageFetcher<T>
    {
        bool OnLastPage { get; }

        IEnumerable<T> FetchNextPage();

        Task<IEnumerable<T>> FetchNextPageAsync(
            Dictionary<string, List<string>> customHeaders = null,
            CancellationToken cancellationToken = default(CancellationToken));
    }
}
