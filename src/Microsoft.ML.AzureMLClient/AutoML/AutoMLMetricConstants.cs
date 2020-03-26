// <copyright file="AutoMLMetricConstants.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

using System.Collections.Generic;

namespace Azure.MachineLearning.Services.AutoML
{
    public static class AutoMLMetricConstants
    {
        internal static readonly ISet<string> ClassificationMetrics = new HashSet<string>
        {
            AUCMacro,
            AUCMicro,
            AUCWeighted,
            Accuracy,
            WeightedAccuracy,
            BalancedAccuracy,
            NormMacroRecall,
            LogLoss,
            F1Micro,
            F1Macro,
            F1Weighted,
            PrecisionMacro,
            PrecisionMicro,
            PrecisionWeighted,
            RecallMacro,
            RecallMicro,
            RecallWeighted,
            AvgPrecisionMacro,
            AvgPrecisionMicro,
            AvgPrecisionWeighted,
            AccuracyTable,
            ConfusionMatrix,
        };

        internal static readonly ISet<string> RegressionMetrics = new HashSet<string>
        {
            ExplainedVariance,
            R2Score,
            Spearman,
            MAPE,
            MeanAbsError,
            MedianAbsError,
            RMSE,
            RMSLE,
            NormMeanAbsError,
            NormMedianAbsError,
            NormRMSE,
            NormRMSLE,
            Residuals,
            PredictedTrue,
        };

        // Classification
        internal static string AUCMacro => "AUC_macro";

        internal static string AUCMicro => "AUC_micro";

        internal static string AUCWeighted => "AUC_weighted";

        internal static string Accuracy => "accuracy";

        internal static string WeightedAccuracy => "weighted_accuracy";

        internal static string BalancedAccuracy => "balanced_accuracy";

        internal static string NormMacroRecall => "norm_macro_recall";

        internal static string LogLoss => "log_loss";

        internal static string F1Micro => "f1_score_micro";

        internal static string F1Macro => "f1_score_macro";

        internal static string F1Weighted => "f1_score_weighted";

        internal static string PrecisionMicro => "precision_score_micro";

        internal static string PrecisionMacro => "precision_score_macro";

        internal static string PrecisionWeighted => "precision_score_weighted";

        internal static string RecallMicro => "recall_score_micro";

        internal static string RecallMacro => "recall_score_macro";

        internal static string RecallWeighted => "recall_score_weighted";

        internal static string AvgPrecisionMicro => "average_precision_score_micro";

        internal static string AvgPrecisionMacro => "average_precision_score_macro";

        internal static string AvgPrecisionWeighted => "average_precision_score_weighted";

        internal static string AccuracyTable => "accuracy_table";

        internal static string ConfusionMatrix => "confusion_matrix";

        // Regression
        internal static string ExplainedVariance => "explained_variance";

        internal static string R2Score => "r2_score";

        internal static string Spearman => "spearman_correlation";

        internal static string MAPE => "mean_absolute_percentage_error";

        internal static string MeanAbsError => "mean_absolute_error";

        internal static string MedianAbsError => "median_absolute_error";

        internal static string RMSE => "root_mean_squared_error";

        internal static string RMSLE => "root_mean_squared_log_error";

        internal static string NormMeanAbsError => "normalized_mean_absolute_error";

        internal static string NormMedianAbsError => "normalized_median_absolute_error";

        internal static string NormRMSE => "normalized_root_mean_squared_error";

        internal static string NormRMSLE => "normalized_root_mean_squared_log_error";

        internal static string Residuals => "residuals";

        internal static string PredictedTrue => "predicted_true";

        public static bool IsValidMetric(string targetMetric)
        {
            return ClassificationMetrics.Contains(targetMetric) || RegressionMetrics.Contains(targetMetric);
        }
    }
}
