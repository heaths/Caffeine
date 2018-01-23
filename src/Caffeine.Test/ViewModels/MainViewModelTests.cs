// <copyright file="MainViewModelTests.cs" company="Heath Stewart">
// Copyright (c) 2017 Heath Stewart
// See the LICENSE.txt file in the project root for more information.
// </copyright>

namespace Caffeine.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Threading;
    using Moq;
    using Xunit;

    public class MainViewModelTests
    {
        private readonly Mock<ISystemService> mockSystemService;
        private readonly Mock<IPowerRequest> mockPowerRequest;
        private readonly Mock<IDisposable> mockPowerRequestDisposable;

        public MainViewModelTests()
        {
            mockSystemService = new Mock<ISystemService>();

            mockPowerRequest = new Mock<IPowerRequest>();
            mockSystemService.Setup(x => x.CreatePowerRequest(It.IsAny<string>())).Returns(mockPowerRequest.Object);

            mockPowerRequestDisposable = mockPowerRequest.As<IDisposable>();
        }

        private ISystemService Service => mockSystemService.Object;

        [Fact]
        public void New_SystemServices_Null_Throws()
        {
            Assert.Throws<ArgumentNullException>("systemService", () => new MainViewModel(null));
        }

        [Fact]
        public void New_Sets_Shutdown_Priority()
        {
            var sut = new MainViewModel(Service);
            mockSystemService.Verify(x => x.SetShutdownParameters(0x3ff));
        }

        [Fact]
        public void New_Shutdown_Priority_Failure()
        {
            mockSystemService.Setup(x => x.SetShutdownParameters(It.IsAny<int>()))
                .Throws<Win32Exception>();

            new MainViewModel(Service);
        }

        [Fact]
        public void DisplayRequired_Creates_PowerRequest()
        {
            var sut = new MainViewModel(Service);
            mockSystemService.Verify(x => x.CreatePowerRequest(It.IsAny<string>()), Times.Never);

            Assert.PropertyChanged(sut, nameof(sut.DisplayRequired), () => sut.DisplayRequired = true);
            Assert.True(sut.DisplayRequired);

            mockSystemService.Verify(x => x.CreatePowerRequest(It.IsAny<string>()), Times.Once);
            mockPowerRequest.Verify(x => x.Set(PowerRequestType.DisplayRequired), Times.Once);
        }

        [Fact]
        public void DisplayRequired_Clears_PowerRequest()
        {
            var sut = new MainViewModel(Service)
            {
                DisplayRequired = true,
            };

            Assert.True(sut.DisplayRequired);
            Assert.PropertyChanged(sut, nameof(sut.DisplayRequired), () => sut.DisplayRequired = false);

            mockSystemService.Verify(x => x.CreatePowerRequest(It.IsAny<string>()), Times.Once);
            mockPowerRequest.Verify(x => x.Clear(PowerRequestType.DisplayRequired), Times.Once);
        }

        [Fact]
        public void DisplayRequired_Disposed_Throws()
        {
            var sut = new MainViewModel(Service);
            sut.Dispose();

            Assert.Throws<ObjectDisposedException>(() => sut.DisplayRequired = true);
        }

        [Fact]
        public void SystemRequired_Creates_PowerRequest()
        {
            var sut = new MainViewModel(Service);
            mockSystemService.Verify(x => x.CreatePowerRequest(It.IsAny<string>()), Times.Never);

            Assert.PropertyChanged(sut, nameof(sut.SystemRequired), () => sut.SystemRequired = true);
            Assert.True(sut.SystemRequired);

            mockSystemService.Verify(x => x.CreatePowerRequest(It.IsAny<string>()), Times.Once);
            mockPowerRequest.Verify(x => x.Set(PowerRequestType.SystemRequired), Times.Once);
        }

        [Fact]
        public void SystemRequired_Clears_PowerRequest()
        {
            var sut = new MainViewModel(Service)
            {
                SystemRequired = true,
            };

            Assert.True(sut.SystemRequired);
            Assert.PropertyChanged(sut, nameof(sut.SystemRequired), () => sut.SystemRequired = false);

            mockSystemService.Verify(x => x.CreatePowerRequest(It.IsAny<string>()), Times.Once);
            mockPowerRequest.Verify(x => x.Clear(PowerRequestType.SystemRequired), Times.Once);
        }

        [Fact]
        public void SystemRequired_Disposed_Throws()
        {
            var sut = new MainViewModel(Service);
            sut.Dispose();

            Assert.Throws<ObjectDisposedException>(() => sut.SystemRequired = true);
        }

        [Fact]
        public void SuspendShutdown_PropertyChanged()
        {
            var sut = new MainViewModel(Service);
            Assert.False(sut.SuspendShutdown);

            Assert.PropertyChanged(sut, nameof(sut.SuspendShutdown), () => sut.SuspendShutdown = true);
            Assert.True(sut.SuspendShutdown);
        }

        [Fact]
        public void SuspendShutdown_Changes_ShutdownVisibility()
        {
            var sut = new MainViewModel(Service);
            Assert.Equal(Visibility.Collapsed, sut.ShutdownVisibility);

            Assert.PropertyChanged(sut, nameof(sut.ShutdownVisibility), () => sut.SuspendShutdown = true);
            Assert.Equal(Visibility.Visible, sut.ShutdownVisibility);
        }

        [Fact]
        public void SuspendShutdown_Changes_CountdownVisibility()
        {
            var sut = new MainViewModel(Service);
            Assert.Equal(Visibility.Collapsed, sut.CountdownVisibility);

            Assert.PropertyChanged(sut, nameof(sut.CountdownVisibility), () => sut.SuspendShutdown = true);
            Assert.Equal(Visibility.Visible, sut.CountdownVisibility);
        }

        [Fact]
        public void CancelShutdown_PropertyChanged()
        {
            var sut = new MainViewModel(Service);
            Assert.False(sut.CancelShutdown);

            Assert.PropertyChanged(sut, nameof(sut.CancelShutdown), () => sut.CancelShutdown = true);
            Assert.True(sut.CancelShutdown);
        }

        [Fact]
        public void CancelShutdown_Changes_CountdownVisibility()
        {
            var sut = new MainViewModel(Service)
            {
                SuspendShutdown = true,
            };
            Assert.Equal(Visibility.Visible, sut.CountdownVisibility);

            Assert.PropertyChanged(sut, nameof(sut.CountdownVisibility), () => sut.CancelShutdown = true);
            Assert.Equal(Visibility.Collapsed, sut.CountdownVisibility);
        }

        [WpfFact]
        public async Task Countdown_Changes()
        {
            mockSystemService.SetupSequence(x => x.Ticks)
                .Returns(1000)
                .Returns(2000);
            mockSystemService.Setup(x => x.GetLastInput())
                .Returns(1000);

            var ticks = new List<TimeSpan>();

            var sut = new MainViewModel(Service, Dispatcher.CurrentDispatcher)
            {
                Duration = TimeSpan.FromSeconds(1),
            };

            sut.PropertyChanged += (source, args) =>
            {
                if ("Countdown".Equals(args.PropertyName))
                {
                    ticks.Add(sut.Countdown);
                }
            };

            sut.SuspendShutdown = true;
            await sut.WaitAsync();

            Assert.Collection(
                ticks,
                x => Assert.Equal(TimeSpan.FromSeconds(1), x),
                x => Assert.Equal(TimeSpan.FromSeconds(0), x));
        }

        [Fact]
        public void TopMost_PropertyChanged()
        {
            var sut = new MainViewModel(Service);
            Assert.False(sut.TopMost);

            Assert.PropertyChanged(sut, nameof(sut.TopMost), () => sut.TopMost = true);
            Assert.True(sut.TopMost);
        }

        [Fact]
        public void Dispose_Disposes_Services()
        {
            var sut = new MainViewModel(Service)
            {
                DisplayRequired = true,
            };
            Assert.False(sut.IsDisposed);

            sut.Dispose();
            Assert.True(sut.IsDisposed);
            mockPowerRequestDisposable.Verify(x => x.Dispose(), Times.Once);
        }

        [Fact]
        public void ProcessMessage_Handled()
        {
            const int WM_QUERYENDSESSION = 0x0011;

            var sut = new MainViewModel(Service)
            {
                SuspendShutdown = true,
            };

            var handled = false;
            var actual = sut.ProcessMessage(IntPtr.Zero, WM_QUERYENDSESSION, IntPtr.Zero, IntPtr.Zero, ref handled);

            Assert.Equal(IntPtr.Zero, actual);
            Assert.True(handled);
        }

        [Fact]
        public void ProcessMessage_Critical()
        {
            const int WM_QUERYENDSESSION = 0x0011;
            const int ENDSESSION_CRITICAL = 0x40000000;

            var sut = new MainViewModel(Service)
            {
                SuspendShutdown = true,
            };

            var handled = false;
            var actual = sut.ProcessMessage(IntPtr.Zero, WM_QUERYENDSESSION, IntPtr.Zero, new IntPtr(ENDSESSION_CRITICAL), ref handled);

            Assert.Equal(IntPtr.Zero, actual);
            Assert.False(handled);
        }

        [Fact]
        public void ProcessMessage_Unhandled()
        {
            const int WM_ENDSESSION = 0x0016;

            var sut = new MainViewModel(Service)
            {
                SuspendShutdown = true,
            };

            var handled = false;
            var actual = sut.ProcessMessage(IntPtr.Zero, WM_ENDSESSION, IntPtr.Zero, IntPtr.Zero, ref handled);

            Assert.Equal(IntPtr.Zero, actual);
            Assert.False(handled);
        }
    }
}
