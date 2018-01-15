// <copyright file="SystemService.cs" company="Heath Stewart">
// Copyright (c) 2017 Heath Stewart
// See the LICENSE.txt file in the project root for more information.
// </copyright>

namespace Caffeine.ViewModels
{
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

        /// <inheritdoc/>
        public IPowerRequest CreatePowerRequest(string reason) => new PowerRequest(reason);
    }
}
