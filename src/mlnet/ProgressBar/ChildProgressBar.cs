// <copyright file="ChildProgressBar.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

using System;

namespace Microsoft.ML.CLI.ShellProgressBar
{
    public class ChildProgressBar : ProgressBarBase, IProgressBar
    {
        private readonly Action scheduleDraw;
        private readonly Action<ProgressBarHeight> growth;

        protected override void DisplayProgress() => scheduleDraw?.Invoke();

        internal ChildProgressBar(int maxTicks, string message, Action scheduleDraw, ProgressBarOptions options = null, Action<ProgressBarHeight> growth = null)
            : base(maxTicks, message, options)
        {
            _callOnce = new object();
            this.scheduleDraw = scheduleDraw;
            this.growth = growth;
            this.growth?.Invoke(ProgressBarHeight.Increment);
        }

        private bool _calledDone;
        private readonly object _callOnce;

        protected override void OnDone()
        {
            if (_calledDone) return;
            lock (_callOnce)
            {
                if (_calledDone) return;

                if (EndTime == null)
                    EndTime = DateTime.Now;

                if (Collapse)
                    growth?.Invoke(ProgressBarHeight.Decrement);

                _calledDone = true;
            }
        }

        public void Dispose()
        {
            OnDone();
            foreach (var c in Children) c.Dispose();
        }
    }
}
