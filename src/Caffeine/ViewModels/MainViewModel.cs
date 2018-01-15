// <copyright file="MainViewModel.cs" company="Heath Stewart">
// Copyright (c) 2017 Heath Stewart
// See the LICENSE.txt file in the project root for more information.
// </copyright>

namespace Caffeine.ViewModels
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using Caffeine.Properties;

    /// <summary>
    /// The main ViewModel.
    /// </summary>
    public class MainViewModel : BindableObject, IDisposable
    {
        private readonly ISystemService service;

        private bool isDisposed;
        private IPowerRequest powerRequest;
        private bool displayRequired;
        private bool systemRequired;
        private bool topMost;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainViewModel"/> class.
        /// </summary>
        [ExcludeFromCodeCoverage]
        public MainViewModel()
            : this(null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MainViewModel"/> class.
        /// </summary>
        /// <param name="service">An <see cref="ISystemService"/> that provides system services. If null, <see cref="SystemService.Default"/> is used.</param>
        public MainViewModel(ISystemService service)
        {
            this.service = service ?? SystemService.Default;
        }

        /// <summary>
        /// Gets the title.
        /// </summary>
        [ExcludeFromCodeCoverage]
        public string Title => Resources.Title;

        /// <summary>
        /// Gets or sets a value indicating whether the display is required.
        /// </summary>
        public bool DisplayRequired
        {
            get => displayRequired;
            set => SetPowerRequest(PowerRequestType.DisplayRequired, ref displayRequired, value);
        }

        /// <summary>
        /// Gets the caption for <see cref="DisplayRequired"/>.
        /// </summary>
        [ExcludeFromCodeCoverage]
        public string DisplayRequiredCaption => Resources.DisplayRequiredCaption;

        /// <summary>
        /// Gets or sets a value indicating whether the system is required.
        /// </summary>
        public bool SystemRequired
        {
            get => systemRequired;
            set => SetPowerRequest(PowerRequestType.SystemRequired, ref systemRequired, value);
        }

        /// <summary>
        /// Gets the caption for <see cref="SystemRequired"/>.
        /// </summary>
        [ExcludeFromCodeCoverage]
        public string SystemRequiredCaption => Resources.SystemRequiredCaption;

        /// <summary>
        /// Gets or sets a value indicating whether the window should remain on top of others.
        /// </summary>
        public bool TopMost
        {
            get => topMost;
            set => SetProperty(ref topMost, value);
        }

        /// <summary>
        /// Gets the caption for <see cref="TopMost"/>.
        /// </summary>
        [ExcludeFromCodeCoverage]
        public string TopMostCaption => Resources.TopMostCaption;

        /// <summary>
        /// Gets a value indicating whether this object is disposed.
        /// </summary>
        public bool IsDisposed
        {
            get => isDisposed;
            private set => SetProperty(ref isDisposed, value);
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Disposes this object.
        /// </summary>
        /// <param name="disposing">A value indicating whether this object is being disposed.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!IsDisposed)
            {
                if (disposing)
                {
                    powerRequest?.Dispose();
                }

                IsDisposed = true;
            }
        }

        private void EnsurePowerRequest() =>
            LazyInitializer.EnsureInitialized(ref powerRequest, () => service.CreatePowerRequest(Resources.PowerRequestReason));

        private void SetPowerRequest(PowerRequestType type, ref bool field, bool value, [CallerMemberName] string propertyName = null)
        {
            ThrowIfDisposed();

            if (field != value)
            {
                EnsurePowerRequest();

                if (value)
                {
                    powerRequest.Set(type);
                }
                else
                {
                    powerRequest.Clear(type);
                }

                field = value;
                OnPropertyChanged(propertyName);
            }
        }

        private void ThrowIfDisposed()
        {
            if (IsDisposed)
            {
                throw new ObjectDisposedException(nameof(MainViewModel));
            }
        }
    }
}
