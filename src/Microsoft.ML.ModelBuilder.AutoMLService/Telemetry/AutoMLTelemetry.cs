// <copyright file="AutoMLTelemetry.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Microsoft.ML.AutoML;
using Microsoft.ML.ModelBuilder.AutoMLService.Contract;

namespace Microsoft.ML.ModelBuilder.AutoMLService.Telemetry
{
    internal class AutoMLTelemetry
    {
        private AutoMLTelemetry()
        {
        }

        public event EventHandler<AutoMLTelemetryEvent> AutoMLTelemetryEventHandler;

        public static AutoMLTelemetry Instance { get; set; } = new AutoMLTelemetry();

        public static string GetSanitizedPipelineStr(Pipeline pipeline)
        {
            if (pipeline?.Nodes == null)
            {
                return null;
            }

            var transformNodes = pipeline.Nodes.Where(n => n.NodeType == PipelineNodeType.Transform);
            var trainerNode = pipeline.Nodes.FirstOrDefault(n => n.NodeType == PipelineNodeType.Trainer);
            var sb = new StringBuilder();
            foreach (var transformNode in transformNodes)
            {
                sb.Append(transformNode.Name);
                sb.Append(",");
            }

            if (trainerNode != null)
            {
                sb.Append(trainerNode.Name);
                sb.Append("{");
                var serializedHyperparams = trainerNode.Properties
                    .Where(p => SweepableParams.AllHyperparameterNames.Contains(p.Key))
                    .Select(p => $"{p.Key}: {p.Value}");
                sb.Append(string.Join(", ", serializedHyperparams));
                sb.Append("}");
            }

            return sb.ToString();
        }

        public void TrackEvent(
            string eventName,
            IDictionary<string, string> properties,
            TimeSpan? duration = null,
            Exception ex = null)
        {
            try
            {
                var eventProperties = GetEventProperties(properties, duration, ex);
                var telemetryEvent = new AutoMLTelemetryEvent();
                telemetryEvent.Properties = eventProperties;
                telemetryEvent.EventName = eventName;
                this.OnAutoMLTelemetryEvent(null, telemetryEvent);
            }
            catch (Exception e)
            {
                Debug.Fail(e.ToString());
            }
        }

        public void OnAutoMLTelemetryEvent(object sender, AutoMLTelemetryEvent e)
        {
            this.AutoMLTelemetryEventHandler?.Invoke(sender, e);
        }

        private static Dictionary<string, string> GetEventProperties(
            IDictionary<string, string> properties,
            TimeSpan? duration,
            Exception ex)
        {
            var eventProperties = new Dictionary<string, string>();

            if (duration != null)
            {
                eventProperties["Duration"] = duration.Value.TotalMilliseconds.ToString();
            }

            if (ex != null)
            {
                eventProperties["Exception"] = GetSanitizedExceptionStr(ex);
            }

            if (properties != null)
            {
                foreach (KeyValuePair<string, string> property in properties)
                {
                    if (property.Value != null)
                    {
                        eventProperties[property.Key] = property.Value;
                    }
                }
            }

            return eventProperties;
        }

        private static string GetSanitizedExceptionStr(Exception ex)
        {
            return $@"{ex.GetType()}
{ex.StackTrace}";
        }
    }
}