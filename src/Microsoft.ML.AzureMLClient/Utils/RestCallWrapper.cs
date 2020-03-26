// <copyright file="RestCallWrapper.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Rest;

namespace Azure.MachineLearning.Services
{
    internal static class RestCallWrapper
    {
        internal static async Task WrapAsync(Func<Task<HttpOperationResponse>> call)
        {
            try
            {
                await call().ConfigureAwait(false);
            }
            catch (GeneratedOld.Models.ErrorResponseException ex)
            {
                throw MachineLearningServiceException.FromErrorResponseException(ex);
            }
            catch (Generated.Models.ErrorResponseException ex)
            {
                throw MachineLearningServiceException.FromErrorResponseException(ex);
            }
        }

        internal static async Task<TBody> WrapAsync<TBody>(Func<Task<HttpOperationResponse<TBody>>> call)
        {
            try
            {
                var response = await call().ConfigureAwait(false);
                return response.Body;
            }
            catch (GeneratedOld.Models.ErrorResponseException ex)
            {
                throw MachineLearningServiceException.FromErrorResponseException(ex);
            }
            catch (Generated.Models.ErrorResponseException ex)
            {
                throw MachineLearningServiceException.FromErrorResponseException(ex);
            }
        }

        internal static async Task<TBody> WrapAsync<TBody, THeader>(Func<Task<HttpOperationResponse<TBody, THeader>>> call)
        {
            try
            {
                var response = await call().ConfigureAwait(false);
                return response.Body;
            }
            catch (GeneratedOld.Models.ErrorResponseException ex)
            {
                throw MachineLearningServiceException.FromErrorResponseException(ex);
            }
            catch (Generated.Models.ErrorResponseException ex)
            {
                throw MachineLearningServiceException.FromErrorResponseException(ex);
            }
        }

        internal static async Task<THeader> WrapAsync<THeader>(Func<Task<HttpOperationHeaderResponse<THeader>>> call)
        {
            try
            {
                var response = await call().ConfigureAwait(false);
                return response.Headers;
            }
            catch (GeneratedOld.Models.ErrorResponseException ex)
            {
                throw MachineLearningServiceException.FromErrorResponseException(ex);
            }
            catch (Generated.Models.ErrorResponseException ex)
            {
                throw MachineLearningServiceException.FromErrorResponseException(ex);
            }
        }
    }
}
