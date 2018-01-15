// <copyright file="MainViewModelTests.cs" company="Heath Stewart">
// Copyright (c) 2017 Heath Stewart
// See the LICENSE.txt file in the project root for more information.
// </copyright>

namespace Caffeine.ViewModels
{
    using System;
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
    }
}
