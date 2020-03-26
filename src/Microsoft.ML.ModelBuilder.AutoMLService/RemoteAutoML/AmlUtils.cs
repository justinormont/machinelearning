// <copyright file="AmlUtils.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

using System;
using System.Net;
using System.Threading.Tasks;
using Azure.MachineLearning.Services;

namespace AzureML
{
    public static class AmlUtils
    {
        public static System.Net.HttpStatusCode? GetHttpStatusCode(Exception e)
        {
            if (e == null || e.InnerException == null || e.InnerException.GetType() != typeof(Azure.MachineLearning.Services.MachineLearningServiceException))
            {
                return null;
            }

            var mlServiceException = e.InnerException as MachineLearningServiceException;
            var retValue = mlServiceException.Response == null ? (HttpStatusCode?)null : mlServiceException.Response.StatusCode;

            return retValue;
        }

        public static bool Is404(Exception e)
        {
            var statusCode = GetHttpStatusCode(e);

            if (statusCode == null)
            {
                return false;
            }

            return statusCode.Value == System.Net.HttpStatusCode.NotFound;
        }

        public static string GetExceptionMessage(Exception e, string resourceType, string resourceName)
        {
            if (AmlUtils.Is404(e))
            {
                return $"{resourceType} \"{resourceName}\" not found.";
            }
            else
            {
                if (AmlUtils.GetHttpStatusCode(e).HasValue)
                {
                    return $"Error {GetHttpStatusCode(e)} while accessing {resourceType} {resourceName}.";
                }
                else
                {
                    return $"Error \"{e.Message}\" while accessing {resourceType} {resourceName}.";
                }
            }
        }

#pragma warning disable MSML_GeneralName // This name should be PascalCased
        public static T CallAMLAndHandleExceptions<T>(Func<T> f, string resourceType, string resourceName)
#pragma warning restore MSML_GeneralName // This name should be PascalCased
        {
            try
            {
                return f();
            }
            catch (Exception e)
            {
                Console.WriteLine(AmlUtils.GetExceptionMessage(e, resourceType, resourceName));
                throw;
            }
        }

#pragma warning disable MSML_GeneralName // This name should be PascalCased
        public static async Task<T> CallAMLAndHandleExceptionsAsync<T>(Func<Task<T>> f, string resourceType, string resourceName)
#pragma warning restore MSML_GeneralName // This name should be PascalCased
        {
            try
            {
                return await f();
            }
            catch (Exception e)
            {
                Console.WriteLine(AmlUtils.GetExceptionMessage(e, resourceType, resourceName));
                throw;
            }
        }
    }
}
