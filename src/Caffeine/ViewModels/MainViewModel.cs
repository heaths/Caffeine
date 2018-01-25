// <copyright file="MainViewModel.cs" company="Heath Stewart">
// Copyright (c) 2017 Heath Stewart
// See the LICENSE.txt file in the project root for more information.
// </copyright>

namespace Caffeine.ViewModels
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Threading;
    using Caffeine.Properties;

    /// <summary>
    /// The main ViewModel.
    /// </summary>
    public class MainViewModel : BindableObject, IDisposable
    {
        private const int ShutdownPriority = 0x3ff;
        private static readonly string TimeSpanFormat = @"hh\:mm\:ss";

        private readonly ISystemService systemService;
        private readonly Dispatcher dispatcher;
        private readonly IdleService idleService;

        private bool isDisposed;
        private IPowerRequest powerRequest;
        private bool displayRequired;
        private bool systemRequired;
        private bool suspendShutdown;
        private bool cancelShutdown;
        private TimeSpan countdown;
        private bool topMost;
        private bool isStarted;
        private bool isShutdownRequested;
        private bool stopProcessingMessages;
        private IDisposable blockShutdown;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainViewModel"/> class.
        /// </summary>
        [ExcludeFromCodeCoverage]
        public MainViewModel()
            : this(SystemService.Default, Application.Current?.Dispatcher)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MainViewModel"/> class.
        /// </summary>
        /// <param name="systemService">An <see cref="ISystemService"/> that provides system services.</param>
        /// <param name="dispatcher">Optional <see cref="Dispatcher"/> to dispatch events to the UI thread.</param>
        /// <exception cref="ArgumentNullException"><paramref name="systemService"/> is null.</exception>
        public MainViewModel(ISystemService systemService, Dispatcher dispatcher = null)
        {
            Validate.NotNull(systemService, nameof(systemService));

            this.systemService = systemService;
            this.dispatcher = dispatcher;

            idleService = new IdleService(this.systemService);
            idleService.Elapsed += OnElapsed;
            idleService.Tick += OnTick;

            Countdown = Duration;

            // Attempt to prioritize ourselves when a shutdown request occurs.
            try
            {
                systemService.SetShutdownParameters(ShutdownPriority);
            }
            catch
            {
            }
        }

        /// <summary>
        /// Gets or sets the duration the machine can be idle before shutting down after a request is received.
        /// </summary>
        public TimeSpan Duration
        {
            get => idleService.Duration;
            set
            {
                if (idleService.Duration != value)
                {
                    idleService.Duration = value;
                    OnPropertyChanged(nameof(Duration));
                }
            }
        }

        /// <summary>
        /// Gets or sets the window handle.
        /// </summary>
        [ExcludeFromCodeCoverage]
        public IntPtr Handle { get; set; }

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
        /// Gets or sets a value indicating whether shutdown should be suspended.
        /// </summary>
        public bool SuspendShutdown
        {
            get => suspendShutdown;
            set
            {
                if (SetProperty(ref suspendShutdown, value))
                {
                    BlockShutdown();

                    OnPropertyChanged(nameof(ShutdownVisibility));
                    OnPropertyChanged(nameof(WaitingVisibility));
                    OnPropertyChanged(nameof(CountdownVisibility));
                }
            }
        }

        /// <summary>
        /// Gets the caption for <see cref="SuspendShutdown"/>.
        /// </summary>
        [ExcludeFromCodeCoverage]
        public string SuspendShutdownCaption => Resources.SuspendShutdownCaption;

        /// <summary>
        /// Gets or sets a value indicating whether shutdown should be canceled.
        /// </summary>
        public bool CancelShutdown
        {
            get => cancelShutdown;
            set
            {
                if (SetProperty(ref cancelShutdown, value))
                {
                    BlockShutdown();

                    OnPropertyChanged(nameof(WaitingVisibility));
                    OnPropertyChanged(nameof(CountdownVisibility));
                }
            }
        }

        /// <summary>
        /// Gets the caption for <see cref="CancelShutdown"/>.
        /// </summary>
        [ExcludeFromCodeCoverage]
        public string CancelShutdownCaption => Resources.CancelShutdownCaption;

        /// <summary>
        /// Gets the <see cref="Visibility"/> for the shutdown panel based on <see cref="SuspendShutdown"/>.
        /// </summary>
        public Visibility ShutdownVisibility =>
            SuspendShutdown ? Visibility.Visible : Visibility.Collapsed;

        /// <summary>
        /// Gets the <see cref="Visibility"/> for the countdown timer based on <see cref="SuspendShutdown"/> and <see cref="CancelShutdown"/>.
        /// </summary>
        public Visibility CountdownVisibility =>
            ShouldSuspendShutdown && IsShutdownRequested ? Visibility.Visible : Visibility.Collapsed;

        /// <summary>
        /// Gets the <see cref="Visibility"/> of the <see cref="WaitingCaption"/> label.
        /// </summary>
        public Visibility WaitingVisibility =>
            (ShouldSuspendShutdown && !IsShutdownRequested) ? Visibility.Visible : Visibility.Collapsed;

        /// <summary>
        /// Gets the caption for the "Waiting for shutdown request" label.
        /// </summary>
        [ExcludeFromCodeCoverage]
        public string WaitingCaption => Resources.WaitingCaption;

        /// <summary>
        /// Gets the duration for idle detection.
        /// </summary>
        public TimeSpan Countdown
        {
            [ExcludeFromCodeCoverage]
            get => countdown;
            private set
            {
                if (SetProperty(ref countdown, value))
                {
                    OnPropertyChanged(nameof(CountdownFormatted));
                }
            }
        }

        /// <summary>
        /// Gets the <see cref="Countdown"/> formatted for the <see cref="CultureInfo.CurrentCulture"/>.
        /// </summary>
        [ExcludeFromCodeCoverage]
        public string CountdownFormatted => countdown.ToString(TimeSpanFormat, CultureInfo.CurrentCulture);

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
        /// Gets or sets a value indicating whether the <see cref="Countdown"/> has started.
        /// </summary>
        public bool IsStarted
        {
            get => isStarted;
            set
            {
                IsShutdownRequested = value;

                if (SetProperty(ref isStarted, value))
                {
                    OnPropertyChanged(nameof(WaitingVisibility));
                    OnPropertyChanged(nameof(CountdownVisibility));
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether this object is disposed.
        /// </summary>
        public bool IsDisposed
        {
            get => isDisposed;
            private set => SetProperty(ref isDisposed, value);
        }

        private bool ShouldSuspendShutdown => SuspendShutdown && !CancelShutdown;

        private bool IsShutdownRequested
        {
            get => isShutdownRequested;
            set => isShutdownRequested = value || isShutdownRequested;
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Processes windows messages.
        /// </summary>
        /// <param name="handle">The window handle.</param>
        /// <param name="message">The message ID.</param>
        /// <param name="wParam">The message's WPARAM value.</param>
        /// <param name="lParam">The message's LPARAM value.</param>
        /// <param name="handled">A value that indicates whether the message was handled. Set the value to true if the message was handled; otherwise, false.</param>
        /// <returns>The appropriate return value depends on the particular message.</returns>
        public IntPtr ProcessMessage(IntPtr handle, int message, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (!stopProcessingMessages)
            {
                switch (message)
                {
                    case NativeMethods.WM_QUERYENDSESSION
                    when lParam.ToInt32() != NativeMethods.ENDSESSION_CRITICAL && SuspendShutdown:
                        IsShutdownRequested = true;

                        if (!CancelShutdown)
                        {
                            Start();
                        }

                        handled = true;
                        return IntPtr.Zero;
                }
            }

            handled = false;
            return IntPtr.Zero;
        }

        /// <summary>
        /// Starts the <see cref="IdleService"/> and waits till the <see cref="IdleService.Elapsed"/> event is raised.
        /// </summary>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> to cancel waiting.</param>
        /// <returns>A <see cref="Task"/> to await.</returns>
        public async Task StartAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            ThrowIfDisposed();

            IsStarted = true;

            await idleService.StartAsync(cancellationToken);
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
                    blockShutdown?.Dispose();
                    powerRequest?.Dispose();
                }

                IsDisposed = true;
            }
        }

        private void BlockShutdown()
        {
            if (blockShutdown != null)
            {
                blockShutdown.Dispose();
                blockShutdown = null;
            }

            if (SuspendShutdown)
            {
                blockShutdown = systemService.BlockShutdown(Handle, Resources.PowerRequestReason);
            }

            if (ShouldSuspendShutdown && IsShutdownRequested)
            {
                Start();
            }
            else
            {
                Stop();
            }
        }

        private void EnsurePowerRequest() =>
            LazyInitializer.EnsureInitialized(ref powerRequest, () => systemService.CreatePowerRequest(Resources.PowerRequestReason));

        private void OnElapsed(object source, TimerEventArgs args)
        {
            Stop();

            stopProcessingMessages = true;
            systemService.Shutdown();
        }

        private void OnTick(object source, TimerEventArgs args)
        {
            void SetCoundown() => Countdown = args.TimeSpan;
            dispatcher?.BeginInvoke((Action)SetCoundown, DispatcherPriority.Render);
        }

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

        private void Start()
        {
            ThrowIfDisposed();

            IsStarted = true;

            idleService.Start();
        }

        private void Stop()
        {
            ThrowIfDisposed();

            idleService.Stop();

            IsStarted = false;
            Countdown = Duration;
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
