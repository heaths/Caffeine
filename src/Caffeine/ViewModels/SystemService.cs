// <copyright file="SystemService.cs" company="Heath Stewart">
// Copyright (c) 2017 Heath Stewart
// See the LICENSE.txt file in the project root for more information.
// </copyright>

namespace Caffeine.ViewModels
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Runtime.InteropServices;

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

        /// <inheritdoc/>
        public void Shutdown(bool restart = false)
        {
            EnablePrivilege(NativeMethods.SE_SHUTDOWN_NAME);

            if (!NativeMethods.InitiateSystemShutdown(null, null, 0, false, restart))
            {
                throw new Win32Exception();
            }
        }

        private void EnablePrivilege(string name)
        {
            if (!NativeMethods.OpenProcessToken(
                Process.GetCurrentProcess().Handle,
                NativeMethods.TOKEN_QUERY | NativeMethods.TOKEN_ADJUST_PRIVILEGES,
                out var handle))
            {
                throw new Win32Exception();
            }

            using (handle)
            {
                var tkp = new TOKEN_PRIVILEGES
                {
                    PrivilegeCount = 1,
                    Attributes = NativeMethods.SE_PRIVILEGE_ENABLED,
                };

                if (!NativeMethods.LookupPrivilegeValue(null, name, out tkp.Luid))
                {
                    throw new Win32Exception();
                }

                if (!NativeMethods.AdjustTokenPrivileges(
                    handle,
                    false,
                    ref tkp,
                    Marshal.SizeOf(tkp.GetType()),
                    IntPtr.Zero,
                    out _))
                {
                    throw new Win32Exception();
                }
            }
        }
    }
}
