// <copyright file="SafeObjectHandle.cs" company="Heath Stewart">
// Copyright (c) 2017 Heath Stewart
// See the LICENSE.txt file in the project root for more information.
// </copyright>

namespace Caffeine.ViewModels
{
    using System.Diagnostics.CodeAnalysis;
    using System.Security;
    using Microsoft.Win32.SafeHandles;

    /// <summary>
    /// A handle for a system power request.
    /// </summary>
    [ExcludeFromCodeCoverage]
    [SecurityCritical]
    internal sealed class SafeObjectHandle : SafeHandleMinusOneIsInvalid
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SafeObjectHandle"/> class.
        /// </summary>
        public SafeObjectHandle()
            : base(true)
        {
        }

        /// <summary>
        /// Closes the handle.
        /// </summary>
        /// <returns>A value indicating whether the handle was successfully closed.</returns>
        [SecurityCritical]
        protected override bool ReleaseHandle()
        {
            return NativeMethods.CloseHandle(handle);
        }
    }
}
