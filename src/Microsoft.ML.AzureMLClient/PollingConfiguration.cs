// <copyright file="PollingConfiguration.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

using System;
using System.Threading;
using System.Threading.Tasks;

namespace Azure.MachineLearning.Services
{
    public class PollingConfiguration
    {
        private TimeSpan _timeout;
        private TimeSpan _initial;
        private TimeSpan _max;
        private double _intervalGrowth;

        public PollingConfiguration()
        {
            this._timeout = TimeSpan.MaxValue;
            this._initial = TimeSpan.FromSeconds(1);
            this._max = TimeSpan.FromSeconds(90);
            this._intervalGrowth = 1.5;
        }

        public TimeSpan Timeout
        {
            get
            {
                return this._timeout;
            }

            set
            {
                if (value < this.InitialInterval)
                {
                    var message = string.Format(
                        "Must set Timeout to be longer than the InitialInterval ({0} seconds)",
                        this.InitialInterval.TotalSeconds);
                    throw new ArgumentOutOfRangeException(message);
                }
                this._timeout = value;
            }
        }

        public TimeSpan InitialInterval
        {
            get
            {
                return this._initial;
            }

            set
            {
                if (value <= TimeSpan.FromSeconds(0))
                {
                    throw new ArgumentOutOfRangeException("Must have positive InitialInterval");
                }
                if (value >= this.MaximumInterval)
                {
                    var message = string.Format(
                        "Must set InitialInterval less than MaximumInterval ({0} seconds)",
                        this.MaximumInterval.TotalSeconds);
                    throw new ArgumentOutOfRangeException(message);
                }
                this._initial = value;
            }
        }

        public TimeSpan MaximumInterval
        {
            get
            {
                return this._max;
            }

            set
            {
                if (value <= this._initial)
                {
                    var message = string.Format(
                        "Must set MaximumInterval to be larger than InitialInterval ({0} seconds)",
                        this.InitialInterval.TotalSeconds);
                    throw new ArgumentOutOfRangeException(message);
                }
                this._max = value;
            }
        }

        public double IntervalGrowthMultiplier
        {
            get
            {
                return this._intervalGrowth;
            }

            set
            {
                if (value < 1.0)
                {
                    throw new ArgumentOutOfRangeException("Must specify growth multiplier >= 1.0");
                }
                this._intervalGrowth = value;
            }
        }

        public async Task<bool> WaitForCompletion(
            Func<Task> updateStatus,
            Func<bool> stoppingCondition,
            bool showOutput = false,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            Throw.IfNull(updateStatus, nameof(updateStatus));
            Throw.IfNull(stoppingCondition, nameof(stoppingCondition));

            var waitStart = DateTime.Now;
            bool timedOut = false;
            var currentInterval = InitialInterval;

            while (!stoppingCondition() && !timedOut)
            {
                if (showOutput)
                {
                    Console.WriteLine("Waiting for {0:F1} seconds", currentInterval.TotalSeconds);
                }

                await updateStatus().ConfigureAwait(false);

                await Task.Delay(currentInterval, cancellationToken).ConfigureAwait(false);
                currentInterval = TimeSpan.FromSeconds(currentInterval.TotalSeconds * IntervalGrowthMultiplier);
                if (currentInterval > MaximumInterval)
                {
                    currentInterval = MaximumInterval;
                }

                if ((DateTime.Now - waitStart) > Timeout)
                {
                    timedOut = true;
                    if (showOutput)
                    {
                        Console.Error.WriteLine("WaitForCompletion timed out");
                    }
                }
            }

            return stoppingCondition();
        }
    }
}
