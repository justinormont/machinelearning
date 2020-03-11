// <copyright file="FixedDurationBar.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

using System;
using System.Threading;

namespace Microsoft.ML.CLI.ShellProgressBar
{
    public class FixedDurationBar : ProgressBar
    {
        private long seenTicks;
        private readonly ManualResetEvent completedHandle;

        public FixedDurationBar(TimeSpan duration, string message, ProgressBarOptions options = null) : base((int)Math.Ceiling(duration.TotalSeconds), message, options)
        {
            this.completedHandle = new ManualResetEvent(false);
            if (!this.Options.DisplayTimeInRealTime)
            {
                throw new ArgumentException(
                    $"{nameof(ProgressBarOptions)}.{nameof(ProgressBarOptions.DisplayTimeInRealTime)} has to be true for {nameof(FixedDurationBar)}", nameof(options)
                );
            }
        }

        public WaitHandle CompletedHandle => this.completedHandle;

        public bool IsCompleted { get; private set; }

        protected override void OnTimerTick()
        {
            Interlocked.Increment(ref this.seenTicks);
            if (this.seenTicks % 2 == 0)
            {
                this.Tick();
            }

            base.OnTimerTick();
        }

        protected override void OnDone()
        {
            this.IsCompleted = true;
            this.completedHandle.Set();
        }
    }
}
