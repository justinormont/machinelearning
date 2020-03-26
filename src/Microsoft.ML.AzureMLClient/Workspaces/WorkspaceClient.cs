// <copyright file = "WorkspaceClient.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

using Microsoft.Azure.Management.ResourceManager.Fluent;

using Microsoft.Rest;
using Microsoft.Rest.Serialization;
using Newtonsoft.Json;

namespace Azure.MachineLearning.Services.Workspaces
{
    public class WorkspaceClient : Microsoft.Rest.ServiceClient<WorkspaceClient>,
        Microsoft.Rest.Azure.IAzureClient,
        IDisposable
    {
        public const string MicrosoftAzureTenantId = "72f988bf-86f1-41af-91ab-2d7cd011db47";

        /// <summary>
        /// Initializes a new instance of the WorkspaceClient class.
        /// </summary>
        /// <param name='credentials'>
        /// Required. Credentials needed for the client to connect to Azure.
        /// </param>
        /// <param name='handlers'>
        /// Optional. The delegating handlers to add to the http client pipeline.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when a required parameter is null
        /// </exception>
        public WorkspaceClient(ServiceClientCredentials credentials, params DelegatingHandler[] handlers)
            : this(credentials, MicrosoftAzureTenantId, AzureEnvironment.AzureGlobalCloud, handlers)
        {
        }

#pragma warning disable CS1573 // Parameter has no matching param tag in the XML comment (but other parameters do)
        /// <summary>
        /// Initializes a new instance of the WorkspaceClient class.
        /// </summary>
        /// <param name='credentials'>
        /// Required. Credentials needed for the client to connect to Azure.
        /// </param>
        /// <param name='handlers'>
        /// Optional. The delegating handlers to add to the http client pipeline.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when a required parameter is null
        /// </exception>
        public WorkspaceClient(ServiceClientCredentials credentials, string tenantId, params DelegatingHandler[] handlers)
#pragma warning restore CS1573 // Parameter has no matching param tag in the XML comment (but other parameters do)
            : this(credentials, tenantId, AzureEnvironment.AzureGlobalCloud, handlers)
        {
        }

#pragma warning disable CS1573 // Parameter has no matching param tag in the XML comment (but other parameters do)
#pragma warning disable CS1573 // Parameter has no matching param tag in the XML comment (but other parameters do)
        /// <summary>
        /// Initializes a new instance of the WorkspaceClient class.
        /// </summary>
        /// <param name='baseUri'>
        /// Optional. The base URI of the service.
        /// </param>
        /// <param name='credentials'>
        /// Required. Credentials needed for the client to connect to Azure.
        /// </param>
        /// <param name='handlers'>
        /// Optional. The delegating handlers to add to the http client pipeline.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when a required parameter is null
        /// </exception>
        public WorkspaceClient(ServiceClientCredentials credentials, string tenantId, AzureEnvironment azureEnvironment, params DelegatingHandler[] handlers)
#pragma warning restore CS1573 // Parameter has no matching param tag in the XML comment (but other parameters do)
#pragma warning restore CS1573 // Parameter has no matching param tag in the XML comment (but other parameters do)
            : base(handlers)
        {
            Throw.IfNullOrEmpty(tenantId, nameof(tenantId));
            Throw.IfNull(credentials, nameof(credentials));
            BaseUri = new Uri(azureEnvironment.ResourceManagerEndpoint);
            TenantId = tenantId;
            AzureEnvironment = azureEnvironment;
            Credentials = credentials;
            Credentials.InitializeServiceClient(this);
            Initialize();
        }

        /// <summary>
        /// The authentication callback delegate which is to be implemented by the client code
        /// </summary>
        /// <param name="authority"> Identifier of the authority, a URL. </param>
        /// <param name="resource"> Identifier of the target resource that is the recipient of the requested token, a URL. </param>
        /// <param name="scope"> The scope of the authentication request. </param>
        /// <returns> access token </returns>
        public delegate Task<string> AuthenticationCallback(string authority, string resource, string scope);

        /// <summary>
        /// The base URI of the service.
        /// </summary>
        public System.Uri BaseUri { get; private set; }

        /// <summary>
        /// The tenantId for the Azure Active Directory Service
        /// </summary>
        public string TenantId { get; private set; }

        /// <summary>
        /// The type of Azure cloud (AzurePublicCloud)
        /// </summary>
        public AzureEnvironment AzureEnvironment { get; private set; } = AzureEnvironment.AzureGlobalCloud;

        /// <summary>
        /// Client API version.
        /// </summary>
        public string ApiVersion { get; private set; } = "2018-11-19";

        /// <summary>
        /// The preferred language for the response.
        /// </summary>
        public string AcceptLanguage { get; set; } = "en-US";

        public WorkspaceOperations Workspaces { get; private set; }

        /// <summary>
        /// Credentials needed for the client to connect to Azure.
        /// </summary>
        public ServiceClientCredentials Credentials { get; private set; }

        /// <summary>
        /// The retry timeout in seconds for Long Running Operations.
        /// </summary>
        public int? LongRunningOperationRetryTimeout { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        /// <summary>
        /// Gets json serialization settings.
        /// </summary>
        public JsonSerializerSettings SerializationSettings { get; private set; }

        /// <summary>
        /// Gets json serialization settings.
        /// </summary>
        public JsonSerializerSettings DeserializationSettings { get; private set; }

        /// <summary>
        /// Whether a unique x-ms-client-request-id should be GeneratedOld. When set to
        /// true a unique x-ms-client-request-id value is generated and included in
        /// each request. Default is true.
        /// </summary>
        public bool? GenerateClientRequestId { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        private void Initialize()
        {
            this.HttpClient.BaseAddress = BaseUri;

            this.Workspaces = new WorkspaceOperations(this);
            this.ApiVersion = "2018-11-19";
            this.AcceptLanguage = "en-US";

            this.SerializationSettings = new JsonSerializerSettings
            {
                Formatting = Newtonsoft.Json.Formatting.Indented,
                DateFormatHandling = Newtonsoft.Json.DateFormatHandling.IsoDateFormat,
                DateTimeZoneHandling = Newtonsoft.Json.DateTimeZoneHandling.Utc,
                NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore,
                ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Serialize,
                ContractResolver = new ReadOnlyJsonContractResolver(),
                Converters = new List<JsonConverter>
                    {
                        new Iso8601TimeSpanConverter()
                    }
            };

            this.DeserializationSettings = new JsonSerializerSettings
            {
                DateFormatHandling = Newtonsoft.Json.DateFormatHandling.IsoDateFormat,
                DateTimeZoneHandling = Newtonsoft.Json.DateTimeZoneHandling.Utc,
                NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore,
                ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Serialize,
                ContractResolver = new ReadOnlyJsonContractResolver(),
                Converters = new List<JsonConverter>
                    {
                        new Iso8601TimeSpanConverter()
                    }
            };
        }
    }
}
