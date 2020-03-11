// <copyright file="RunConfiguration.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;

/*
 * This file is copied from
 * Vienna/src/azureml-api/src/Execution/Contracts/RunConfiguration.cs
 * This is a temporary solution until we can get the Swagger files updated
 * */

namespace Azure.MachineLearning.Services
{
    public enum Framework
    {
        Python,
        PySpark,
        Cntk,
        TensorFlow,
        PyTorch,
        [Obsolete("Replaced by new schema, use Communicator.ParameterServer")]
        TensorFlowParameterServer,
        [Obsolete("Replaced by new schema, use Communicator.OpenMpi")]
        PythonMpi,
        [Obsolete("Replaced by new schema, use Communicator.IntelMpi")]
        PythonIntelMpi,
        PySparkInteractive
    }

    public enum Communicator
    {
        None,
        ParameterServer,
        [Obsolete("Use Communicator.Mpi instead")]
        OpenMpi,
        [Obsolete("Use Communicator.Mpi instead")]
        IntelMpi,
        Gloo,
        Mpi
    }

    public enum YarnDeployMode
    {
        None,
        Client,
        Cluster
    }

    public enum SparkMasterMode
    {
        None,
        Local,
        Yarn,
        Standalone
    }

    public enum DataStoreMode
    {
        Mount,
        Download,
        Upload
    }

    public class HistoryConfiguration
    {
        public bool OutputCollection { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore)]
        [System.ComponentModel.DefaultValue(new string[] { "logs" })]
        public List<string> DirectoriesToWatch { get; set; }

        [JsonExtensionData]
        public IDictionary<string, JToken> ExtraConfiguration { get; set; }
    }

    public class SparkConfiguration
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<string, string> Configuration { get; set; } = new Dictionary<string, string>();
    }

    [Obsolete("Moved its properties to RunConfiguration")]
    public class BatchAiConfiguration
    {
        [Obsolete("Moved to RunConfiguration")]
        public int NodeCount { get; set; }
    }

    public class AMLComputeConfiguration
    {
        public string Name { get; set; }

        public string VmSize { get; set; }

        public string VmPriority { get; set; }

        public bool RetainCluster { get; set; }

        public int ClusterMaxNodeCount { get; set; } = 1;
    }

    public class TensorflowConfiguration
    {
        /// <summary>
        /// The number of workers.
        /// </summary>
        public int WorkerCount { get; set; }

        /// <summary>
        /// Number of parameter servers.
        /// </summary>
        public int ParameterServerCount { get; set; }
    }

    public class MpiConfiguration
    {
        /// <summary>
        /// Number of processes per node.
        /// </summary>
        public int ProcessCountPerNode { get; set; }
    }

    public class HdiConfiguration
    {
        public YarnDeployMode YarnDeployMode { get; set; }
    }

    public class DataReferenceConfiguration
    {
        public string DataStoreName { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public DataStoreMode Mode { get; set; }

        public string PathOnDataStore { get; set; }

        public string PathOnCompute { get; set; }

        public bool Overwrite { get; set; }
    }

    public class ContainerInstanceConfiguration
    {
        // Defaults to the region of the workspace.
        public string Region { get; set; }

        // Default size corresponds to the largest container supported in all regions.
        // Details: https://docs.microsoft.com/en-us/azure/container-instances/container-instances-quotas
        public double CpuCores { get; set; } = 2;

        public double MemoryGb { get; set; } = 3.5;
    }

    public class RunConfiguration
    {
        public string Script { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<string> Arguments { get; set; } = new List<string>();

        public string SourceDirectoryDataStore { get; set; }

        public Framework Framework { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public Communicator? Communicator { get; set; }

        public string Target { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<string, DataReferenceConfiguration> DataReferences { get; set; } = new Dictionary<string, DataReferenceConfiguration>();

        // Defaults to ArgumentVector[0] if not specified.
        // This is primarily intended for notebooks to override the default job name.
        public string JobName { get; set; }

        public bool? AutoPrepareEnvironment { get; set; }

        // MaxRunDurationSeconds is in seconds. MaxRunDurationSeconds=null means indefinite duration.
        public long? MaxRunDurationSeconds { get; set; }

        // Number of compute nodes to run the job on. BatchAI only.
        public int? NodeCount { get; set; }

        // TODO Also add an EnvironmentFile property to allow this section to be optionally
        // sourced from a separate project configuration file.
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public EnvironmentDefinition Environment { get; set; } = new EnvironmentDefinition();

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public HistoryConfiguration History { get; set; } = new HistoryConfiguration();

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public SparkConfiguration Spark { get; set; } = new SparkConfiguration();

        [Obsolete("Moved its properties to RunConfiguration")]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public BatchAiConfiguration BatchAi { get; set; } = new BatchAiConfiguration();

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public AMLComputeConfiguration AmlCompute { get; set; } = new AMLComputeConfiguration();

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public TensorflowConfiguration Tensorflow { get; set; } = new TensorflowConfiguration();

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public MpiConfiguration Mpi { get; set; } = new MpiConfiguration();

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public HdiConfiguration Hdi { get; set; } = new HdiConfiguration();

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public ContainerInstanceConfiguration ContainerInstance { get; set; } = new ContainerInstanceConfiguration();

        // TODO Currently unused; either deprecate it or move it to an appropriate location.
        // This was added to support the remote notebook kernel scenarios, where a port needed to be exposed
        // from inside a Docker container so that the notebook server could connect to the remote kernel.
        public List<int> ExposedPorts { get; set; }
    }
}
