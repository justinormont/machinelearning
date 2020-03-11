// <copyright file="ProgressBarBase.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

using System;
using System.Collections.Concurrent;
using System.Text;
using System.Threading;

namespace Microsoft.ML.CLI.ShellProgressBar
{
    public abstract class ProgressBarBase
    {
        static ProgressBarBase()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        }

        public readonly DateTime startDate;
        private int maxTicks;
        private int currentTick;
        private string message;

        protected ProgressBarBase(int maxTicks, string message, ProgressBarOptions options)
        {
            this.maxTicks = Math.Max(0, maxTicks);
            this.message = message;
            this.Options = options ?? ProgressBarOptions.Default;
            this.startDate = DateTime.Now;
            this.Children = new ConcurrentBag<ChildProgressBar>();
        }

        public DateTime? EndTime { get; protected set; }

        public ConsoleColor ForeGroundColor =>
    this.EndTime.HasValue ? this.Options.ForegroundColorDone ?? this.Options.ForegroundColor : this.Options.ForegroundColor;

        public int CurrentTick => this.currentTick;

        public int MaxTicks
        {
            get => this.maxTicks;
            set
            {
                Interlocked.Exchange(ref this.maxTicks, value);
                this.DisplayProgress();
            }
        }

        public string Message
        {
            get => this.message;
            set
            {
                Interlocked.Exchange(ref this.message, value);
                this.DisplayProgress();
            }
        }

        public double Percentage
        {
            get
            {
                var percentage = Math.Max(0, Math.Min(100, (100.0 / this.maxTicks) * this.currentTick));

                // Gracefully handle if the percentage is NaN due to division by 0
                if (double.IsNaN(percentage) || percentage < 0)
                {
                    percentage = 100;
                }

                return percentage;
            }
        }

        public bool Collapse => this.EndTime.HasValue && this.Options.CollapseWhenFinished;

        internal ProgressBarOptions Options { get; }

        internal ConcurrentBag<ChildProgressBar> Children { get; }

        public ChildProgressBar Spawn(int maxTicks, string message, ProgressBarOptions options = null)
        {
            var pbar = new ChildProgressBar(maxTicks, message, this.DisplayProgress, options, this.Grow);
            this.Children.Add(pbar);
            this.DisplayProgress();
            return pbar;
        }

        public void Tick(string message = null)
        {
            Interlocked.Increment(ref this.currentTick);

            this.FinishTick(message);
        }

        public void Tick(int newTickCount, string message = null)
        {
            Interlocked.Exchange(ref this.currentTick, newTickCount);

            this.FinishTick(message);
        }

        protected abstract void DisplayProgress();

        protected virtual void Grow(ProgressBarHeight direction)
        {
        }

        protected virtual void OnDone()
        {
        }

        private void FinishTick(string message)
        {
            if (message != null)
            {
                Interlocked.Exchange(ref this.message, message);
            }

            if (this.currentTick >= this.maxTicks)
            {
                this.EndTime = DateTime.Now;
                this.OnDone();
            }

            this.DisplayProgress();
        }
    }
}
