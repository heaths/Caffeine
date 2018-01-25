// <copyright file="ShutdownBlockReason.cs" company="Heath Stewart">
// Copyright (c) 2017 Heath Stewart
// See the LICENSE.txt file in the project root for more information.
// </copyright>

namespace Caffeine.ViewModels
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// A reason to block a shutdown request.
    /// </summary>
    [ExcludeFromCodeCoverage]
    internal class ShutdownBlockReason : IDisposable
    {
        private static readonly IntPtr InvalidHandle = new IntPtr(-1);

        private readonly ISystemService systemService;
        private readonly IntPtr handle;

        /// <summary>
        /// Initializes a new instance of the <see cref="ShutdownBlockReason"/> class.
        /// </summary>
        /// <param name="systemService">An <see cref="ISystemService"/> to get system information.</param>
        /// <param name="handle">A handle to a window.</param>
        /// <param name="reason">The reason to block a shutdown request.</param>
        public ShutdownBlockReason(ISystemService systemService, IntPtr handle, string reason)
        {
            this.systemService = systemService;
            this.handle = handle;

            if (IsSupported && IsValid && !NativeMethods.ShutdownBlockReasonCreate(handle, reason))
            {
                throw new Win32Exception();
            }
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="ShutdownBlockReason"/> class.
        /// </summary>
        ~ShutdownBlockReason()
        {
            Dispose(false);
        }

        /// <summary>
        /// Gets a value indicating whether this object is disposed.
        /// </summary>
        public bool IsDisposed { get; private set; }

        private bool IsSupported =>
            systemService.OSVersion != null && systemService.OSVersion >= NativeMethods.Vista;

        private bool IsValid => handle != InvalidHandle;

        /// <inheritdoc/>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Cancel()
        {
            if (IsSupported && IsValid && !NativeMethods.ShutdownBlockReasonDestroy(handle))
            {
                throw new Win32Exception();
            }
        }

        private void Dispose(bool disposing)
        {
            if (!IsDisposed)
            {
                Cancel();

                IsDisposed = true;
            }
        }
    }
}
