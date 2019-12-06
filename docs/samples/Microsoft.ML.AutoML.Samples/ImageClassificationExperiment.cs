using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.ML.Data;

namespace Microsoft.ML.AutoML.Samples
{
    public static class ImageClassificationExperiment
    {
        private static string TrainDataPath = @"C:\Users\xiaoyuz\Desktop\WeatherData\WeatherData.tsv";
        private static string ModelPath = @"C:\Users\xiaoyuz\Desktop\WeatherData\WeatherData.zip";
        private static uint ExperimentTime = 3600;

        public static void Run()
        {
            MLContext mlContext = new MLContext();

            // STEP 1: Load data
            IDataView trainDataView = mlContext.Data.LoadFromTextFile<Image>(TrainDataPath, hasHeader: true, separatorChar: '\t');

            var setting = new MulticlassExperimentSettings()
            {
                MaxExperimentTimeInSeconds = ExperimentTime,
                CacheBeforeTrainer = CacheBeforeTrainer.On,
                CacheDirectory = new System.IO.DirectoryInfo(@"C:\Users\xiaoyuz\Desktop\WeatherData"),
            };
            // STEP 2: Run AutoML experiment
            Console.WriteLine($"Running AutoML binary classification experiment for {ExperimentTime} seconds...");
            ExperimentResult<MulticlassClassificationMetrics> experimentResult = mlContext.Auto()
                .CreateMulticlassClassificationExperiment(setting)
                .Execute(trainDataView);

            // STEP 3: Print metric from the best model
            RunDetail<MulticlassClassificationMetrics> bestRun = experimentResult.BestRun;
            foreach (var result in experimentResult.RunDetails)
            {
                Console.WriteLine($"Duration: {result.RuntimeInSeconds}");
                PrintMetrics(result.ValidationMetrics);
            }
            Console.WriteLine($"Total models produced: {experimentResult.RunDetails.Count()}");
            Console.WriteLine($"Best model's trainer: {bestRun.TrainerName}");
            Console.WriteLine($"Metrics of best model from validation data --");

            // STEP 5: Save the best model for later deployment and inferencing
            using (FileStream fs = File.Create(ModelPath))
                mlContext.Model.Save(bestRun.Model, trainDataView.Schema, fs);
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }

        private static void PrintMetrics(MulticlassClassificationMetrics metrics)
        {
            Console.WriteLine($"LogLoss: {metrics.LogLoss}");
            Console.WriteLine($"LogLossReduction: {metrics.LogLossReduction}");
            Console.WriteLine($"MacroAccuracy: {metrics.MacroAccuracy}");
            Console.WriteLine($"MicroAccuracy: {metrics.MicroAccuracy}");
        }


        private class Image
        {
            [LoadColumn(0)]
            public string Label { get; set; }

            [LoadColumn(1)]
            public string ImageSource { get; set; }
        }
    }
}
