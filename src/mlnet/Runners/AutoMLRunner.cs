// <copyright file="AutoMLRunner.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

using AngleSharp.Css.Values;
using Microsoft.ML.CLI.ShellProgressBar;
using Microsoft.ML.ModelBuilder.AutoMLService;
using Microsoft.ML.ModelBuilder.AutoMLService.Contract;
using Microsoft.ProgramSynthesis.Conditionals.Build.RuleNodeTypes;
using NLog;
using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Timers;
using AutoMLEngine = Microsoft.ML.ModelBuilder.AutoMLEngine;

namespace Microsoft.ML.CLI.Runners
{
    internal class AutoMLRunner : IRunner
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        private readonly ILocalAutoMLTrainParameters settings;
        private readonly AutoMLEngine autoMLEngine;
        private StringBuilder experimentResult = new StringBuilder();

        internal AutoMLRunner(ILocalAutoMLTrainParameters settings)
        {
            this.settings = settings;
            this.autoMLEngine = new AutoMLEngine();
        }

        public void Execute()
        {
            string bestTrainer = string.Empty;
            string bestScore = string.Empty;
            // number of model that explored
            int exploreNum = 0;

            // Set up pBar
            var pbar = new MyProgressBar(settings.TrainTime);
            AutoMLServiceLogger.Instance.DiagnosticDataReceived += (object sender, DataReceivedEventArgs e) =>
            {
                pbar.Draw();
            };

            pbar.Start();

            EventHandler<AlgorithmIterationEventArgs> autoMLEngine_AlgorithmIterationCompleted = (object sender, AlgorithmIterationEventArgs e) =>
            {
                exploreNum += 1;

                if (e.IsBest)
                {
                    bestTrainer = e.TrainerName;
                    bestScore = e.Score.ToString();
                }
            };

            this.autoMLEngine.AlgorithmIterationCompleted += autoMLEngine_AlgorithmIterationCompleted;
            this.autoMLEngine.AutoMLTelemetryReceived += this.AutoMLEngine_AutoMLTelemetryReceived;
            try
            {
                this.autoMLEngine.StartTrainingAsync(new AutoMLServiceParamater(this.settings), CancellationToken.None).Wait();
            }
            finally
            {
                this.autoMLEngine.AutoMLTelemetryReceived -= this.AutoMLEngine_AutoMLTelemetryReceived;
                this.autoMLEngine.AlgorithmIterationCompleted -= autoMLEngine_AlgorithmIterationCompleted;
            }

            logger.Log(LogLevel.Info, $"{Strings.GenerateModelConsumption}: { Path.Combine(this.settings.TempOutputDirectory, $"{this.settings.Name}.ConsoleApp")}");
        }

        private void AutoMLEngine_AutoMLTelemetryReceived(object sender, ModelBuilder.AutoMLService.Contract.AutoMLTelemetryEvent e)
        {
            Telemetry.Telemetry.TrackEvent(e.EventName, e.Properties);
        }

        private class MyProgressBar
        {
            private int timeSpan = 0;
            private System.Timers.Timer timer;

            public MyProgressBar(int timeSpan)
            {
                this.timeSpan = timeSpan;
                this.timer = new System.Timers.Timer(1000);
            }

            public void Start()
            {
                this.timer.Elapsed += this.Timer_Elapsed1;
                this.timer.Start();
            }

            private void Timer_Elapsed1(object sender, ElapsedEventArgs e)
            {
                this.timeSpan -= 1;
                this.Draw();
                if(this.timeSpan <= 0)
                {
                    this.Stop();
                }
            }

            public void Stop()
            {
                this.timer.Stop();

                // Empty line
                logger.Info("\r" + new string(' ',Console.BufferWidth-1));
            }

            public void Draw()
            {
                if (this.timeSpan > 0)
                {
                    logger.Info($"I'm progress bar {this.timeSpan}s left");
                    Console.CursorTop -= 1;
                }
            }
        }
    }
}
