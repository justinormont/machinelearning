// <copyright file = "Workspace.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;

using Azure.MachineLearning.Services.Compute;
using Azure.MachineLearning.Services.Datastores;
using Azure.MachineLearning.Services.Experiments;
using Azure.MachineLearning.Services.Images;
using Azure.MachineLearning.Services.Models;
using Azure.MachineLearning.Services.Snapshots;

using Azure.MachineLearning.Services.Webservices;

using Microsoft.Azure.Management.ResourceManager.Fluent.Core;

namespace Azure.MachineLearning.Services.Workspaces
{
    public class Workspace : IResource
    {
        public Workspace(WorkspaceData workspaceDataDto, ServiceContext serviceContext)
        {
            Throw.IfNull(workspaceDataDto, nameof(workspaceDataDto));
            Throw.IfNull(serviceContext, nameof(serviceContext));

            if (workspaceDataDto.Name != serviceContext.WorkspaceName)
            {
                throw new ArgumentException("Workspace names do not match");
            }

            this.Context = serviceContext;

            this.UpdatefromWorkspaceData(workspaceDataDto);

            this.Models = new ModelOperations(this.Context);
            this.Experiments = new ExperimentOperations(this.Context);
            this.ComputeTargets = new ComputeTargetOperations(this.Context);
            this.Datastores = new DatastoreOperations(this.Context);
            this.Webservices = new WebserviceOperations(this.Context);
            this.Images = new ImageOperations(this.Context);
            this.Snapshots = new SnapshotOperations(this.Context);
        }

        public ServiceContext Context { get; private set; }

        public Guid SubscriptionId
        {
            get
            {
                return this.Context.SubscriptionId;
            }
        }

        public string ResourceGroupName
        {
            get
            {
                return this.Context.ResourceGroupName;
            }
        }

        public string StorageAccountArmId { get; private set; }

        public string KeyVaultArmId { get; private set; }

        public string AppInsightsArmId { get; private set; }

        public string AcrArmId { get; private set; }

        public ExperimentOperations Experiments { get; private set; }

        public ModelOperations Models { get; private set; }

        public ComputeTargetOperations ComputeTargets { get; private set; }

        public DatastoreOperations Datastores { get; private set; }

        public WebserviceOperations Webservices { get; private set; }

        public ImageOperations Images { get; private set; }

        #region From IResource
        public string Type { get; private set; }

        public string RegionName
        {
            get
            {
                return this.Context.Location;
            }
        }

        public Region Region
        {
            get
            {
                return Region.Create(this.RegionName);
            }
        }

        // The following appears to violate
        // https://docs.microsoft.com/en-us/dotnet/standard/design-guidelines/guidelines-for-collections#snapshots-versus-live-collections
        // which states that anything which is a snapshot should not be a property
        public IReadOnlyDictionary<string, string> Tags { get; private set; }

        public string Key { get; private set; }

        public string Id { get; private set; }

        public Guid WorkspaceId { get; private set; }

        public string Name { get; private set; }
        #endregion

        public SnapshotOperations Snapshots { get; private set; }

        private void UpdatefromWorkspaceData(WorkspaceData workspaceData)
        {
            Throw.IfNull(workspaceData, nameof(workspaceData));

            this.Name = workspaceData.Name;
            this.StorageAccountArmId = workspaceData.Properties.StorageAccount;
            this.KeyVaultArmId = workspaceData.Properties.KeyVault;
            this.AppInsightsArmId = workspaceData.Properties.ApplicationInsights;
            this.AcrArmId = workspaceData.Properties.ContainerRegistry;
            this.Type = workspaceData.Type;
            this.Tags = workspaceData.Tags;
            this.Id = workspaceData.Id;
            this.WorkspaceId = workspaceData.Properties.WorkspaceId;
        }
    }
}
