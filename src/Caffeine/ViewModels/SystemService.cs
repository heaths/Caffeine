// <copyright file="SystemService.cs" company="Heath Stewart">
// Copyright (c) 2017 Heath Stewart
// See the LICENSE.txt file in the project root for more information.
// </copyright>

namespace Caffeine.ViewModels
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// Provides system services.
    /// </summary>
    [ExcludeFromCodeCoverage]
    internal class SystemService : ISystemService
    {
        /// <summary>
        /// Gets the singleton instance of this <see cref="SystemService"/>.
        /// </summary>
        public static readonly ISystemService Default = new SystemService();

        private const int ShutdownPriorityMin = 0x100;
        private const int ShutdownPriorityMax = 0x3ff;

        /// <inheritdoc/>
        public long Ticks => Environment.TickCount;

        /// <inheritdoc/>
        public Version OSVersion => Environment.OSVersion.Version;

        /// <inheritdoc/>
        public IDisposable BlockShutdown(IntPtr handle, string reason) =>
            new ShutdownBlockReason(this, handle, reason);

        /// <inheritdoc/>
        public IPowerRequest CreatePowerRequest(string reason) =>
            new PowerRequest(reason);

        /// <inheritdoc/>
        public long GetLastInput()
        {
            var info = LASTINPUTINFO.Default;

            if (NativeMethods.GetLastInputInfo(ref info))
            {
                return info.dwTime;
            }

            return 0;
        }

        /// <inheritdoc/>
        public void SetShutdownParameters(int priority)
        {
            if (priority < ShutdownPriorityMin || priority > ShutdownPriorityMax)
            {
                throw new ArgumentOutOfRangeException(nameof(priority));
            }

            if (!NativeMethods.SetProcessShutdownParameters(priority, 0))
            {
                throw new Win32Exception();
            }
        }
    }
}
