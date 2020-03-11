// <copyright file="LabelMapping.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ML.Data;
using Microsoft.ML.Transforms;

namespace Microsoft.ML.ModelBuilder.AutoMLService.RemoteAutoML
{
    [CustomMappingFactoryAttribute(nameof(LabelMapping))]
    public class LabelMapping : CustomMappingFactory<LabelMappingInput, LabelMappingOutput>
    {
        public static string[] Label { get; set; }

        // This is the custom mapping. We now separate it into a method, so that we can use it both in training and in loading.
        public static void Mapping(LabelMappingInput input, LabelMappingOutput output)
        {
            var values = input.Output1.GetValues().ToArray();
            var maxVal = values.Max();
            var exp = values.Select(v => Math.Exp(v - maxVal));
            var sumExp = exp.Sum();

            exp.Select(v => (float)(v / sumExp)).ToArray();
            output.Score = exp.Select(v => (float)(v / sumExp)).ToArray();

            var maxValue = output.Score.Max();
            var maxValueIndex = Array.IndexOf(output.Score, maxValue);
            output.Label = Label[maxValueIndex];
        }

        // This factory method will be called when loading the model to get the mapping operation.
        public override Action<LabelMappingInput, LabelMappingOutput> GetMapping()
        {
            return Mapping;
        }
    }

    public class LabelMappingInput
    {
        [ColumnName("output1")]
        public VBuffer<float> Output1;
    }

    public class LabelMappingOutput
    {
        [ColumnName("PredictedLabel")]
        public string Label { get; set; }

        [ColumnName("Score")]
        public float[] Score { get; set; }
    }
}
