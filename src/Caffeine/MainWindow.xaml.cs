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

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private WindowInteropHelper helper;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow"/> class.
        /// </summary>
        [ExcludeFromCodeCoverage]
        public MainWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Gets the native handle for this window.
        /// </summary>
        [ExcludeFromCodeCoverage]
        internal IntPtr Handle => LazyInitializer.EnsureInitialized(ref helper, () => new WindowInteropHelper(this)).Handle;
    }
}
