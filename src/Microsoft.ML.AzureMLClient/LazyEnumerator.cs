// <copyright file="LazyEnumerator.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

using System.Collections;
using System.Collections.Generic;

namespace Azure.MachineLearning.Services
{
    public class LazyEnumerator<T> : IEnumerable<T>
    {
        public LazyEnumerator()
        {
            this.Cache = new List<T>();
        }

        public IPageFetcher<T> Fetcher { get; set; }

        public List<T> Cache { get; set; }

        public IEnumerator<T> GetEnumerator()
        {
            // We maintain an internal cache so that the IEnumerable can be
            // evaluated more than once without needing to make a fresh
            // set of REST calls
            foreach (var c in this.Cache)
            {
                yield return c;
            }

            while (!this.Fetcher.OnLastPage)
            {
                var nextPage = this.Fetcher.FetchNextPage();
                this.Cache.AddRange(nextPage);

                foreach (var item in nextPage)
                {
                    yield return item;
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
