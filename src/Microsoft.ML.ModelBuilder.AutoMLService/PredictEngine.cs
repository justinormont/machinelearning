// <copyright file="PredictEngine.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data;
using Microsoft.ML.Data;
using Microsoft.ML.ModelBuilder.AutoMLService.RemoteAutoML;
using Newtonsoft.Json;

namespace Microsoft.ML.ModelBuilder.AutoMLService
{
    internal class PredictEngine : IPredictEngine
    {
        public ITransformer Model { get; set; }

        public string BestModelMap { get; set; }

        public DataViewSchema InputSchema { get; set; }

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        public async Task<KeyValuePair<string, float>> PredictBinaryClassificationAsync(IDictionary<string, object> values, string predictedLabelColumnName, string scoreColumnName)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            var inputDataView = this.IDictionaryToIDataView(values, this.InputSchema);
            var resultDataView = this.Model?.Transform(inputDataView);
            var score = this.ExtractColumnListFromIDataView<float>(resultDataView, scoreColumnName).First();
            var predictedLabel = this.ExtractColumnListFromIDataView<bool>(resultDataView, predictedLabelColumnName).First();

            return new KeyValuePair<string, float>(predictedLabel.ToString(), score);
        }

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        public async Task<IEnumerable<KeyValuePair<string, float>>> PredictMultiClassClassificationAsync(IDictionary<string, object> values, string labelColumnName, string scoreColumnName)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            var inputDataView = this.IDictionaryToIDataView(values, this.InputSchema);
            var resultDataView = this.Model?.Transform(inputDataView);

            var scores = this.ExtractColumnListFromIDataView<float[]>(resultDataView, scoreColumnName).First();

            var keyMap = this.GetLabelMap(this.Model, inputDataView, labelColumnName);

            return this.CombineAndSortMultiClassLabelAndScores(keyMap, scores);
        }

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        public async Task<KeyValuePair<string, float>> PredictRegressionAsync(IDictionary<string, object> values, string labelColumnName, string scoreColumnName = "Score")
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            var inputDataView = this.IDictionaryToIDataView(values, this.InputSchema);
            var resultDataView = this.Model?.Transform(inputDataView);
            var score = this.ExtractColumnListFromIDataView<float>(resultDataView, scoreColumnName).First();

            return new KeyValuePair<string, float>(labelColumnName, score);
        }

        public async Task<KeyValuePair<string, float>> PredictRecommendationAsync(IDictionary<string, object> values, string labelColumnName, string scoreColumnName = "Score")
        {
            var inputDataView = this.IDictionaryToIDataView(values, this.InputSchema);
            var resultDataView = this.Model?.Transform(inputDataView);
            var score = this.ExtractColumnListFromIDataView<float>(resultDataView, scoreColumnName).First();

            return new KeyValuePair<string, float>(labelColumnName, score);
        }

        public async Task<IEnumerable<KeyValuePair<string, float>>> PredictRecommendationBatchAsync(IEnumerable<IDictionary<string, object>> values, string labelColumnName, string scoreColumnName = "Score")
        {
            var results = new List<KeyValuePair<string, float>>();

            foreach (var value in values)
            {
                results.Add(await this.PredictRecommendationAsync(value, labelColumnName, scoreColumnName));
            }

            return results;
        }

        private IDataView IDictionaryToIDataView(IDictionary<string, object> dictionary, DataViewSchema schema)
        {
            return new DataFrame(schema.AsEnumerable().Where(x => dictionary.ContainsKey(x.Name)).Select(x => x.ToBaseColumn(dictionary[x.Name])).ToList());
        }

        private IList<T> ExtractColumnListFromIDataView<T>(IDataView dataView, string columnName)
        {
            var column = dataView.Schema[columnName];
            return this.GetColumnValueAsList<T>(dataView, column);
        }

        private IList<string> ExtractColumnFromIDataViewAsStringArray(IDataView dataView, string columnName)
        {
            var column = dataView.Schema[columnName];
            var labelColumnType = column.Type.RawType;
            var getColumnListMethod = typeof(PredictEngine).GetMethod("GetFirstColumnValueAsStringArray", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            var getColumnListGenericMethod = getColumnListMethod.MakeGenericMethod(labelColumnType);
            return getColumnListGenericMethod.Invoke(this, new object[] { dataView, column }) as IList<string>;
        }

        private List<string> GetFirstColumnValueAsStringArray<T>(IDataView res, DataViewSchema.Column column)
        {
            return this.GetColumnValueAsList<T>(res, column).Select(x => x.ToString()).ToList();
        }

        private List<T> GetColumnValueAsList<T>(IDataView res, DataViewSchema.Column column)
        {
            return res.GetColumn<T>(column).ToList();
        }

        private Dictionary<int, string> GetLabelMap(ITransformer model, IDataView dataView, string labelColumnName)
        {
            // if Azure image, use label from LabelMapping
            if (LabelMapping.Label != null)
            {
                return LabelMapping.Label.Select((labelAsChars, index) => new { index, label = labelAsChars.ToString() }).ToDictionary(x => x.index, x => x.label);
            }

            var outputSchema = model.GetOutputSchema(dataView.Schema)[labelColumnName];
            var labelColumnType = dataView.Schema[labelColumnName].Type.RawType;
            var getInnerGetLabelMapMethod = typeof(PredictEngine).GetMethod("InnerGetLabelMap", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).MakeGenericMethod(labelColumnType);
            return getInnerGetLabelMapMethod.Invoke(this, new object[] { outputSchema }) as Dictionary<int, string>;
        }

        private Dictionary<int, string> InnerGetLabelMap<TSource>(DataViewSchema.Column column)
        {
            VBuffer<TSource> keys = default;
            column.GetKeyValues(ref keys);
            return keys.DenseValues().Select((labelAsChars, index) => new { index, label = labelAsChars.ToString() }).ToDictionary(x => x.index, x => x.label);
        }

        private IEnumerable<KeyValuePair<string, float>> CombineAndSortMultiClassLabelAndScores(Dictionary<int, string> labelMap, IList<float> scores)
        {
            return scores.Select((score, keyIndex) => new KeyValuePair<string, float>(labelMap[keyIndex], score)).OrderByDescending(x => x.Value);
        }
    }
}
