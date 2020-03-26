// <copyright file="AzureWebRequest.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Azure.MachineLearning.Services.GeneratedOld.Models;
using Microsoft.ML.ModelBuilder.AutoMLService;
using Newtonsoft.Json;

// Originally copied from CloudExplorer - https://devdiv.visualstudio.com/DevDiv/_git/CloudExplorer?path=/src/CloudHub.VS.Host/Azure/AzureRequest.cs
namespace Azure.MachineLearning.Services.AMLRest
{
    /// <summary>
    /// Handle calling the REST API.
    /// </summary>
    internal static class AzureWebRequest
    {
        public static async Task<string> GetResponseAsync(string url, string token, string method, Dictionary<string, string> headers = null, string body = null)
        {
            using (var response = await GetResponseInternalAsync(url, token, method, headers, body))
            {
                using (var responseStream = new StreamReader(response.GetResponseStream()))
                {
                    return await responseStream.ReadToEndAsync();
                }
            }
        }

        private static async Task<HttpWebResponse> GetResponseInternalAsync(string url, string token, string method, Dictionary<string, string> headers = null, string body = null)
        {
            HttpWebResponse response = null;
            int backOffMilliseconds = 250;
            TimeSpan maxRetryElapseTime = TimeSpan.FromMinutes(2);
            DateTime startTime = DateTime.Now;

            while (response == null)
            {
                HttpWebRequest request = await CreateRequestAsync(url, token, method, headers, body);
                try
                {
                    response = (HttpWebResponse)await request.GetResponseAsync();

                    if (response.StatusCode >= (HttpStatusCode)300)
                    {
                        throw new Exception("Error getting response from Azure");
                    }
                }
                catch (WebException e)
                {
                    var httpResponse = e.Response as HttpWebResponse;
                    List<int> retriableStatusCodes = new List<int>(2) { 429 /* Too many requests */, 503 /* Service Unavailable */ };

                    if (httpResponse != null && retriableStatusCodes.Contains((int)httpResponse.StatusCode)
                        && DateTime.Now - startTime < maxRetryElapseTime)
                    {
                        // Retry on retriable errors
                        backOffMilliseconds *= 2;
                        await Task.Delay(backOffMilliseconds);
                    }
                    else
                    {
                        using (var responseStream = new StreamReader(e.Response.GetResponseStream()))
                        {
                            var resultBody = await responseStream.ReadToEndAsync();
                            var error = JsonConvert.DeserializeObject<WebRequestError>(resultBody);

                            // Combine the exception message with more details from the web
                            var fullMessage = string.Concat(e.Message, " ", error.Error.Message);

                            throw new Exception(fullMessage);
                        }
                    }
                }
            }

            return response;
        }

        /// <summary>
        /// Creates an Azure Web Request.
        /// </summary>
        private static async Task<HttpWebRequest> CreateRequestAsync(string url, string token, string method, Dictionary<string, string> headers = null, string body = null)
        {
            const string requestContentType = "application/json; charset=utf-8";

            // Prepare request
            var request = (HttpWebRequest)HttpWebRequest.Create(url);
            request.Method = method;

            if (headers != null)
            {
                foreach (var header in headers)
                {
                    request.Headers.Add(header.Key, header.Value);
                }
            }

            request.Headers.Add(HttpRequestHeader.Authorization, token);
            request.Accept = requestContentType;

            // Add content information
            request.ContentType = requestContentType;

            // Add content only if there is any body
            if (!string.IsNullOrEmpty(body))
            {
                var encoding = new UTF8Encoding();
                var contentBytes = encoding.GetBytes(body);
                using (var stream = await request.GetRequestStreamAsync())
                {
                    await stream.WriteAsync(contentBytes, 0, contentBytes.Length);
                }
            }
            else
            {
                request.ContentLength = 0;
            }

            return request;
        }
    }
}
