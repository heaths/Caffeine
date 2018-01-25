// <copyright file="IdleService.cs" company="Heath Stewart">
// Copyright (c) 2017 Heath Stewart
// See the LICENSE.txt file in the project root for more information.
// </copyright>

namespace Caffeine.ViewModels
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Detects when the system idle.
    /// </summary>
    public class IdleService : IDisposable
    {
        // Raise the Timer.Elapsed every second.
        private static readonly TimeSpan Period = TimeSpan.FromSeconds(1);

        private readonly ISystemService systemService;
        private readonly TimerCallback callback;
        private readonly ManualResetEventSlim completionEvent;

        private TimeSpan duration = TimeSpan.FromMinutes(10);
        private object cookie;
        private Timer timer;

        /// <summary>
        /// Initializes a new instance of the <see cref="IdleService"/> class.
        /// </summary>
        /// <param name="systemService">An <see cref="ISystemService"/> that provides system services. If null, <see cref="SystemService.Default"/> is used.</param>
        public IdleService(ISystemService systemService)
        {
            this.systemService = systemService ?? SystemService.Default;

            callback = new TimerCallback(OnElapsed);
            completionEvent = new ManualResetEventSlim(false);
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="IdleService"/> class.
        /// </summary>
        ~IdleService()
        {
            Dispose(false);
        }

        /// <summary>
        /// Event raised when the timer has elapsed.
        /// </summary>
        public event EventHandler<TimerEventArgs> Tick;

        /// <summary>
        /// Event raised when the system has been idle for the given <see cref="Duration"/>.
        /// </summary>
        /// <remarks>
        /// The timer is stopped automatically when <see cref="Elapsed"/> is raised.
        /// </remarks>
        public event EventHandler<TimerEventArgs> Elapsed;

        /// <summary>
        /// Gets a value indicating whether this object is disposed.
        /// </summary>
        public bool IsDisposed { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the timer is started.
        /// </summary>
        public bool IsStarted => timer != null;

        /// <summary>
        /// Gets or sets the time required before considering the system idle.
        /// </summary>
        /// <value>The default is 10 minutes.</value>
        /// <exception cref="ArgumentOutOfRangeException">The value is negative.</exception>
        public TimeSpan Duration
        {
            get => duration;
            set
            {
                if (value < TimeSpan.Zero)
                {
                    throw new ArgumentOutOfRangeException(nameof(Duration));
                }

                duration = value;
            }
        }

        /// <summary>
        /// Gets a <see cref="WaitHandle"/> that is signaled on <see cref="Stop"/> or when <see cref="Elapsed"/> has been raised.
        /// </summary>
        public WaitHandle WaitHandle => completionEvent.WaitHandle;

        /// <summary>
        /// Starts idle detection.
        /// </summary>
        public void Start()
        {
            ThrowIfDisposed();

            Close();

            cookie = new object();
            timer = new Timer(callback, cookie, TimeSpan.Zero, Period);
        }

        /// <summary>
        /// Starts idle detection and asynchronously waits until <see cref="Elapsed"/> is raised.
        /// </summary>
        /// <param name="cancellationToken">Optional <see cref="CancellationToken"/> to cancel waiting for completion.</param>
        /// <returns>A <see cref="Task"/> with which to wait.</returns>
        /// <exception cref="OperationCanceledException">Waiting for completion was cancelled.</exception>
        public async Task StartAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            ThrowIfDisposed();

            Start();
            await Task.Run(() => completionEvent.Wait(cancellationToken));
        }

        /// <summary>
        /// Stops idle detection.
        /// </summary>
        public void Stop()
        {
            ThrowIfDisposed();

            Close();
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Disposes the current object.
        /// </summary>
        /// <param name="disposing">True if the object is being disposed; otherwise, false to indicate the object is being finalized.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!IsDisposed)
            {
                Close();

                IsDisposed = true;
            }
        }

        private void Close()
        {
            if (timer != null)
            {
                timer.Dispose();

                cookie = null;
                timer = null;
            }
        }

        private void OnElapsed(object state)
        {
            if (state != cookie)
            {
                return;
            }

            var lastActivity = systemService.GetLastInput();
            if (lastActivity != 0)
            {
                var elapsed = TimeSpan.FromMilliseconds(systemService.Ticks - lastActivity);
                Tick?.Invoke(this, new TimerEventArgs(Duration - elapsed));

                if (elapsed >= duration)
                {
                    Elapsed?.Invoke(this, new TimerEventArgs(elapsed));

                    Close();
                    completionEvent.Set();
                }
            }
        }

        private void ThrowIfDisposed()
        {
            if (IsDisposed)
            {
                throw new ObjectDisposedException(nameof(IdleService));
            }
        }
    }
}
