// <copyright file="PowerRequest.cs" company="Heath Stewart">
// Copyright (c) 2017 Heath Stewart
// See the LICENSE.txt file in the project root for more information.
// </copyright>

namespace Caffeine.ViewModels
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// A system power request.
    /// </summary>
    [ExcludeFromCodeCoverage]
    internal sealed class PowerRequest : IPowerRequest
    {
        private readonly SafePowerRequestHandle handle;

        /// <summary>
        /// Initializes a new instance of the <see cref="PowerRequest"/> class.
        /// </summary>
        /// <param name="reason">Reason for the power request.</param>
        /// <exception cref="ArgumentException"><paramref name="reason"/> is empty.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="reason"/> is null.</exception>
        public PowerRequest(string reason)
        {
            Validate.NotNullOrEmpty(reason, nameof(reason));

            var context = new REASON_CONTEXT(reason);

            handle = NativeMethods.PowerCreateRequest(ref context);
            if (handle.IsInvalid)
            {
                throw new Win32Exception();
            }
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="PowerRequest"/> class.
        /// </summary>
        ~PowerRequest()
        {
            Dispose(false);
        }

        /// <inheritdoc/>
        public void Clear(PowerRequestType type)
        {
            if (!NativeMethods.PowerClearRequest(handle, type))
            {
                throw new Win32Exception();
            }
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <inheritdoc/>
        public void Set(PowerRequestType type)
        {
            if (!NativeMethods.PowerSetRequest(handle, type))
            {
                throw new Win32Exception();
            }
        }

        private void Dispose(bool disposing)
        {
            if (handle != null && !handle.IsClosed)
            {
                handle.Dispose();
            }
        }
    }
}
