// <copyright file="AutoMLService.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

using System;
using System.IO;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.ML.ModelBuilder.AutoMLService.Contract;
using Microsoft.ML.ModelBuilder.AutoMLService.Telemetry;
using Microsoft.VisualStudio.Threading;
using StreamJsonRpc;

namespace Microsoft.ML.ModelBuilder.AutoMLService
{
    public class AutoMLService
    {
        public static IServiceCollection ServiceCollection = new ServiceCollection();

        private Stream stream;
        private JsonRpc rpc;

        public AutoMLService(Stream stream, IServiceProvider serviceProvider)
        {
            this.stream = stream;
            AutoMLServiceLogger.Instance.DiagnosticDataReceived += this.Engine_DiagnosticDataReceived;

            var engine = new AutoMLEngine(AutoMLServiceLogger.Instance);

            engine.RunStarted += this.Engine_RunStartedReceived;
            engine.AutoMLTelemetryReceived += this.Engine_AutoMLTelemetryReceived;
            engine.AlgorithmIterationCompleted += this.AutoMLService_AlgorithmIterationCompleted;

            this.rpc = JsonRpc.Attach(this.stream, engine);

            // Create ModelBuilder Service
            var modelBuilderService = new ModelBuilderService(this.rpc);
            AutoMLService.ServiceCollection.AddSingleton<IModelBuilderService>(modelBuilderService);

            // call engine.Dispose everytime rpc disconnected
            this.rpc.Disconnected += (object sender, JsonRpcDisconnectedEventArgs e) =>
            {
                var modelBuilderServiceDescriptor = AutoMLService.ServiceCollection.FirstOrDefault(descriptor => descriptor.ServiceType == typeof(IModelBuilderService));
                AutoMLService.ServiceCollection.Remove(modelBuilderServiceDescriptor);
                engine.Dispose();
            };
        }

        private void Engine_RunStartedReceived(object sender, Contract.RemoteRunStartedEventArgs e)
        {
            this.rpc.NotifyAsync("RunStarted", e).Forget();
        }

        private void Engine_AutoMLTelemetryReceived(object sender, Contract.AutoMLTelemetryEvent e)
        {
            this.rpc.NotifyAsync("AutoMLTelemetryReceived", e).Forget();
        }

        private void AutoMLTelemetry_AutoMLTelemetryEventHandler(object sender, Contract.AutoMLTelemetryEvent e)
        {
            this.rpc.NotifyAsync("AutoMLTelemetryReceived", e).Forget();
        }

        private void Engine_DiagnosticDataReceived(object sender, DataReceivedEventArgs e)
        {
            this.rpc.NotifyAsync("DiagnosticDataReceived", e).Forget();
        }

        private void AutoMLService_AlgorithmIterationCompleted(object sender, AlgorithmIterationEventArgs e)
        {
            this.rpc.NotifyAsync("AlgorithmIterationCompleted", e).Forget();
        }
    }
}
