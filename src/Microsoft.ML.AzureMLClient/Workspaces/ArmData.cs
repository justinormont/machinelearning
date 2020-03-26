// <copyright file="ArmData.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

using System;

using Newtonsoft.Json;

namespace Azure.MachineLearning.Services.Workspaces
{
    public class ArmData
    {
        private string _id;

        [JsonProperty(PropertyName = "id")]
        public string Id
        {
            get
            {
                return this._id;
            }

            set
            {
                this._id = value;

                string[] parts = value.Split('/');

                this.SubscriptionId = Guid.Parse(parts[2]);
                this.ResourceGroupName = parts[4];
            }
        }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "type")]
        public string Type { get; set; }

        public Guid SubscriptionId { get; private set; }

        public string ResourceGroupName { get; private set; }
    }
}
