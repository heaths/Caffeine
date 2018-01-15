// <copyright file="Validate.cs" company="Heath Stewart">
// Copyright (c) 2017 Heath Stewart
// See the LICENSE.txt file in the project root for more information.
// </copyright>

namespace Caffeine
{
    using System;
    using Caffeine.Properties;

    /// <summary>
    /// Parameter validation methods.
    /// </summary>
    public static class Validate
    {
        /// <summary>
        /// Throws an <see cref="ArgumentNullException"/> if <paramref name="param"/> is null.
        /// </summary>
        /// <param name="param">The parameter to validate.</param>
        /// <param name="paramName">The name of the parameter.</param>
        /// <exception cref="ArgumentNullException"><paramref name="param"/> is null.</exception>
        public static void NotNull(object param, string paramName)
        {
            if (param is null)
            {
                throw new ArgumentNullException(paramName);
            }
        }

        /// <summary>
        /// Throws an <see cref="ArgumentNullException"/> if <paramref name="param"/> is null or empty.
        /// </summary>
        /// <param name="param">The parameter to validate.</param>
        /// <param name="paramName">The name of the parameter.</param>
        /// <exception cref="ArgumentException"><paramref name="param"/> is empty.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="param"/> is null.</exception>
        public static void NotNullOrEmpty(string param, string paramName)
        {
            NotNull(param, paramName);

            if (param.Length == 0)
            {
                throw new ArgumentException(Resources.NotEmpty, paramName);
            }
        }
    }
}
