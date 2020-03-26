// <copyright file="StaticHelpers.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

using System;
using System.Globalization;
using System.Linq;
using System.Web;

namespace Azure.MachineLearning.Services
{
    public class StaticHelpers
    {
        public static T ParseEnum<T>(string input, bool ignoreCase, T defaultValue)
            where T : struct
        {
            Type type = typeof(T);
            if (!type.IsEnum)
            {
                throw new InvalidOperationException(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "Type {0} is not an Enum",
                        type));
            }

            if (Enum.TryParse<T>(input, ignoreCase, out T outValue))
            {
                return (T)outValue;
            }

            return defaultValue;
        }

        public static string GetEpochSeconds()
        {
            var result = (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc)).TotalSeconds;
            return Convert.ToInt64(result).ToString();
        }

        public static string GenerateUniqueRunId(string name)
        {
            const string format = "{0}_{1}_{2}";

            string newName = name == null ? Guid.NewGuid().ToString().Substring(0, 8) : name;
            return string.Format(format, newName, GetEpochSeconds(), Guid.NewGuid().ToString().Substring(0, 8));
        }

        public static string ExtractSingleQueryParamFromUrl(string urlString, string paramName)
        {
            Throw.IfNullOrEmpty(urlString, nameof(urlString));
            Throw.IfNullOrEmpty(paramName, nameof(paramName));

            var nextQueryParams = new Uri(urlString).Query;

            var queryDict = HttpUtility.ParseQueryString(nextQueryParams);
            if (queryDict.AllKeys.Count(x => x == paramName) == 1)
            {
                return queryDict[paramName];
            }
            else
            {
                var message = string.Format("Did not find exactly one instance of {0} in {1}", paramName, urlString);
                throw new ArgumentException(message);
            }
        }
    }
}
