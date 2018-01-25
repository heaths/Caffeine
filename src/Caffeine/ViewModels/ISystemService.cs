// <copyright file="ISystemService.cs" company="Heath Stewart">
// Copyright (c) 2017 Heath Stewart
// See the LICENSE.txt file in the project root for more information.
// </copyright>

namespace Caffeine.ViewModels
{
    using System;
    using System.ComponentModel;

    /// <summary>
    /// Represents system services.
    /// </summary>
    public interface ISystemService
    {
        /// <summary>
        /// Gets the operating system version.
        /// </summary>
        Version OSVersion { get; }

        /// <summary>
        /// Gets the number of ticks since the system was started.
        /// </summary>
        long Ticks { get; }

        /// <summary>
        /// Blocks shutdown for the given <paramref name="reason"/>.
        /// </summary>
        /// <param name="handle">A handle to a window.</param>
        /// <param name="reason">The reason to block shutdowns.</param>
        /// <returns>An <see cref="IDisposable"/> object that cancels the request when disposed.</returns>
        /// <exception cref="Win32Exception">A Windows error occured.</exception>
        IDisposable BlockShutdown(IntPtr handle, string reason);

        /// <summary>
        /// Creates an <see cref="IPowerRequest"/>.
        /// </summary>
        /// <param name="reason">The reason the request was created.</param>
        /// <returns>An <see cref="IPowerRequest"/>.</returns>
        /// <exception cref="ArgumentException"><paramref name="reason"/> is empty.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="reason"/> is null.</exception>
        /// <exception cref="Win32Exception">A Windows error occured.</exception>
        IPowerRequest CreatePowerRequest(string reason);

        /// <summary>
        /// Gets the number of ticks since the last keyboard or mouse input.
        /// </summary>
        /// <returns>The number of ticks from the last keyboard or mouse input.</returns>
        long GetLastInput();

        /// <summary>
        /// Sets shutdown parameters for the calling process.
        /// </summary>
        /// <param name="priority">The priority that the process should be shutdown, between 256 and 1023 inclusive. Higher values go first.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="priority"/> is less than 256 or greater than 1023.</exception>
        /// <exception cref="Win32Exception">A Windows error occured.</exception>
        void SetShutdownParameters(int priority);

        /// <summary>
        /// Initiates a system shutdown.
        /// </summary>
        /// <param name="restart">A value indicating whether to restart the machine after shutdown.</param>
        /// <exception cref="Win32Exception">A Windows error occured.</exception>
        void Shutdown(bool restart = false);
    }
}
