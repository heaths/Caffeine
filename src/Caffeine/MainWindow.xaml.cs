// <copyright file="MainWindow.xaml.cs" company="Heath Stewart">
// Copyright (c) 2017 Heath Stewart
// See the LICENSE.txt file in the project root for more information.
// </copyright>

namespace Caffeine
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading;
    using System.Windows;
    using System.Windows.Interop;
    using Caffeine.ViewModels;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    [ExcludeFromCodeCoverage]
    public partial class MainWindow : Window
    {
        private readonly MainViewModel viewModel;
        private WindowInteropHelper helper;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow"/> class.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            viewModel = DataContext as MainViewModel;
            if (viewModel != null)
            {
                viewModel.Handle = Handle;

                var source = HwndSource.FromHwnd(Handle);
                source.AddHook(new HwndSourceHook(viewModel.ProcessMessage));
            }
        }

        /// <summary>
        /// Gets the native handle for this window.
        /// </summary>
        internal IntPtr Handle =>
            LazyInitializer.EnsureInitialized(ref helper, () => new WindowInteropHelper(this)).EnsureHandle();

        /// <inheritdoc/>
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            viewModel?.Dispose();
        }
    }
}
