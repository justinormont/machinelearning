// <copyright file="AzureImageClassificationExperiment.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AzureML;
using Microsoft.ML.AutoML;
using Microsoft.ML.ModelBuilder.AutoMLService.Contract;

namespace Microsoft.ML.ModelBuilder.AutoMLService.Experiments
{
    internal class AzureImageClassificationExperiment : IExperiment
    {
        private readonly IAutoMLServiceLogger logger;
        private MLContext context;
        private IRemoteAutoMLTrainParameters remoteAutoMLTrainParameters;

        public AzureImageClassificationExperiment(MLContext context, IRemoteAutoMLTrainParameters remoteAutoMLTrainParameters, IAutoMLServiceLogger logger = null)
        {
            this.logger = logger;
            this.context = context;
            this.remoteAutoMLTrainParameters = remoteAutoMLTrainParameters;
        }

        public event EventHandler<RemoteRunStartedEventArgs> RunStarted;

        public event EventHandler<AlgorithmIterationEventArgs> AlgorithmIterationCompleted;

        public event EventHandler<DataReceivedEventArgs> DiagnosticDataReceived;

        public async Task<IExperimentResult> ExecuteAsync(IDataView trainData, IDataView validateData, ColumnInformation columnInformation, CancellationToken? ct, CancellationToken? timeout)
        {
            AutoMLRunnerImages runner = null;
            List<AlgorithmIterationEventArgs> iterationInformation = new List<AlgorithmIterationEventArgs>();

            IExperimentResult trainResult = new ExperimentResult();

            void IterationHandler(object sender, AlgorithmIterationEventArgs e)
            {
                this.AlgorithmIterationCompleted?.Invoke(sender, e);
                iterationInformation.Add(e);
            }

            try
            {
                runner = new AutoMLRunnerImages(this.remoteAutoMLTrainParameters, this.logger);
                runner.RunStarted += this.Runner_RunStarted;
                runner.AlgorithmIterationCompleted += IterationHandler;
                string projectFolder = this.remoteAutoMLTrainParameters.TempOutputDirectory;
                var result = await Task.Run(() => runner.RunAutoMLAsync(this.context, ct.GetValueOrDefault()));
                trainResult.BestModel = result.BestModel;
                trainResult.BestPipeline = result.BestPipeline;

                trainResult.TrainResult = new TrainResult()
                {
                    Task = this.remoteAutoMLTrainParameters.TaskType,
                    Dataset = this.remoteAutoMLTrainParameters.InputFile,
                    Predict = this.remoteAutoMLTrainParameters.LabelColumn,
                    ExperimentTime = this.remoteAutoMLTrainParameters.TrainTime.ToString(),
                    ModelsExplored = iterationInformation.Count.ToString(),
                    Algorithms = iterationInformation,
                };
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (runner != null)
                {
                    runner.AlgorithmIterationCompleted -= IterationHandler;
                }
            }

            return trainResult;
        }

        private void Runner_RunStarted(object sender, RemoteRunStartedEventArgs e)
        {
            this.RunStarted?.Invoke(sender, e);
        }
    }
}
