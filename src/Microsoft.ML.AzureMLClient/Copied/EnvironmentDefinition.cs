// <copyright file="EnvironmentDefinition.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

/*
 * This file is copied from
 * Vienna/src/azureml-api/src/EnvironmentManagement/Contracts/EnvironmentDefinition.cs
 * This is a temporary solution until we can get the Swagger files updated
 * */

namespace Azure.MachineLearning.Services
{
    public class ContainerRegistry
    {
        public string Address { get; set; }

        public string Username { get; set; }

        [SecretValue]
        public string Password { get; set; }
    }

    public class SparkMavenPackage
    {
        public string Group { get; set; }

        public string Artifact { get; set; }

        public string Version { get; set; }
    }

    public class EnvironmentDefinition
    {
        // Read-only from a contract perspective; set with URI fields on the relevant APIs.
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Name { get; set; }

        // Read-only from a contract perspective; set with URI fields on the relevant APIs.
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public int Version { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public PythonSection Python { get; set; } = new PythonSection();

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<string, string> EnvironmentVariables { get; set; } = new Dictionary<string, string>();

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public DockerSection Docker { get; set; } = new DockerSection();

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public SparkSection Spark { get; set; } = new SparkSection();

        public class PythonSection
        {
            public string InterpreterPath { get; set; }

            public bool UserManagedDependencies { get; set; }

            // An inline JSONified version of the Conda dependencies YAML structure. It is intentionally nullable in the contract rather than
            // just empty, because null indicates that the contents should be read from the project files instead.
            public JToken CondaDependencies { get; set; }

            public string BaseCondaEnvironment { get; set; }
        }

        public class DockerSection
        {
            public DockerImageName BaseImage { get; set; }

            public bool Enabled { get; set; }

            public bool SharedVolumes { get; set; } = true;

            public Preparation Preparation { get; set; }

            public bool GpuSupport { get; set; }

            // 1GB is nvidia's recommended default shm size. In testing, more was not needed.
            public string ShmSize { get; set; } = "1g";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public List<string> Arguments { get; set; } = new List<string>();

            // Defaults to null, not an empty structure.
            public ContainerRegistry BaseImageRegistry { get; set; }
        }

        public class SparkSection
        {
            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public List<string> Repositories { get; set; } = new List<string>();

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public List<SparkMavenPackage> Packages { get; set; } = new List<SparkMavenPackage>();

            public bool PrecachePackages { get; set; } = true;
        }

        public class Preparation
        {
            public string CommandLine { get; set; }
        }
    }
}
