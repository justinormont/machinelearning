// <copyright file="Model.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

using Azure.MachineLearning.Services.Artifacts;
using Azure.MachineLearning.Services.Assets;

namespace Azure.MachineLearning.Services.Models
{
    public abstract class Model
    {
        public Model(ServiceContext serviceContext, GeneratedOld.Models.Model modelDto)
        {
            Throw.IfNull(serviceContext, nameof(serviceContext));
            Throw.IfNull(modelDto, nameof(modelDto));
            this.ServiceContext = serviceContext;
            this.UpdateCommonFromData(modelDto);
        }

        public ServiceContext ServiceContext { get; private set; }

        public string Name { get; private set; }

        public string Id { get; private set; }

        public int? Version { get; private set; }

        public IReadOnlyList<string> Tags { get; private set; }

        public Uri Url { get; private set; }

        public string MimeType { get; set; }

        public string Description { get; private set; }

        public DateTime? CreatedTime { get; private set; }

        public bool? Unpack { get; private set; }

        public async virtual Task DownloadAsync(
            DirectoryInfo destinationFolder,
            Dictionary<string, List<string>> customHeaders = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            Throw.IfNull(destinationFolder, nameof(destinationFolder));

            string assetId = AzureMLUriParser.GetModelId(Url);

            var assetOps = new AssetOperations(this.ServiceContext);
            Asset asset = await assetOps.GetAsync(assetId, customHeaders, cancellationToken).ConfigureAwait(false);

            var artifactOps = new ArtifactOperations(this.ServiceContext);
            var webClient = new WebClient();

            foreach (var a in asset.Artifacts)
            {
                IPageFetcher<ArtifactContent> dataList = artifactOps.GetPagedList(a.Prefix);
                do
                {
                    IEnumerable<ArtifactContent> nextData = await dataList.FetchNextPageAsync(customHeaders, cancellationToken).ConfigureAwait(false);
                    foreach (var d in nextData)
                    {
                        // This next bit is a security hole if someone can control the path and walk up
                        // the directory tree

                        // Prune the first directory from the path
                        // We assume URL separators here, hence we don't use
                        // Path.DirectorySeparatorChar
                        var splitChars = new char[] { '/' };
                        string[] splitPath = d.Path.Split(splitChars, 2);
                        string trailingPath = splitPath[splitPath.Length - 1];
                        string targetPath = Path.Combine(destinationFolder.FullName, trailingPath);

                        DirectoryInfo targetDir = Directory.GetParent(targetPath);
                        if (!targetDir.Exists)
                        {
                            targetDir.Create();
                        }

                        await Task.Run(
                            () => webClient.DownloadFile(
                                d.ContentUri,
                                targetPath), cancellationToken).ConfigureAwait(false);
                    }
                }
                while (!dataList.OnLastPage);
            }
        }

        protected void UpdateCommonFromData(GeneratedOld.Models.Model modelDto)
        {
            Throw.IfNull(modelDto, nameof(modelDto));

            this.Name = modelDto.Name;
            this.Id = modelDto.Id;
            this.Version = modelDto.Version;
            this.Tags = new List<string>();
            if (modelDto.Tags != null)
            {
                this.Tags = modelDto.Tags.ToList().AsReadOnly();
            }
            this.Url = new Uri(modelDto.Url);
            this.Description = modelDto.Description;
            this.MimeType = modelDto.MimeType;
            this.CreatedTime = modelDto.CreatedTime;
            this.Unpack = modelDto.Unpack;
        }
    }
}
