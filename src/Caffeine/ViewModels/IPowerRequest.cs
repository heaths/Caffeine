// <copyright file="IPowerRequest.cs" company="Heath Stewart">
// Copyright (c) 2017 Heath Stewart
// See the LICENSE.txt file in the project root for more information.
// </copyright>

namespace Caffeine.ViewModels
{
    using System;
    using System.ComponentModel;

    /// <summary>
    /// Represents a power request.
    /// </summary>
    public interface IPowerRequest : IDisposable
    {
        /// <summary>
        /// Sets the <see cref="PowerRequestType"/>.
        /// </summary>
        /// <param name="type">The <see cref="PowerRequestType"/> to set.</param>
        /// <exception cref="Win32Exception">Failed to set the <see cref="PowerRequestType"/>.</exception>
        void Set(PowerRequestType type);

        /// <summary>
        /// Clear the <see cref="PowerRequestType"/>.
        /// </summary>
        /// <param name="type">The <see cref="PowerRequestType"/> to clear.</param>
        /// <exception cref="Win32Exception">Failed to clear the <see cref="PowerRequestType"/>.</exception>
        void Clear(PowerRequestType type);
    }
}
