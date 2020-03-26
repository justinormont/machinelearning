// <copyright file="AutoMLRunMonitoringImages.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Azure.MachineLearning.Services.Experiments;
using Azure.MachineLearning.Services.Runs;
using Azure.MachineLearning.Services.Workspaces;
using Microsoft.Azure.Management.ResourceManager.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent.Authentication;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.ML.ModelBuilder.AutoMLService;
using Microsoft.ML.ModelBuilder.AutoMLService.Contract;
using Microsoft.Rest;

namespace AzureML
{
    internal static class AutoMLRunMonitoringImages
    {
        private static TimeSpan refreshInterval = TimeSpan.FromSeconds(1);

        public static async Task<(Run bestRun, double bestScore)> ReportStatusAsync(AutoMLRun autoMLRun, Workspace workspace, Experiment experiment, Action<AlgorithmIterationEventArgs> reporter, IAutoMLServiceLogger logger, CancellationToken cancellationToken = default)
        {
            // var setupIterationStatus = MonitorSetupIterationAsync(autoMLRun, experiment, logger);
            return await MonitorParentRunAsync(autoMLRun, reporter, logger, cancellationToken);
        }

        public static async Task<(Run bestRun, double bestScore)> MonitorParentRunAsync(AutoMLRun autoMLRun, Action<AlgorithmIterationEventArgs> reporter, IAutoMLServiceLogger logger, CancellationToken cancellationToken = default)
        {
            var failures = new FailureCounter(30);
            var childRunRetrieval = new FailureCounter(300);

            do
            {
                try
                {
                    Thread.Sleep(refreshInterval);

                    // Check cancellation
                    cancellationToken.ThrowIfCancellationRequested();

                    await autoMLRun.RefreshAsync();
                    var autoMlChildRuns = autoMLRun.ListChildren();

                    // TODO: this isn't gonna universally work, only with images as they are now

                    // images runs only have one child which is hyperdrive run
                    var hdRun = autoMlChildRuns.FirstOrDefault();

                    if (hdRun == null)
                    {
                        childRunRetrieval.RecordFailure("Didn't find a child run.");

                        continue;
                    }

                    var hdChildRuns = hdRun.ListChildren();

                    var runsByStatus = hdChildRuns.GroupBy(cr => cr.Status, cr => cr);

                    var childRunStatus = string.Join(", ", runsByStatus.Select(r => $"{r.Key}: {r.Count()}"));

                    var completedChildRuns = hdChildRuns.Where(cr => cr.InTerminalState && cr.Status == "Completed" && cr.Type != "preparation");

                    (Run bestRun, double bestScore) bestRun = (null, 0);
                    string bestRunStatus = string.Empty;

                    if (completedChildRuns.Any())
                    {
                        var runStats = new RunStats();
                        bestRun = await runStats.GetBestRunAsync(completedChildRuns, runStats.GetPrimaryMetricFromProperties(autoMLRun), cancellationToken: cancellationToken);

                        if (bestRun.bestRun == null)
                        {
                            continue;
                        }

                        var algo = bestRun.bestRun.Properties.ContainsKey("run_algorithm") ? bestRun.bestRun.Properties["run_algorithm"] : "unknown";
                        var preproc = bestRun.bestRun.Properties.ContainsKey("run_preprocessor") ? bestRun.bestRun.Properties["run_preprocessor"] : "unknown";
                        var metricName = runStats.GetPrimaryMetricFromProperties(autoMLRun);
                        bestRunStatus = $"Best {metricName} metric value is {bestRun.bestScore} using algorithm {algo} and preprocessor {preproc}";

                        var metricDictionary = new Dictionary<string, double>();
                        metricDictionary[metricName] = Math.Round(bestRun.bestScore, 2);

                        reporter(new AlgorithmIterationEventArgs()
                        {
                            IsBest = true,
                            IterationIndex = 0,
                            Metrics = metricDictionary,
                            RuntimeInSeconds = 0,
                            Score = Math.Round(bestRun.bestScore, 2) / 100,
                            TrainerName = $"Azure AutoML Algorithm",
                        });

                        return bestRun;
                    }

                    if (hdRun.InTerminalState)
                    {
                        var finalMsg = $"AutoML sweep final status is {hdRun.Status}. Run time is {(hdRun.EndTimeUtc - hdRun.CreatedUtc).Value.TotalSeconds} seconds.";

                        Console.WriteLine(new[] { finalMsg, "Child run status: " + childRunStatus, bestRunStatus });

                        if (hdRun.Status == "Failed")
                        {
                            throw new Exception("Training has failed.Please go to Azure portal to see more details.");
                        }
                        else
                        {
                            return bestRun;
                        }
                    }

                    if (autoMLRun.InTerminalState)
                    {
                        logger?.Info("Run is " + autoMLRun.Status);
                        if (autoMLRun.Status == "Failed")
                        {
                          throw new Exception("Training has failed. Please go to Azure portal to see more details.");
                        }
                        else
                        {
                            return bestRun;
                        }
                    }

                    logger?.Info("Running AutoML pipeline sweep..");
                    logger?.Info($"Current child run stats: {childRunStatus}");
                    logger?.Info(bestRunStatus);
                }
                catch (OperationCanceledException)
                {
                    throw;
                }
                catch (Azure.MachineLearning.Services.GeneratedOld.Models.ErrorResponseException exp)
                {
                    if (exp.Response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    {
                        // Refresh token
                        var token = await AutoMLRunMonitoringImages.ModelBuilderService.RefreshTokenAsync(cancellationToken);
                        var tokenCredentials = new TokenCredentials(token);
                        var serviceClientCredentials = new AzureCredentials(
                                                        tokenCredentials,
                                                        tokenCredentials,
                                                        null,  // TODO: provide a way to specify TenantId?
                                                        AzureEnvironment.AzureGlobalCloud);
                        autoMLRun.ServiceContext.Credentials = serviceClientCredentials;
                    }
                    else
                    {
                        // Retry only when it's network error
                        await failures.RecordFailureWithRetryWarningAsync(exp, AutoMLRunMonitoringImages.ModelBuilderService, cancellationToken);
                    }
                }
                catch (Exception ex)
                {
                    if (ex.Message == "Training has failed.Please go to Azure portal to see more details.")
                    {
                        throw ex;
                    }

                    await failures.RecordFailureWithRetryWarningAsync(ex, AutoMLRunMonitoringImages.ModelBuilderService, cancellationToken);
                }
            }
            while (true);
        }

        public static async Task<string> MonitorSetupIterationAsync(AutoMLRun autoMLRun, Experiment experiment, Action<string> logger)
        {
            Run setupIteration;

            do
            {
                Thread.Sleep(refreshInterval);
                var runs = experiment.Runs;
                var runList = runs.List();

                var childRuns = autoMLRun.ListChildren();

                // TODO: figure out a better way to find setup iteration for this run
                setupIteration = runList.Where(cr => cr.Id.StartsWith(autoMLRun.Id) && cr.Properties.ContainsKey("iteration") && cr.Properties["iteration"] == "setup").FirstOrDefault();

                // TODO: check for overall experiment terminal status and handle that
                if (setupIteration != null)
                {
                    break;

                    // Console.WriteLine($"Setup iteration {setupIteration.Name} is {setupIteration.Status}");
                }

                logger("Waiting for setup iteration to get created..");
            }
            while (true);

            // var fpm = new ConsoleFixedPositionMessage(1, enableSpinner: true);
            do
            {
                await setupIteration.RefreshAsync();
                logger($"Setup iteration in progress, status is {setupIteration.Status}..");

                if (setupIteration.InTerminalState)
                {
                    logger($"Setup iteration final status is {setupIteration.Status}. Run time {(setupIteration.EndTimeUtc - setupIteration.CreatedUtc).Value.TotalSeconds} seconds.");
                    return setupIteration.Status;
                }

                Thread.Sleep(refreshInterval);
            }
            while (true);
        }

        public static string GetRunUrl(AutoMLRun run, string subscriptionId, string resourceGroupName, string workspaceName)
        {
            return $"https://mlworkspacecanary.azure.ai/portal/subscriptions/{subscriptionId}/resourceGroups/{resourceGroupName}/providers/Microsoft.MachineLearningServices/workspaces/{workspaceName}/experiments/{run.ExperimentName}/runs/{run.Id}";
        }

        private static IModelBuilderService ModelBuilderService
        {
            get
            {
                var service = AutoMLService.ServiceCollection.BuildServiceProvider().GetRequiredService<IModelBuilderService>();
                if (service == null)
                {
                    throw new Exception("modelbuilder service not found");
                }

                return service;
            }
        }
    }
}
