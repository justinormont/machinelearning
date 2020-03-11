// <copyright file="HttpBearerChallengeCache.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

using System;
using System.Collections.Concurrent;

namespace Azure.MachineLearning.Services.Authentication
{
    /// <summary>
    /// Singleton class for handling caching of the http bearer challenge
    /// </summary>
    public sealed class HttpBearerChallengeCache
    {
        private static HttpBearerChallengeCache _instance = new HttpBearerChallengeCache();
        private ConcurrentDictionary<string, HttpBearerChallenge> _cache = null;
        private object _cacheLock = null;

        private HttpBearerChallengeCache()
        {
            _cache = new ConcurrentDictionary<string, HttpBearerChallenge>();
            _cacheLock = new object();
        }

        /// <summary>
        /// Gets the singleton instance of <see cref="HttpBearerChallengeCache"/>
        /// </summary>
        /// <returns>Instance of this class</returns>
        public static HttpBearerChallengeCache GetInstance()
        {
            return _instance;
        }

        /// <summary>
        /// Gets the challenge for the cached URL.
        /// </summary>
        /// <param name="uri"> the URL that the challenge is cached for.</param>
        /// <returns>the cached challenge or null otherwise.</returns>
        public HttpBearerChallenge GetChallengeForURL(Uri uri)
        {
            if (uri == null)
            {
                throw new ArgumentNullException(nameof(uri));
            }

            HttpBearerChallenge value = null;

            lock (_cacheLock)
            {
                _cache.TryGetValue(uri.FullAuthority(), out value);
            }

            return value;
        }

        /// <summary>
        /// Removes the cached challenge for the specified URL
        /// </summary>
        /// <param name="uri"> the URL to remove its cached challenge </param>
        public void RemoveChallengeForURL(Uri uri)
        {
            if (uri == null)
            {
                throw new ArgumentNullException(nameof(uri));
            }

            HttpBearerChallenge httpBearerChallenge = null;
            lock (_cacheLock)
            {
                _cache.TryRemove(uri.FullAuthority(), out httpBearerChallenge);
            }
        }

        /// <summary>
        /// Caches the challenge for the specified URL
        /// </summary>
        /// <param name="uri"> URL corresponding to challenge as cache key </param>
        /// <param name="value"> the challenge </param>
        public void SetChallengeForURL(Uri uri, HttpBearerChallenge value)
        {
            if (uri == null)
            {
                throw new ArgumentNullException("url");
            }

            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            if (string.Compare(uri.FullAuthority(), value.SourceAuthority, StringComparison.OrdinalIgnoreCase) != 0)
            {
                throw new ArgumentException("Source URL and Challenge URL do not match");
            }

            lock (_cacheLock)
            {
                _cache[uri.FullAuthority()] = value;
            }
        }

        /// <summary>
        /// Clears the cache
        /// </summary>
        public void Clear()
        {
            lock (_cacheLock)
            {
                _cache.Clear();
            }
        }
    }
}
