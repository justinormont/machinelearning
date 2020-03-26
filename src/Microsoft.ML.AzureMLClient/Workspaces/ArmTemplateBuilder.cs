// <copyright file = "ArmTemplateBuilder.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

using System;
using Microsoft.Azure.Management.ResourceManager.Fluent.Core;

using Newtonsoft.Json.Linq;

namespace Azure.MachineLearning.Services.Management
{
    public class ArmTemplateBuilder
    {
        private JObject template = new JObject();

        public ArmTemplateBuilder()
        {
            var schema = new JProperty("$schema", "https://schema.management.azure.com/schemas/2015-01-01/deploymentTemplate.json#");
            this.template.Add(schema);
            this.template.Add(new JProperty("contentVersion", "1.0.0.1"));
            this.template.Add(new JProperty("parameters", new JObject()));
            this.template.Add(new JProperty("variables", new JObject()));
            this.template.Add(new JProperty("resources", new JArray()));
        }

        public JObject GetTemplate()
        {
            return this.template;
        }

        public void Print()
        {
            Console.Write(this.template.ToString());
        }

        public JObject BuildKVTemplate(string name, Region region, string tenantId)
        {
            var res = new JObject();
            res.Add(new JProperty("type", "Microsoft.KeyVault/vaults"));
            res.Add(new JProperty("name", name));
            res.Add(new JProperty("apiVersion", "2016-10-01"));
            res.Add(new JProperty("location", region.ToString()));
            res.Add(new JProperty("dependsOn", new JArray()));

            var properties = new JObject();
            properties.Add(new JProperty("enabledForDeployment", true));
            properties.Add(new JProperty("enabledForTemplateDeployment", true));
            properties.Add(new JProperty("enabledForVolumeEncryption", true));
            properties.Add(new JProperty("tenantId", tenantId));
            properties.Add(new JProperty("accessPolicies", new JArray()));

            var sku = new JObject();
            sku.Add(new JProperty("name", "Standard"));
            sku.Add(new JProperty("family", "A"));
            properties.Add(new JProperty("sku", sku));

            res.Add("properties", properties);

            return res;
        }

        public JObject BuildStorageAccountTemplate(string name, Region region)
        {
            var res = new JObject
            {
                { "type", "Microsoft.Storage/storageAccounts" },
                { "name", name },
                { "apiVersion", "2018-07-01" },
                { "location", region.ToString() },
                {
                    "sku",
                    new JObject { { "name", "Standard_LRS" } }
                },
                { "kind", "StorageV2" },
                { "dependsOn", new JArray() }
            };

            var properties = new JObject();

            var encryption = new JObject
            {
                {
                    "services",
                    new JObject
                    {
                        { "blob", new JObject { { "enabled", "true" } } },
                        { "file", new JObject { { "enabled", "true" } } }
                    }
                },
                { "keySource", "Microsoft.Storage" }
            };

            properties.Add("encryption", encryption);
            properties.Add(new JProperty("supportsHttpsTrafficOnly", true));

            res.Add("properties", properties);

            return res;
        }

        public JObject BuildAzureContainerRegistryTemplate(string name, Region region)
        {
            var res = new JObject
            {
                { "type", "Microsoft.ContainerRegistry/registries" },
                { "name", name },
                { "apiVersion", "2017-10-01" },
                { "location", region.ToString() },
                {
                    "sku",
                    new JObject
                    {
                        { "name", "Standard" },
                        { "tier", "Standard" }
                    }
                },
                {
                    "properties",
                    new JObject
                    {
                        { "adminUserEnabled", "true" }
                    }
                }
            };

            return res;
        }

        public JObject BuildApplicationInsightsTemplate(string name, Region region)
        {
            region = WorkspaceManagementHelpers.RemapAppInsightsRegion(region);

            var res = new JObject
            {
                { "type", "Microsoft.Insights/components" },
                { "name", name },
                { "kind", "web" },
                { "apiVersion", "2015-05-01" },
                { "location", region.ToString() },
                {
                    "properties",
                    new JObject
                    {
                        { "Application_Type", "web" }
                    }
                }
            };

            return res;
        }

        public JObject BuildWorkspaceResource(
            string name,
            Region region,
            string keyVaultId,
            string azureContainerRegistryId,
            string storageId,
            string applicationInsightsId,
            bool isBasic)
        {
            var skuValue = isBasic ? "basic" : "enterprise";

            var res = new JObject
            {
                { "type", "Microsoft.MachineLearningServices/workspaces" },
                { "name", name },
                { "apiVersion", "2018-11-19" },
                {
                    "identity",
                    new JObject
                    {
                        { "type", "systemAssigned" }
                    }
                },
                { "location", region.ToString() },
                { "resources", new JArray() },
                { "dependsOn", new JArray() },
                { "sku", new JObject
                    {
                        {"tier", skuValue },
                        {"name", skuValue },
                    }
                },
                {
                    "properties",
                    new JObject
                    {
                        { "containerRegistry", azureContainerRegistryId },
                        { "keyVault", keyVaultId },
                        { "applicationInsights", applicationInsightsId },
                        { "friendlyName", name },
                        { "storageAccount", storageId }
                    }
                }
            };

            return res;
        }

        public void AddResource(JObject resource)
        {
            JArray resources = this.template.GetValue("resources") as JArray;
            resources.Add(resource);
        }
    }
}
