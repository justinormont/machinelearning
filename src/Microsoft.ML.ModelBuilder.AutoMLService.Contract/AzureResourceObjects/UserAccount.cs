// <copyright file="UserAccount.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace Microsoft.ML.ModelBuilder.AutoMLService
{
    public class UserAccount
    {
        public string Id { get; set; }

        // Display information for the account's provider
        public string ProviderDisplayName { get; set; }

        public string ProviderLogo { get; set; }

        // Display information for the account
        public string Email { get; set; }

        public string DisplayName { get; set; }

        // If account needs reauth
        public bool NeedsReauthentication { get; set; }

        // Relevant URLS
        public string ManagementEndpoint { get; set; }

        public string AdalEndpoint { get; set; }

        public string PortalUrl { get; set; }

        public string GraphEndpoint { get; set; }

        // Other identifiers
        public string ProviderId { get; set; }

        public string VsoId { get; set; }

        // Only used by IUserAccountsManager
        public dynamic VsAccountObj { get; set; }
    }
}
