// <copyright file="BindableObject.cs" company="Heath Stewart">
// Copyright (c) 2017 Heath Stewart
// See the LICENSE.txt file in the project root for more information.
// </copyright>

namespace Caffeine
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// Represents a bindable object.
    /// </summary>
    public abstract class BindableObject : INotifyPropertyChanged
    {
        /// <inheritdoc/>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Fires <see cref="PropertyChanged"/> for the given <paramref name="propertyName"/>.
        /// </summary>
        /// <param name="propertyName">The name of the changed property. The default is the calling property.</param>
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Sets the <paramref name="field"/> if it differs from <paramref name="value"/> and fires <see cref="OnPropertyChanged(string)"/>.
        /// </summary>
        /// <typeparam name="T">The type of property to compare.</typeparam>
        /// <param name="field">The backing field to compare.</param>
        /// <param name="value">The new property value to compare.</param>
        /// <param name="comparer">The <see cref="IEqualityComparer{T}"/> to use for comparisons. The default is <see cref="EqualityComparer{T}.Default"/>.</param>
        /// <param name="propertyName">The name of the changed property. The default is the calling property.</param>
        /// <returns>A value indicating whether the <paramref name="field"/> was changed.</returns>
        protected bool SetProperty<T>(ref T field, T value, IEqualityComparer<T> comparer = null, [CallerMemberName] string propertyName = null)
        {
            comparer = comparer ?? EqualityComparer<T>.Default;

            if (!comparer.Equals(field, value))
            {
                field = value;
                OnPropertyChanged(propertyName);

                return true;
            }

            return false;
        }
    }
}
