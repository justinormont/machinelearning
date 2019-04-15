// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using Microsoft.ML.Data;

namespace Microsoft.ML.Auto
{
    internal static class SuggestedPipelineBuilder
    {
        public static SuggestedPipeline Build(MLContext context,
            IDataView data,
            ICollection<SuggestedTransform> transforms,
            ICollection<SuggestedTransform> transformsPostTrainer,
            SuggestedTrainer trainer,
            bool? enableCaching)
        {
            var trainerInfo = trainer.BuildTrainer().Info;
            AddNormalizationTransforms(context, trainerInfo, transforms, new string[] { DefaultColumnNames.Features}, data);
            var cacheBeforeTrainer = ShouldCacheBeforeTrainer(trainerInfo, enableCaching);
            return new SuggestedPipeline(transforms, transformsPostTrainer, trainer, context, cacheBeforeTrainer);
        }

        private static void AddNormalizationTransforms(MLContext context,
            TrainerInfo trainerInfo,
            ICollection<SuggestedTransform> transforms)
        {
            // Only add normalization if trainer needs it
            if (!trainerInfo.NeedNormalization)
            {
                return;
            }

            var transform = NormalizingExtension.CreateSuggestedTransform(context, DefaultColumnNames.Features, DefaultColumnNames.Features);
            transforms.Add(transform);
        }

        private static void AddNormalizationTransforms(MLContext context,
           TrainerInfo trainerInfo,
           ICollection<SuggestedTransform> transforms, string[] columnNames, IDataView data = null)
        {
            var columnNamesToNormalize = new List<string>(columnNames.Length);

            // Only add normalization if trainer needs it
            if (!trainerInfo.NeedNormalization)
            {
                return;
            }

            // If an IDataView is supplied, check if each column needs normalization
            if (data != null)
            {
                var e = transforms.GetEnumerator();
                while (e.MoveNext()) { };
                var preview = e.Current.Estimator.Preview(data);

                if (columnNames?.Length > 0)
                {
                    foreach (string columnName in columnNames)
                    {
                        if (!preview.Schema[columnName].IsNormalized())
                        {
                            columnNamesToNormalize.Add(columnName);
                        }
                    }

                }
            }
            else
            {
                columnNamesToNormalize = new List<string>(columnNames);
            }

            // todo: Move to multi-column version of NormalizeMinMax so we are not taking N passes of the dataset
            foreach (string columnName in columnNamesToNormalize)
            {
                var transform = NormalizingExtension.CreateSuggestedTransform(context, columnName, columnName);
                transforms.Add(transform);
            }


        }

        private static bool ShouldCacheBeforeTrainer(TrainerInfo trainerInfo, bool? enableCaching)
        {
            return enableCaching == true || (enableCaching == null && trainerInfo.WantCaching);
        }
    }
}
