// <copyright file="DockerImageName.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

/*
 * This file is copied from
 * Vienna/src/azureml-api/src/EnvironmentManagement/Contracts/DockerImageName.cs
 * This is a temporary solution until we can get the Swagger files updated
 * */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Azure.MachineLearning.Services
{
    [JsonConverter(typeof(DockerImageNameJsonConverter))]
    [TypeConverter(typeof(DockerImageNameStringConverter))]
    public class DockerImageName
    {
        public DockerImageName(string imageName)
        {
            // Parsing details: https://stackoverflow.com/questions/37861791/how-are-docker-image-names-parsed

            Repository = imageName;

            // The first slash-delimited component is a hostname if and only if it contains a dot or a colon.
            var slashed = imageName.Split(new char[] { '/' }, 2);
            if (slashed.Length > 1 && (slashed[0].Contains(".") || slashed[0].Contains(":")))
            {
                Hostname = slashed[0];
                Repository = slashed[1];
            }

            // Both a digest and a tag can be specified; the digest takes priority and follows the tag.
            var digested = Repository.Split(new char[] { '@' }, 2);
            if (digested.Length > 1)
            {
                Repository = digested[0];
                Digest = digested[1];
            }

            // Aside from a port specifier in the hostname, only one colon is allowed in the image name and it signals a tag.
            var pieces = Repository.Split(new char[] { ':' }, 2);
            if (pieces.Length > 1)
            {
                Repository = pieces[0];
                Tag = pieces[1];
            }
        }

        public string Hostname { get; set; }

        public string Repository { get; set; }

        public string Tag { get; set; }

        public string Digest { get; set; }

        public string FullyQualified
        {
            get
            {
                var result = Repository;
                if (Hostname != null)
                {
                    result = $"{Hostname}/" + result;
                }

                if (Tag != null)
                {
                    result += $":{Tag}";
                }

                if (Digest != null)
                {
                    result += $"@{Digest}";
                }

                return result;
            }
        }

        public static implicit operator DockerImageName(string imageName)
        {
            return new DockerImageName(imageName);
        }

        public static implicit operator string(DockerImageName imageName)
        {
            return imageName.FullyQualified;
        }

        public override string ToString()
        {
            return this;
        }
    }

    public class DockerImageNameStringConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is string)
            {
                return new DockerImageName((string)value);
            }
            return base.ConvertFrom(context, culture, value);
        }
    }

    public class DockerImageNameJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(DockerImageName);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (objectType != typeof(DockerImageName))
            {
                throw new ArgumentException($"Unsupported type {objectType}");
            }

            if (reader.TokenType == JsonToken.Null)
            {
                return null;
            }

            // Load JToken from stream
            JToken token = JToken.Load(reader);

            return new DockerImageName((string)token);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var imageName = (DockerImageName)value;
            serializer.Serialize(writer, (string)imageName);
        }
    }
}
