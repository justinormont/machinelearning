// <copyright file="SubscriptionInformation.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace Microsoft.ML.ModelBuilder.AutoMLService
{
    public class SubscriptionInformation
    {
        public string AccountId { get; set; }

        public string TenantId { get; set; }

        /// <summary>
        /// Gets or sets the fully qualified ID for the subscription. For example, /subscriptions/00000000-0000-0000-0000-000000000000.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the subscription ID.
        /// </summary>
        public string SubscriptionId { get; set; }

        /// <summary>
        /// Gets or sets the subscription display name.
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// Gets or sets the authorization source of the request. Valid values are one or
        /// more combinations of Legacy, RoleBased, Bypassed, Direct and Management. For
        /// example, 'Legacy, RoleBased'.
        /// </summary>
        public string AuthorizationSource { get; set; }
    }
}
