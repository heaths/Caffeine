// <copyright file="IdleServiceTests.cs" company="Heath Stewart">
// Copyright (c) 2017 Heath Stewart
// See the LICENSE.txt file in the project root for more information.
// </copyright>

namespace Caffeine.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Moq;
    using Xunit;

    public class IdleServiceTests
    {
        [Fact]
        public void Duration_Negative_Throws()
        {
            var mockSystemService = new Mock<ISystemService>();

            var sut = new IdleService(mockSystemService.Object);
            Assert.Throws<ArgumentOutOfRangeException>(nameof(sut.Duration), () => sut.Duration = TimeSpan.FromMilliseconds(-1));
        }

        [Fact]
        public async Task Tick_Raised()
        {
            var mockSystemService = new Mock<ISystemService>();
            mockSystemService.SetupSequence(x => x.Ticks)
                .Returns(1000)
                .Returns(2000);
            mockSystemService.Setup(x => x.GetLastInput())
                .Returns(1000);

            var expected = TimeSpan.FromSeconds(1);
            using (var sut = new IdleService(mockSystemService.Object))
            {
                var ticks = new List<TimeSpan>();

                sut.Duration = expected;
                sut.Tick += (source, args) => ticks.Add(args.TimeSpan);

                await sut.StartAsync();

                Assert.Collection(
                    ticks,
                    x => Assert.Equal(TimeSpan.FromSeconds(1), x),
                    x => Assert.Equal(TimeSpan.FromSeconds(0), x));
            }
        }

        [Fact]
        public async Task Elapsed_Raised()
        {
            var mockSystemService = new Mock<ISystemService>();
            mockSystemService.SetupSequence(x => x.Ticks)
                .Returns(1000)
                .Returns(2000);
            mockSystemService.Setup(x => x.GetLastInput())
                .Returns(1000);

            var expected = TimeSpan.FromSeconds(1);
            using (var sut = new IdleService(mockSystemService.Object))
            {
                sut.Duration = expected;

                var evt = await Assert.RaisesAsync<TimerEventArgs>(
                    x => sut.Elapsed += x,
                    x => sut.Elapsed -= x,
                    () => sut.StartAsync());

                Assert.False(sut.IsStarted);
                Assert.Equal(expected, evt.Arguments.TimeSpan);
            }
        }

        [Fact]
        public void Start_IfDisposed_Throws()
        {
            var mockSystemService = new Mock<ISystemService>();

            var sut = new IdleService(mockSystemService.Object);
            sut.Dispose();

            Assert.Throws<ObjectDisposedException>(() => sut.Start());
        }

        [Fact]
        public async Task StartAsync_IfDisposed_Throws()
        {
            var mockSystemService = new Mock<ISystemService>();

            var sut = new IdleService(mockSystemService.Object);
            sut.Dispose();

            await Assert.ThrowsAsync<ObjectDisposedException>(() => sut.StartAsync());
        }

        [Fact]
        public void Stop_IfDisposed_Throws()
        {
            var mockSystemService = new Mock<ISystemService>();

            var sut = new IdleService(mockSystemService.Object);
            sut.Dispose();

            Assert.Throws<ObjectDisposedException>(() => sut.Stop());
        }
    }
}
