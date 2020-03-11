// <copyright file = "WorkspaceManagementHelpers.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

using System;
using System.Linq;
using System.Text.RegularExpressions;

using Microsoft.Azure.Management.ResourceManager.Fluent.Core;

namespace Azure.MachineLearning.Services.Management
{
    public static class WorkspaceManagementHelpers
    {
        public enum DependentResourceType
        {
            KeyVault,
            ApplicationInsights,
            StorageAccount,
            ResourceGroup,
            ContainerRegistry,
            MachineLearningWorkspace
        }

        public static bool ValidateWorkspaceName(string name)
        {
            string pattern = @"^([a-zA-Z0-9][\w\-]){2,32}$";

            return ExecuteMatch(name, pattern);
        }

        public static bool ValidateExperimentName(string name)
        {
            string pattern = @"^([a-zA-Z0-9\w\-]){0,35}$";

            return ExecuteMatch(name, pattern);
        }

        // Vault Name must contain only alphanumeric characters and dashes, must start with a letter.
        // Vault Name is limited to 3-24 alphanumeric characters.
        // No consecutive hypens, can't start or end with hyphen
        public static bool ValidateAzureDNSName(string name)
        {
            string pattern = @"^([a-zA-Z0-9\w\-]){0,35}$";

            return ExecuteMatch(name, pattern);
        }

        // Application Insights Names use the same rules as ValidateResourceGroupName
        public static bool ValidateApplicationInsightName(string name)
        {
            return ValidateResourceGroupName(name);
        }

        // Valid resource group names are located here:
        // https://docs.microsoft.com/en-us/rest/api/resources/resourcegroups/createorupdate
        public static bool ValidateResourceGroupName(string name)
        {
            string pattern = @"^([-\w\._\(\)]){0,89}[-\w_\(\)]$";

            return ExecuteMatch(name, pattern);
        }

        // Valid resource group names are located here:
        // https://docs.microsoft.com/en-us/rest/api/resources/resourcegroups/createorupdate
        // names can contain uppercase characters
        public static bool ValidateAzureContainerName(string name)
        {
            string pattern = @"^([a-zA-Z0-9]){5,50}[-\w_\(\)]$";

            return ExecuteMatch(name, pattern);
        }

        public static string GenerateNameForDependentResource(string workspaceName, DependentResourceType resourceType)
        {
            Throw.IfNull(workspaceName, nameof(workspaceName));
            string dependentName = string.Concat(workspaceName.Where(ch => char.IsLetterOrDigit(ch)));
            dependentName = string.Concat(dependentName, resourceType.ToString().ToLower().Substring(0, 8));
            dependentName = string.Concat(dependentName, GenerateRandomString());
            if (resourceType == DependentResourceType.StorageAccount
                || resourceType == DependentResourceType.KeyVault)
            {
                dependentName = dependentName.ToLowerInvariant();
            }

            // resources can't start with digits
            if (char.IsDigit(dependentName[0]))
            {
                dependentName = string.Concat(GenerateRandomString(1, false), dependentName);
            }

            return dependentName.Length > 23 ? dependentName.Substring(0, 23) : dependentName;
        }

        // App insights is not in all locations as us. So, create a map
        // update according to https://azure.microsoft.com/en-us/global-infrastructure/services/?products=monitor
        public static Region RemapAppInsightsRegion(Region region)
        {
            if (region == Region.USWestCentral || region == Region.USWest)
            {
                return Region.USWest2;
            }
            if (region == Region.USEast2)
            {
                return Region.USEast;
            }

            return region;
        }

        private static bool ExecuteMatch(string name, string pattern)
        {
            if (name == null || pattern == null)
            {
                return false;
            }

            return new Regex(pattern).IsMatch(name);
        }

        // This is not used for any security purpose, so using Random is sufficient
        private static string GenerateRandomString(int length = 8, bool alphanumeric = true)
        {
            if (length < 1)
            {
                throw new ArgumentException(string.Format("length must be greater than 0, {0} specified", length));
            }

            string chars = "abcdefghijklmnopqrstuvwxyz";
            char[] newArray = new char[length];
            Random random = new Random();

            if (alphanumeric)
            {
                chars = string.Concat("0123456789", chars);
            }

            for (int i = 0; i < length; i++)
            {
                newArray[i] = chars[random.Next(chars.Length)];
            }

            return new string(newArray);
        }
    }
}
