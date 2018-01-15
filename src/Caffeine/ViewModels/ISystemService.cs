// <copyright file="ISystemService.cs" company="Heath Stewart">
// Copyright (c) 2017 Heath Stewart
// See the LICENSE.txt file in the project root for more information.
// </copyright>

namespace Caffeine.ViewModels
{
    using System;

    /// <summary>
    /// Represents system services.
    /// </summary>
    public interface ISystemService
    {
        /// <summary>
        /// Creates an <see cref="IPowerRequest"/>.
        /// </summary>
        /// <param name="reason">The reason the request was created.</param>
        /// <returns>An <see cref="IPowerRequest"/>.</returns>
        /// <exception cref="ArgumentException"><paramref name="reason"/> is empty.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="reason"/> is null.</exception>
        IPowerRequest CreatePowerRequest(string reason);
    }
}
