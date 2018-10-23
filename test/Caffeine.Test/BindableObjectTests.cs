// <copyright file="BindableObjectTests.cs" company="Heath Stewart">
// Copyright (c) 2017 Heath Stewart
// See the LICENSE.txt file in the project root for more information.
// </copyright>

namespace Caffeine
{
    using System;
    using System.Collections.Generic;
    using Moq;
    using Xunit;

    public class BindableObjectTests
    {
        [Fact]
        public void PropertyChanged_Compared()
        {
            var sut = new TestObject();
            Assert.PropertyChanged(sut, nameof(sut.Compared), () => sut.Compared = true);

            sut.PropertyChanged += (source, args) => throw new Exception("PropertyChanged should not have been raised");
            sut.Compared = true;
        }

        [Fact]
        public void PropertyChanged_NonCompared()
        {
            var sut = new TestObject();
            Assert.PropertyChanged(sut, nameof(sut.NonCompared), () => sut.NonCompared = true);
            Assert.PropertyChanged(sut, nameof(sut.NonCompared), () => sut.NonCompared = true);
        }

        [Fact]
        public void SetProperty_Compared()
        {
            var comparer = new Mock<IEqualityComparer<bool>>();
            comparer.Setup(x => x.Equals(It.IsAny<bool>(), It.IsAny<bool>())).Returns(false);

            var sut = new TestObject(comparer.Object);
            Assert.PropertyChanged(sut, nameof(sut.Compared), () => sut.Compared = true);
            Assert.PropertyChanged(sut, nameof(sut.Compared), () => sut.Compared = true);
        }

        private class TestObject : BindableObject
        {
            private readonly IEqualityComparer<bool> comparer;

            private bool compared;
            private bool nonCompared;

            public TestObject(IEqualityComparer<bool> comparer = null)
            {
                this.comparer = comparer;
            }

            public bool Compared
            {
                get => compared;
                set => SetProperty(ref compared, value, comparer);
            }

            public bool NonCompared
            {
                get => nonCompared;
                set
                {
                    nonCompared = value;
                    OnPropertyChanged();
                }
            }
        }
    }
}
