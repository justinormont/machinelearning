// <copyright file="ProgressBarOptions.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

using System;
using System.Runtime.InteropServices;

namespace Microsoft.ML.CLI.ShellProgressBar
{
    /// <summary>
    /// Control the behaviour of your progressbar
    /// </summary>
    public class ProgressBarOptions
    {
        public static readonly ProgressBarOptions Default = new ProgressBarOptions();

        private bool enableTaskBarProgress;

        public ProgressBarOptions()
        {
            this.ForegroundColor = ConsoleColor.Green;
            this.ProgressCharacter = '\u2588';
            this.DisplayTimeInRealTime = true;
            this.CollapseWhenFinished = true;
        }

        /// <summary> Gets or sets the foreground color of the progress bar, message and time</summary>
        public ConsoleColor ForegroundColor { get; set; }

        /// <summary> Gets or sets the foreground color the progressbar has reached a 100 percent</summary>
        public ConsoleColor? ForegroundColorDone { get; set; }

        /// <summary> Gets or sets the background color of the remainder of the progressbar</summary>
        public ConsoleColor? BackgroundColor { get; set; }

        /// <summary> Gets or sets the character to use to draw the progressbar</summary>
        public char ProgressCharacter { get; set; }

        /// <summary>
        /// Gets or sets the character to use for the background of the progress defaults to <see cref="ProgressCharacter"/>
        /// </summary>
        public char? BackgroundCharacter { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether when true will redraw the progressbar using a timer, otherwise only update when
        /// <see cref="ProgressBarBase.Tick(string)"/> is called.
        /// Defaults to true
        ///  </summary>
        public bool DisplayTimeInRealTime { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether collapse the progressbar when done, very useful for child progressbars
        /// Defaults to true
        /// </summary>
        public bool CollapseWhenFinished { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether by default the text and time information is displayed at the bottom and the progress bar at the top.
        /// This setting swaps their position
        /// </summary>
        public bool ProgressBarOnBottom { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether use Windows' task bar to display progress.
        /// </summary>
        /// <remarks>
        /// This feature is available on the Windows platform.
        /// </remarks>
        public bool EnableTaskBarProgress
        {
            get => this.enableTaskBarProgress;
            set
            {
                if (value && !RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    throw new NotSupportedException("Task bar progress only works on Windows");
                }

                this.enableTaskBarProgress = value;
            }
        }
    }
}
