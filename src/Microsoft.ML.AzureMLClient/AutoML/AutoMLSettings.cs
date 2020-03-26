// <copyright file = "AutoMLSettings.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

using System.IO;
using System.Text;
using Newtonsoft.Json;

namespace Azure.MachineLearning.Services.AutoML
{
    public class AutoMLSettings
    {
        public AutoMLSettings(
            int iterationTimeoutInMin,
            int iterations,
            string primaryMetric,
            bool preprocess,
            int nCrossValidations)
        {
            Throw.IfNullOrEmpty(primaryMetric, nameof(primaryMetric));

            Throw.IfValueNotPositive(iterationTimeoutInMin, nameof(iterationTimeoutInMin));
            Throw.IfValueNotPositive(iterations, nameof(iterations));
            Throw.IfValueNotPositive(nCrossValidations, nameof(nCrossValidations));

            IterationTimeoutMinutes = iterationTimeoutInMin;
            Iterations = iterations;
            PrimaryMetric = primaryMetric;
            Preprocess = preprocess;
            NCrossValidations = nCrossValidations;
        }

        [JsonProperty(PropertyName = "iteration_timeout_minutes")]
        public int IterationTimeoutMinutes { get; set; }

        [JsonProperty(PropertyName = "iterations")]
        public int Iterations { get; set; }

        [JsonProperty(PropertyName = "task_type")]
        public string TaskType { get; set; }

        [JsonProperty(PropertyName = "primary_metric")]
        public string PrimaryMetric { get; set; }

        [JsonProperty(PropertyName = "preprocess")]
        public bool Preprocess { get; set; }

        [JsonProperty(PropertyName = "n_cross_validations")]
        public int NCrossValidations { get; set; }

        [JsonProperty(PropertyName = "validation_size")]
        public double ValidationSize { get; set; }

        [JsonProperty(PropertyName = "enable_subsampling")]
        public bool EnableSubsampling { get; set; }

        [JsonProperty(PropertyName = "enable_onnx_compatible_models")]
        public bool EnableOnnxCompatibleModels { get; set; }

        [JsonProperty(PropertyName = "enable_tf")]
        public bool EnableTensorFlow { get; set; }

        [JsonProperty(PropertyName = "debug_log")]
        public string DebugLog { get; set; } = "automl_errors.log";

        [JsonProperty(PropertyName = "images_folder")]
        public string ImagesFolder { get; set; } = ".";

        [JsonProperty(PropertyName = "enable_dnn")]
        public bool EnableDnn { get; set; }

        [JsonProperty(PropertyName = "labels_file")]
        public string LabelsFile { get; set; }

        [JsonProperty(PropertyName = "epochs")]
        public int Epochs { get; set; }

        [JsonProperty(PropertyName = "compute_target")]
        public string ComputeTarget { get; set; }

        internal string ToJsonString()
        {
            return JsonConvert.SerializeObject(this);
        }

        /// <summary>
        /// JOS has an issue where we have to pass settings twice, once in normal json and once in Python, meaning 
        /// single quotes and True/False instead of true/false -- this takes care of producing a string in that format
        /// </summary>
        internal string ToPythonString()
        {
            var sb = new StringBuilder();

            using (var sw = new StringWriter(sb))
            using (var writer = new JsonTextWriter(sw))
            {
                writer.QuoteChar = '\'';

                var jserializer = JsonSerializer.Create(
                    new JsonSerializerSettings
                    {
                        Converters = new JsonConverter[] { new FirstCapBooleanJsonConverter() }
                    });

                jserializer.Serialize(writer, this);

                return sw.ToString();
            }
        }
    }
}