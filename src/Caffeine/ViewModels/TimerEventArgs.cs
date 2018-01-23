// <copyright file="TimerEventArgs.cs" company="Heath Stewart">
// Copyright (c) 2017 Heath Stewart
// See the LICENSE.txt file in the project root for more information.
// </copyright>

namespace Caffeine.ViewModels
{
    using System;

    /// <summary>
    /// Event arguments provided when the <see cref="IdleService.Elapsed"/> event is raised.
    /// </summary>
    public class TimerEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TimerEventArgs"/> class.
        /// </summary>
        /// <param name="timeSpan">The amount of time elapsed.</param>
        public TimerEventArgs(TimeSpan timeSpan)
        {
            TimeSpan = timeSpan;
        }

        /// <summary>
        /// Gets the amount of time that elapsed.
        /// </summary>
        public TimeSpan TimeSpan { get; }
    }
}
