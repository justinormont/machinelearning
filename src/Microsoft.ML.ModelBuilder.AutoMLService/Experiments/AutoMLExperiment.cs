// <copyright file="AutoMLExperiment.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ML.AutoML;
using Microsoft.ML.ModelBuilder.AutoMLService.Contract;
using Microsoft.ML.ModelBuilder.Utils;

namespace Microsoft.ML.ModelBuilder.AutoMLService.Experiments
{
    internal abstract class AutoMLExperiment<TMetrics, TMetric, TExperimentSettings> : IExperiment
        where TMetric : IConvertible
        where TMetrics : class
        where TExperimentSettings : ExperimentSettings
    {
        private readonly IAutoMLServiceLogger logger;
        private IList<RunDetail<TMetrics>> result;
        private double bestScore = 0;

        protected AutoMLExperiment(MLContext context, ILocalAutoMLTrainParameters settings, IAutoMLServiceLogger logger)
        {
            this.Context = context;
            this.Settings = settings;
            this.logger = logger;
        }

        public event EventHandler<RemoteRunStartedEventArgs> RunStarted;

        public event EventHandler<AlgorithmIterationEventArgs> AlgorithmIterationCompleted;

        protected MLContext Context { get; set; }

        protected ILocalAutoMLTrainParameters Settings { get; set; }

        protected abstract ModelBuilderProgressHandler<TMetrics, TMetric, TExperimentSettings> Handler { get; set; }

        public async Task<IExperimentResult> ExecuteAsync(IDataView trainData, IDataView validateData, ColumnInformation columnInformation, CancellationToken? userCancellationToken, CancellationToken? timeout)
        {

            var taskCompleteSource = new TaskCompletionSource<ExperimentResult<TMetrics>>();

            userCancellationToken?.Register(() =>
            {
                userCancellationToken?.ThrowIfCancellationRequested();
                taskCompleteSource.TrySetException(new OperationCanceledException(userCancellationToken.GetValueOrDefault()));
            });

            timeout?.Register(() =>
            {
                taskCompleteSource.TrySetException(new OperationCanceledException(timeout.GetValueOrDefault()));
            });

            var experiment = this.CreateExperiment(userCancellationToken.GetValueOrDefault());
            EventHandler<AutoMLIterationInfo<TMetrics>> autoMLEngine_AlgorithmIterationCompleted = (object sender, AutoMLIterationInfo<TMetrics> e) =>
            {
                // check if contains exception
                // if it's tensorflow dll not found exception, throw it
                if (e.RunDetails.Exception?.Message == "Tensorflow exception triggered while loading model.")
                {
                    taskCompleteSource.TrySetException(new Exception(e.RunDetails.Exception.Message));
                    return;
                }

                this.Handler_AlgorithmIterationHandler(sender, e);
            };

            this.Handler.AlgorithmIterationHandler += autoMLEngine_AlgorithmIterationCompleted;

            this.Handler.OutputHandler += this.Handler_OutputHandler;

            // print matrix head
            this.logger?.Info(this.Handler.MetricsHeader());

            try
            {
                var experimentTask = Task.Run(() => experiment.Execute(trainData, validateData, columnInformation, progressHandler: this.Handler));
                var task = await Task.WhenAny(experimentTask, taskCompleteSource.Task);
                var result = await task;
                this.result = result.RunDetails.Select(x => x).ToList();
            }
            catch (OperationCanceledException exp)
            {
                if (exp.CancellationToken == timeout)
                {
                    if (this.result == null)
                    {
                        // If there's no best model yet, but the train time has finished, throw a timeout exception.
                        throw new TimeoutException("Training time finished without any models trained.", exp);
                    }
                }
                else
                {
                    // User cancelled the training without a model trained successfully. Rethrow the operation canceled exception.
                    // TODO: If we want to enable users to stop training and just use any models that have been generated, then we should return
                    // the ExperimentResult here instead of throwing.
                    // See https://github.com/dotnet/machinelearning-modelbuilder/issues/233
                    throw;
                }
            }
            catch (Exception exp)
            {
                throw exp;
            }
            finally
            {
                this.Handler.AlgorithmIterationHandler -= this.Handler_AlgorithmIterationHandler;
                this.Handler.OutputHandler -= this.Handler_OutputHandler;
                this.Handler.AlgorithmIterationHandler -= autoMLEngine_AlgorithmIterationCompleted;
            }

            // handle result
            var algorithmsList = BestResultUtil.GetTopNRunResults(this.result, this.Handler.MetricsAgent, 5, this.Handler.IsMaximizing)
                                .Select((val) =>
                                {
                                    var args = this.Handler.GenerateAlgorithmIterationEventArg(val.Item1);
                                    args.IterationIndex = val.Item2;
                                    return args;
                                }).ToList<AlgorithmIterationEventArgs>();

            var experimentTime = this.result.Select((val) => val.RuntimeInSeconds).Sum();

            // Print Experiment and Iteration Summary
            var experimentResultsHeader = ConsolePrinter.ExperimentResultsHeader(this.Settings.Scenario, this.Settings.InputFile, this.Settings.LabelColumn, experimentTime.ToString(), this.result.Count());
            this.logger?.Info(experimentResultsHeader);

            var iterationSummary = this.Handler.IterationSummary(this.result, 5);
            this.logger?.Info(iterationSummary);

            var trainResult = new TrainResult()
            {
                Task = this.Settings.Scenario,
                Dataset = this.Settings.InputFile,
                Predict = this.Settings.LabelColumn,
                ExperimentTime = experimentTime.ToString(),
                ModelsExplored = this.result.Count().ToString(),
                Algorithms = algorithmsList,
            };

            var bestModel = BestResultUtil.GetBestRun(this.result, this.Handler.MetricsAgent, this.Handler.IsMaximizing);

            return new ExperimentResult()
            {
                BestModel = bestModel.Model,
                BestPipeline = bestModel.Pipeline,
                TrainResult = trainResult,
            };
        }

        /// <summary>
        /// Create an AutoML Experiment and initialize experiment handler.
        /// </summary>
        /// <param name="cancellationToken">user cancellationToken.</param>
        /// <returns>an AutoML Experiment.</returns>
        protected abstract ExperimentBase<TMetrics, TExperimentSettings> CreateExperiment(CancellationToken cancellationToken);

        private void Handler_OutputHandler(object sender, DataReceivedEventArgs e)
        {
            this.logger?.Info(e.Data);
        }

        private void Handler_AlgorithmIterationHandler(object sender, AutoMLIterationInfo<TMetrics> e)
        {
            if (this.result == null)
            {
                this.result = new List<RunDetail<TMetrics>>();
            }

            // check if it's the best result
            if (this.result.Count() == 0)
            {
                e.IsBest = true;
                this.bestScore = e.Score;
            }
            else
            {
                if (this.Handler.IsMaximizing ? e.Score > this.bestScore : e.Score < this.bestScore)
                {
                    e.IsBest = true;
                    this.bestScore = e.Score;
                }
            }

            this.result.Add(e.RunDetails);
            this.AlgorithmIterationCompleted?.Invoke(this, e);
        }
    }
}
