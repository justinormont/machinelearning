// <auto-generated>
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for
// license information.
//
// Code generated by Microsoft (R) AutoRest Code Generator 1.0.0.0
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>

namespace Azure.MachineLearning.Services.Generated
{
    using Microsoft.Rest;
    using Models;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Artifacts operations.
    /// </summary>
    public partial interface IArtifacts
    {
        /// <summary>
        /// Create Artifact.
        /// </summary>
        /// <remarks>
        /// Create an Artifact.
        /// </remarks>
        /// <param name='subscriptionId'>
        /// The Azure Subscription ID.
        /// </param>
        /// <param name='resourceGroupName'>
        /// The Name of the resource group in which the workspace is located.
        /// </param>
        /// <param name='workspaceName'>
        /// The name of the workspace.
        /// </param>
        /// <param name='artifact'>
        /// The Artifact details.
        /// </param>
        /// <param name='customHeaders'>
        /// The headers that will be added to request.
        /// </param>
        /// <param name='cancellationToken'>
        /// The cancellation token.
        /// </param>
        /// <exception cref="ErrorResponseException">
        /// Thrown when the operation returned an invalid status code
        /// </exception>
        /// <exception cref="Microsoft.Rest.SerializationException">
        /// Thrown when unable to deserialize the response
        /// </exception>
        /// <exception cref="Microsoft.Rest.ValidationException">
        /// Thrown when a required parameter is null
        /// </exception>
        Task<HttpOperationResponse<Artifact>> CreateWithHttpMessagesAsync(System.Guid subscriptionId, string resourceGroupName, string workspaceName, Artifact artifact, Dictionary<string, List<string>> customHeaders = null, CancellationToken cancellationToken = default(CancellationToken));
        /// <summary>
        /// Create an Artifact for an existing data location.
        /// </summary>
        /// <remarks>
        /// Create an Artifact for an existing dataPath.
        /// </remarks>
        /// <param name='subscriptionId'>
        /// The Azure Subscription ID.
        /// </param>
        /// <param name='resourceGroupName'>
        /// The Name of the resource group in which the workspace is located.
        /// </param>
        /// <param name='workspaceName'>
        /// The name of the workspace.
        /// </param>
        /// <param name='artifact'>
        /// The Artifact creation details.
        /// </param>
        /// <param name='customHeaders'>
        /// The headers that will be added to request.
        /// </param>
        /// <param name='cancellationToken'>
        /// The cancellation token.
        /// </param>
        /// <exception cref="ErrorResponseException">
        /// Thrown when the operation returned an invalid status code
        /// </exception>
        /// <exception cref="Microsoft.Rest.SerializationException">
        /// Thrown when unable to deserialize the response
        /// </exception>
        /// <exception cref="Microsoft.Rest.ValidationException">
        /// Thrown when a required parameter is null
        /// </exception>
        Task<HttpOperationResponse<Artifact>> RegisterWithHttpMessagesAsync(System.Guid subscriptionId, string resourceGroupName, string workspaceName, Artifact artifact, Dictionary<string, List<string>> customHeaders = null, CancellationToken cancellationToken = default(CancellationToken));
        /// <summary>
        /// Get Artifact metadata by Id.
        /// </summary>
        /// <remarks>
        /// Get Artifact metadata for a specific Id.
        /// </remarks>
        /// <param name='subscriptionId'>
        /// The Azure Subscription ID.
        /// </param>
        /// <param name='resourceGroupName'>
        /// The Name of the resource group in which the workspace is located.
        /// </param>
        /// <param name='workspaceName'>
        /// The name of the workspace.
        /// </param>
        /// <param name='origin'>
        /// The origin of the Artifact.
        /// </param>
        /// <param name='container'>
        /// The container name.
        /// </param>
        /// <param name='path'>
        /// The Artifact Path.
        /// </param>
        /// <param name='customHeaders'>
        /// The headers that will be added to request.
        /// </param>
        /// <param name='cancellationToken'>
        /// The cancellation token.
        /// </param>
        /// <exception cref="ErrorResponseException">
        /// Thrown when the operation returned an invalid status code
        /// </exception>
        /// <exception cref="Microsoft.Rest.SerializationException">
        /// Thrown when unable to deserialize the response
        /// </exception>
        /// <exception cref="Microsoft.Rest.ValidationException">
        /// Thrown when a required parameter is null
        /// </exception>
        Task<HttpOperationResponse<Artifact>> GetWithHttpMessagesAsync(System.Guid subscriptionId, string resourceGroupName, string workspaceName, string origin, string container, string path = default(string), Dictionary<string, List<string>> customHeaders = null, CancellationToken cancellationToken = default(CancellationToken));
        /// <summary>
        /// Delete Artifact Metadata.
        /// </summary>
        /// <remarks>
        /// Delete an Artifact Metadata.
        /// </remarks>
        /// <param name='subscriptionId'>
        /// The Azure Subscription ID.
        /// </param>
        /// <param name='resourceGroupName'>
        /// The Name of the resource group in which the workspace is located.
        /// </param>
        /// <param name='workspaceName'>
        /// The name of the workspace.
        /// </param>
        /// <param name='origin'>
        /// The origin of the Artifact.
        /// </param>
        /// <param name='container'>
        /// The container name.
        /// </param>
        /// <param name='path'>
        /// The Artifact Path.
        /// </param>
        /// <param name='hardDelete'>
        /// If set to true. The delete cannot be revert at later time.
        /// </param>
        /// <param name='customHeaders'>
        /// The headers that will be added to request.
        /// </param>
        /// <param name='cancellationToken'>
        /// The cancellation token.
        /// </param>
        /// <exception cref="ErrorResponseException">
        /// Thrown when the operation returned an invalid status code
        /// </exception>
        /// <exception cref="Microsoft.Rest.ValidationException">
        /// Thrown when a required parameter is null
        /// </exception>
        Task<HttpOperationResponse> DeleteMetaDataWithHttpMessagesAsync(System.Guid subscriptionId, string resourceGroupName, string workspaceName, string origin, string container, string path = default(string), bool? hardDelete = false, Dictionary<string, List<string>> customHeaders = null, CancellationToken cancellationToken = default(CancellationToken));
        /// <summary>
        /// Get Artifacts metadata in a container or path.
        /// </summary>
        /// <remarks>
        /// Get Artifacts metadata in a specific container or path.
        /// </remarks>
        /// <param name='subscriptionId'>
        /// The Azure Subscription ID.
        /// </param>
        /// <param name='resourceGroupName'>
        /// The Name of the resource group in which the workspace is located.
        /// </param>
        /// <param name='workspaceName'>
        /// The name of the workspace.
        /// </param>
        /// <param name='origin'>
        /// The origin of the Artifact.
        /// </param>
        /// <param name='container'>
        /// The container name.
        /// </param>
        /// <param name='path'>
        /// The Artifact Path.
        /// </param>
        /// <param name='continuationToken'>
        /// The continuation token.
        /// </param>
        /// <param name='customHeaders'>
        /// The headers that will be added to request.
        /// </param>
        /// <param name='cancellationToken'>
        /// The cancellation token.
        /// </param>
        /// <exception cref="ErrorResponseException">
        /// Thrown when the operation returned an invalid status code
        /// </exception>
        /// <exception cref="Microsoft.Rest.SerializationException">
        /// Thrown when unable to deserialize the response
        /// </exception>
        /// <exception cref="Microsoft.Rest.ValidationException">
        /// Thrown when a required parameter is null
        /// </exception>
        Task<HttpOperationResponse<PaginatedArtifactList>> ListInContainerWithHttpMessagesAsync(System.Guid subscriptionId, string resourceGroupName, string workspaceName, string origin, string container, string path = default(string), string continuationToken = default(string), Dictionary<string, List<string>> customHeaders = null, CancellationToken cancellationToken = default(CancellationToken));
        /// <summary>
        /// Get Artifact content by Id.
        /// </summary>
        /// <remarks>
        /// Get Artifact content of a specific Id.
        /// </remarks>
        /// <param name='subscriptionId'>
        /// The Azure Subscription ID.
        /// </param>
        /// <param name='resourceGroupName'>
        /// The Name of the resource group in which the workspace is located.
        /// </param>
        /// <param name='workspaceName'>
        /// The name of the workspace.
        /// </param>
        /// <param name='origin'>
        /// The origin of the Artifact.
        /// </param>
        /// <param name='container'>
        /// The container name.
        /// </param>
        /// <param name='path'>
        /// The Artifact Path.
        /// </param>
        /// <param name='customHeaders'>
        /// The headers that will be added to request.
        /// </param>
        /// <param name='cancellationToken'>
        /// The cancellation token.
        /// </param>
        /// <exception cref="Microsoft.Rest.HttpOperationException">
        /// Thrown when the operation returned an invalid status code
        /// </exception>
        /// <exception cref="Microsoft.Rest.SerializationException">
        /// Thrown when unable to deserialize the response
        /// </exception>
        /// <exception cref="Microsoft.Rest.ValidationException">
        /// Thrown when a required parameter is null
        /// </exception>
        Task<HttpOperationResponse<Stream>> DownloadWithHttpMessagesAsync(System.Guid subscriptionId, string resourceGroupName, string workspaceName, string origin, string container, string path = default(string), Dictionary<string, List<string>> customHeaders = null, CancellationToken cancellationToken = default(CancellationToken));
        /// <summary>
        /// Upload Artifact content.
        /// </summary>
        /// <remarks>
        /// Upload content to an Artifact.
        /// </remarks>
        /// <param name='subscriptionId'>
        /// The Azure Subscription ID.
        /// </param>
        /// <param name='resourceGroupName'>
        /// The Name of the resource group in which the workspace is located.
        /// </param>
        /// <param name='workspaceName'>
        /// The name of the workspace.
        /// </param>
        /// <param name='origin'>
        /// The origin of the Artifact.
        /// </param>
        /// <param name='container'>
        /// The container name.
        /// </param>
        /// <param name='content'>
        /// The file upload.
        /// </param>
        /// <param name='path'>
        /// The Artifact Path.
        /// </param>
        /// <param name='index'>
        /// The index.
        /// </param>
        /// <param name='append'>
        /// Whether or not to append the content or replace it.
        /// </param>
        /// <param name='allowOverwrite'>
        /// whether to allow overwrite if Artifact Content exist already. when
        /// set to true, Overwrite happens if Artifact Content already exists
        /// </param>
        /// <param name='customHeaders'>
        /// The headers that will be added to request.
        /// </param>
        /// <param name='cancellationToken'>
        /// The cancellation token.
        /// </param>
        /// <exception cref="ErrorResponseException">
        /// Thrown when the operation returned an invalid status code
        /// </exception>
        /// <exception cref="Microsoft.Rest.SerializationException">
        /// Thrown when unable to deserialize the response
        /// </exception>
        /// <exception cref="Microsoft.Rest.ValidationException">
        /// Thrown when a required parameter is null
        /// </exception>
        Task<HttpOperationResponse<Artifact>> UploadWithHttpMessagesAsync(System.Guid subscriptionId, string resourceGroupName, string workspaceName, string origin, string container, string content, string path = default(string), int? index = default(int?), bool? append = false, bool? allowOverwrite = false, Dictionary<string, List<string>> customHeaders = null, CancellationToken cancellationToken = default(CancellationToken));
        /// <summary>
        /// Get Artifact content information.
        /// </summary>
        /// <remarks>
        /// Get content information of an Artifact.
        /// </remarks>
        /// <param name='subscriptionId'>
        /// The Azure Subscription ID.
        /// </param>
        /// <param name='resourceGroupName'>
        /// The Name of the resource group in which the workspace is located.
        /// </param>
        /// <param name='workspaceName'>
        /// The name of the workspace.
        /// </param>
        /// <param name='origin'>
        /// The origin of the Artifact.
        /// </param>
        /// <param name='container'>
        /// The container name.
        /// </param>
        /// <param name='path'>
        /// The Artifact Path.
        /// </param>
        /// <param name='customHeaders'>
        /// The headers that will be added to request.
        /// </param>
        /// <param name='cancellationToken'>
        /// The cancellation token.
        /// </param>
        /// <exception cref="ErrorResponseException">
        /// Thrown when the operation returned an invalid status code
        /// </exception>
        /// <exception cref="Microsoft.Rest.SerializationException">
        /// Thrown when unable to deserialize the response
        /// </exception>
        /// <exception cref="Microsoft.Rest.ValidationException">
        /// Thrown when a required parameter is null
        /// </exception>
        Task<HttpOperationResponse<ArtifactContentInformation>> GetContentInformationWithHttpMessagesAsync(System.Guid subscriptionId, string resourceGroupName, string workspaceName, string origin, string container, string path = default(string), Dictionary<string, List<string>> customHeaders = null, CancellationToken cancellationToken = default(CancellationToken));
        /// <summary>
        /// Get Artifact storage content information.
        /// </summary>
        /// <remarks>
        /// Get storage content information of an Artifact.
        /// </remarks>
        /// <param name='subscriptionId'>
        /// The Azure Subscription ID.
        /// </param>
        /// <param name='resourceGroupName'>
        /// The Name of the resource group in which the workspace is located.
        /// </param>
        /// <param name='workspaceName'>
        /// The name of the workspace.
        /// </param>
        /// <param name='origin'>
        /// The origin of the Artifact.
        /// </param>
        /// <param name='container'>
        /// The container name.
        /// </param>
        /// <param name='path'>
        /// The Artifact Path.
        /// </param>
        /// <param name='customHeaders'>
        /// The headers that will be added to request.
        /// </param>
        /// <param name='cancellationToken'>
        /// The cancellation token.
        /// </param>
        /// <exception cref="ErrorResponseException">
        /// Thrown when the operation returned an invalid status code
        /// </exception>
        /// <exception cref="Microsoft.Rest.SerializationException">
        /// Thrown when unable to deserialize the response
        /// </exception>
        /// <exception cref="Microsoft.Rest.ValidationException">
        /// Thrown when a required parameter is null
        /// </exception>
        Task<HttpOperationResponse<ArtifactContentInformation>> GetStorageContentInformationWithHttpMessagesAsync(System.Guid subscriptionId, string resourceGroupName, string workspaceName, string origin, string container, string path = default(string), Dictionary<string, List<string>> customHeaders = null, CancellationToken cancellationToken = default(CancellationToken));
        /// <summary>
        /// Get writable shared access signature for Artifact.
        /// </summary>
        /// <remarks>
        /// Get writable shared access signature for a specific Artifact.
        /// </remarks>
        /// <param name='subscriptionId'>
        /// The Azure Subscription ID.
        /// </param>
        /// <param name='resourceGroupName'>
        /// The Name of the resource group in which the workspace is located.
        /// </param>
        /// <param name='workspaceName'>
        /// The name of the workspace.
        /// </param>
        /// <param name='origin'>
        /// The origin of the Artifact.
        /// </param>
        /// <param name='container'>
        /// The container name.
        /// </param>
        /// <param name='path'>
        /// The Artifact Path.
        /// </param>
        /// <param name='customHeaders'>
        /// The headers that will be added to request.
        /// </param>
        /// <param name='cancellationToken'>
        /// The cancellation token.
        /// </param>
        /// <exception cref="ErrorResponseException">
        /// Thrown when the operation returned an invalid status code
        /// </exception>
        /// <exception cref="Microsoft.Rest.SerializationException">
        /// Thrown when unable to deserialize the response
        /// </exception>
        /// <exception cref="Microsoft.Rest.ValidationException">
        /// Thrown when a required parameter is null
        /// </exception>
        Task<HttpOperationResponse<ArtifactContentInformation>> GetSasWithHttpMessagesAsync(System.Guid subscriptionId, string resourceGroupName, string workspaceName, string origin, string container, string path = default(string), Dictionary<string, List<string>> customHeaders = null, CancellationToken cancellationToken = default(CancellationToken));
        /// <summary>
        /// Get shared access signature for an Artifact
        /// </summary>
        /// <remarks>
        /// Get shared access signature for an Artifact in specific path.
        /// </remarks>
        /// <param name='subscriptionId'>
        /// The Azure Subscription ID.
        /// </param>
        /// <param name='resourceGroupName'>
        /// The Name of the resource group in which the workspace is located.
        /// </param>
        /// <param name='workspaceName'>
        /// The name of the workspace.
        /// </param>
        /// <param name='origin'>
        /// The origin of the Artifact.
        /// </param>
        /// <param name='container'>
        /// The container name.
        /// </param>
        /// <param name='path'>
        /// The Artifact Path.
        /// </param>
        /// <param name='continuationToken'>
        /// The continuation token.
        /// </param>
        /// <param name='customHeaders'>
        /// The headers that will be added to request.
        /// </param>
        /// <param name='cancellationToken'>
        /// The cancellation token.
        /// </param>
        /// <exception cref="ErrorResponseException">
        /// Thrown when the operation returned an invalid status code
        /// </exception>
        /// <exception cref="Microsoft.Rest.SerializationException">
        /// Thrown when unable to deserialize the response
        /// </exception>
        /// <exception cref="Microsoft.Rest.ValidationException">
        /// Thrown when a required parameter is null
        /// </exception>
        Task<HttpOperationResponse<PaginatedArtifactContentInformationList>> ListSasByPrefixWithHttpMessagesAsync(System.Guid subscriptionId, string resourceGroupName, string workspaceName, string origin, string container, string path = default(string), string continuationToken = default(string), Dictionary<string, List<string>> customHeaders = null, CancellationToken cancellationToken = default(CancellationToken));
        /// <summary>
        /// Get storage Uri for Artifacts in a path.
        /// </summary>
        /// <remarks>
        /// Get storage Uri for Artifacts in a specific path.
        /// </remarks>
        /// <param name='subscriptionId'>
        /// The Azure Subscription ID.
        /// </param>
        /// <param name='resourceGroupName'>
        /// The Name of the resource group in which the workspace is located.
        /// </param>
        /// <param name='workspaceName'>
        /// The name of the workspace.
        /// </param>
        /// <param name='origin'>
        /// The origin of the Artifact.
        /// </param>
        /// <param name='container'>
        /// The container name.
        /// </param>
        /// <param name='path'>
        /// The Artifact Path.
        /// </param>
        /// <param name='continuationToken'>
        /// The continuation token.
        /// </param>
        /// <param name='customHeaders'>
        /// The headers that will be added to request.
        /// </param>
        /// <param name='cancellationToken'>
        /// The cancellation token.
        /// </param>
        /// <exception cref="ErrorResponseException">
        /// Thrown when the operation returned an invalid status code
        /// </exception>
        /// <exception cref="Microsoft.Rest.SerializationException">
        /// Thrown when unable to deserialize the response
        /// </exception>
        /// <exception cref="Microsoft.Rest.ValidationException">
        /// Thrown when a required parameter is null
        /// </exception>
        Task<HttpOperationResponse<PaginatedArtifactContentInformationList>> ListStorageUriByPrefixWithHttpMessagesAsync(System.Guid subscriptionId, string resourceGroupName, string workspaceName, string origin, string container, string path = default(string), string continuationToken = default(string), Dictionary<string, List<string>> customHeaders = null, CancellationToken cancellationToken = default(CancellationToken));
        /// <summary>
        /// Get Batch Artifacts by Ids.
        /// </summary>
        /// <remarks>
        /// Get Batch Artifacts by the specific Ids.
        /// </remarks>
        /// <param name='subscriptionId'>
        /// The Azure Subscription ID.
        /// </param>
        /// <param name='resourceGroupName'>
        /// The Name of the resource group in which the workspace is located.
        /// </param>
        /// <param name='workspaceName'>
        /// The name of the workspace.
        /// </param>
        /// <param name='artifactIds'>
        /// The command for Batch Artifact get request.
        /// </param>
        /// <param name='customHeaders'>
        /// The headers that will be added to request.
        /// </param>
        /// <param name='cancellationToken'>
        /// The cancellation token.
        /// </param>
        /// <exception cref="ErrorResponseException">
        /// Thrown when the operation returned an invalid status code
        /// </exception>
        /// <exception cref="Microsoft.Rest.SerializationException">
        /// Thrown when unable to deserialize the response
        /// </exception>
        /// <exception cref="Microsoft.Rest.ValidationException">
        /// Thrown when a required parameter is null
        /// </exception>
        Task<HttpOperationResponse<BatchArtifactContentInformationResult>> BatchGetByIdWithHttpMessagesAsync(System.Guid subscriptionId, string resourceGroupName, string workspaceName, ArtifactIdList artifactIds, Dictionary<string, List<string>> customHeaders = null, CancellationToken cancellationToken = default(CancellationToken));
        /// <summary>
        /// Get Batch Artifacts storage by Ids.
        /// </summary>
        /// <remarks>
        /// Get Batch Artifacts storage by specific Ids.
        /// </remarks>
        /// <param name='subscriptionId'>
        /// The Azure Subscription ID.
        /// </param>
        /// <param name='resourceGroupName'>
        /// The Name of the resource group in which the workspace is located.
        /// </param>
        /// <param name='workspaceName'>
        /// The name of the workspace.
        /// </param>
        /// <param name='artifactIds'>
        /// The list of artifactIds to get.
        /// </param>
        /// <param name='customHeaders'>
        /// The headers that will be added to request.
        /// </param>
        /// <param name='cancellationToken'>
        /// The cancellation token.
        /// </param>
        /// <exception cref="ErrorResponseException">
        /// Thrown when the operation returned an invalid status code
        /// </exception>
        /// <exception cref="Microsoft.Rest.SerializationException">
        /// Thrown when unable to deserialize the response
        /// </exception>
        /// <exception cref="Microsoft.Rest.ValidationException">
        /// Thrown when a required parameter is null
        /// </exception>
        Task<HttpOperationResponse<BatchArtifactContentInformationResult>> BatchGetStorageByIdWithHttpMessagesAsync(System.Guid subscriptionId, string resourceGroupName, string workspaceName, ArtifactIdList artifactIds, Dictionary<string, List<string>> customHeaders = null, CancellationToken cancellationToken = default(CancellationToken));
        /// <summary>
        /// Batch ingest using shared access signature.
        /// </summary>
        /// <remarks>
        /// Ingest Batch Artifacts using shared access signature.
        /// </remarks>
        /// <param name='subscriptionId'>
        /// The Azure Subscription ID.
        /// </param>
        /// <param name='resourceGroupName'>
        /// The Name of the resource group in which the workspace is located.
        /// </param>
        /// <param name='workspaceName'>
        /// The name of the workspace.
        /// </param>
        /// <param name='origin'>
        /// The origin of the Artifact.
        /// </param>
        /// <param name='container'>
        /// The container name.
        /// </param>
        /// <param name='artifactContainerSas'>
        /// The artifact container shared access signature to use for batch
        /// ingest.
        /// </param>
        /// <param name='customHeaders'>
        /// The headers that will be added to request.
        /// </param>
        /// <param name='cancellationToken'>
        /// The cancellation token.
        /// </param>
        /// <exception cref="ErrorResponseException">
        /// Thrown when the operation returned an invalid status code
        /// </exception>
        /// <exception cref="Microsoft.Rest.SerializationException">
        /// Thrown when unable to deserialize the response
        /// </exception>
        /// <exception cref="Microsoft.Rest.ValidationException">
        /// Thrown when a required parameter is null
        /// </exception>
        Task<HttpOperationResponse<PaginatedArtifactList>> BatchIngestFromSasWithHttpMessagesAsync(System.Guid subscriptionId, string resourceGroupName, string workspaceName, string origin, string container, ArtifactContainerSas artifactContainerSas, Dictionary<string, List<string>> customHeaders = null, CancellationToken cancellationToken = default(CancellationToken));
        /// <summary>
        /// Create a batch of empty Artifacts.
        /// </summary>
        /// <remarks>
        /// Create a Batch of empty Artifacts from the supplied paths.
        /// </remarks>
        /// <param name='subscriptionId'>
        /// The Azure Subscription ID.
        /// </param>
        /// <param name='resourceGroupName'>
        /// The Name of the resource group in which the workspace is located.
        /// </param>
        /// <param name='workspaceName'>
        /// The name of the workspace.
        /// </param>
        /// <param name='origin'>
        /// The origin of the Artifact.
        /// </param>
        /// <param name='container'>
        /// The container name.
        /// </param>
        /// <param name='artifactPaths'>
        /// The list of Artifact paths to create.
        /// </param>
        /// <param name='customHeaders'>
        /// The headers that will be added to request.
        /// </param>
        /// <param name='cancellationToken'>
        /// The cancellation token.
        /// </param>
        /// <exception cref="ErrorResponseException">
        /// Thrown when the operation returned an invalid status code
        /// </exception>
        /// <exception cref="Microsoft.Rest.SerializationException">
        /// Thrown when unable to deserialize the response
        /// </exception>
        /// <exception cref="Microsoft.Rest.ValidationException">
        /// Thrown when a required parameter is null
        /// </exception>
        Task<HttpOperationResponse<BatchArtifactContentInformationResult>> BatchCreateEmptyArtifactsWithHttpMessagesAsync(System.Guid subscriptionId, string resourceGroupName, string workspaceName, string origin, string container, ArtifactPathList artifactPaths, Dictionary<string, List<string>> customHeaders = null, CancellationToken cancellationToken = default(CancellationToken));
        /// <summary>
        /// Delete Batch of Artifact Metadata.
        /// </summary>
        /// <remarks>
        /// Delete a Batch of Artifact Metadata.
        /// </remarks>
        /// <param name='subscriptionId'>
        /// The Azure Subscription ID.
        /// </param>
        /// <param name='resourceGroupName'>
        /// The Name of the resource group in which the workspace is located.
        /// </param>
        /// <param name='workspaceName'>
        /// The name of the workspace.
        /// </param>
        /// <param name='origin'>
        /// The origin of the Artifact.
        /// </param>
        /// <param name='container'>
        /// The container name.
        /// </param>
        /// <param name='artifactPaths'>
        /// The list of Artifact paths to delete.
        /// </param>
        /// <param name='hardDelete'>
        /// If set to true, the delete cannot be reverted at a later time.
        /// </param>
        /// <param name='customHeaders'>
        /// The headers that will be added to request.
        /// </param>
        /// <param name='cancellationToken'>
        /// The cancellation token.
        /// </param>
        /// <exception cref="ErrorResponseException">
        /// Thrown when the operation returned an invalid status code
        /// </exception>
        /// <exception cref="Microsoft.Rest.ValidationException">
        /// Thrown when a required parameter is null
        /// </exception>
        Task<HttpOperationResponse> DeleteBatchMetaDataWithHttpMessagesAsync(System.Guid subscriptionId, string resourceGroupName, string workspaceName, string origin, string container, ArtifactPathList artifactPaths, bool? hardDelete = false, Dictionary<string, List<string>> customHeaders = null, CancellationToken cancellationToken = default(CancellationToken));
        /// <summary>
        /// Delete Artifact Metadata.
        /// </summary>
        /// <remarks>
        /// Delete Artifact Metadata in a specific container.
        /// </remarks>
        /// <param name='subscriptionId'>
        /// The Azure Subscription ID.
        /// </param>
        /// <param name='resourceGroupName'>
        /// The Name of the resource group in which the workspace is located.
        /// </param>
        /// <param name='workspaceName'>
        /// The name of the workspace.
        /// </param>
        /// <param name='origin'>
        /// The origin of the Artifact.
        /// </param>
        /// <param name='container'>
        /// The container name.
        /// </param>
        /// <param name='hardDelete'>
        /// If set to true. The delete cannot be revert at later time.
        /// </param>
        /// <param name='customHeaders'>
        /// The headers that will be added to request.
        /// </param>
        /// <param name='cancellationToken'>
        /// The cancellation token.
        /// </param>
        /// <exception cref="ErrorResponseException">
        /// Thrown when the operation returned an invalid status code
        /// </exception>
        /// <exception cref="Microsoft.Rest.ValidationException">
        /// Thrown when a required parameter is null
        /// </exception>
        Task<HttpOperationResponse> DeleteMetaDataInContainerWithHttpMessagesAsync(System.Guid subscriptionId, string resourceGroupName, string workspaceName, string origin, string container, bool? hardDelete = false, Dictionary<string, List<string>> customHeaders = null, CancellationToken cancellationToken = default(CancellationToken));
    }
}
