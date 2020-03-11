// <copyright file="AzureMLUriParser.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

using System;

using Azure.MachineLearning.Services.Artifacts;

namespace Azure.MachineLearning.Services
{
    public static class AzureMLUriParser
    {
        public static string GetModelId(Uri uri)
        {
            AzureMLUriParser.ThrowOnBadSchema(uri);
            AzureMLUriParser.ThrowOnHostname(uri, "asset");

            var modelId = uri.AbsolutePath.TrimStart('/');

            if (string.IsNullOrEmpty(modelId))
            {
                throw new ArgumentException("Uri did not contain a model Id");
            }

            return modelId;
        }

        public static ArtifactInformation GetArtifactInformation(Uri uri)
        {
            AzureMLUriParser.ThrowOnBadSchema(uri);
            AzureMLUriParser.ThrowOnHostname(uri, "artifact");

            var segments = uri.Segments;

            // First segment is always '/'
            if (segments.Length < 4)
            {
                throw new ArgumentException(string.Format("Uri {0} not in format aml://artifact/<origin>/<container>/<path>", uri));
            }

            var origin = segments[1].TrimEnd('/');
            var container = segments[2].TrimEnd('/');
            var path = string.Join(string.Empty, segments, 3, segments.Length - 3);

            return new ArtifactInformation
            {
                Origin = origin,
                Container = container,
                Path = Uri.UnescapeDataString(path)
            };
        }

        public static void ThrowOnBadSchema(Uri target)
        {
            if (target.Scheme != "aml")
            {
                throw new ArgumentException(string.Format("Uri {0} does not have scheme 'aml'", target.ToString()));
            }
        }

        public static void ThrowOnHostname(Uri target, string requiredHost)
        {
            if (target.Host != requiredHost)
            {
                throw new ArgumentException(string.Format("Uri {0} does not have required host '{1}'", target, requiredHost));
            }
        }
    }
}
