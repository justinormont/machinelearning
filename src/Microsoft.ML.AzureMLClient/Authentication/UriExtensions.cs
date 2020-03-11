// <copyright file="UriExtensions.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

using System;

namespace Azure.MachineLearning.Services.Authentication
{
    internal static class UriExtensions
    {
        /// <summary>
        /// Returns an authority string for URI that is guaranteed to contain
        /// a port number.
        /// </summary>
        /// <param name="uri">The Uri from which to compute the authority</param>
        /// <returns>The complete authority for the Uri</returns>
        public static string FullAuthority(this Uri uri)
        {
            string authority = uri.Authority;

            if (!authority.Contains(":") && uri.Port > 0)
            {
                // Append port for complete authority
                authority = string.Format("{0}:{1}", uri.Authority, uri.Port.ToString());
            }

            return uri.ToString();
        }
    }
}
