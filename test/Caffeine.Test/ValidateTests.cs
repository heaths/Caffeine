// <copyright file="ValidateTests.cs" company="Heath Stewart">
// Copyright (c) 2017 Heath Stewart
// See the LICENSE.txt file in the project root for more information.
// </copyright>

namespace Caffeine
{
    using System;
    using Xunit;

    public class ValidateTests
    {
        [Fact]
        public void NotNull_Null_Throws()
        {
            Assert.Throws<ArgumentNullException>("test", () => Validate.NotNull(null, "test"));
        }

        [Fact]
        public void NotNull()
        {
            Validate.NotNull("test", "test");
        }

        [Fact]
        public void NotNullOrEmpty_Null_Throws()
        {
            Assert.Throws<ArgumentNullException>("test", () => Validate.NotNullOrEmpty(null, "test"));
        }

        [Fact]
        public void NotNullOrEmpty_Empty_Throws()
        {
            Assert.Throws<ArgumentException>("test", () => Validate.NotNullOrEmpty(string.Empty, "test"));
        }

        [Fact]
        public void NotNullOrEmpty()
        {
            Validate.NotNullOrEmpty("test", "test");
        }
    }
}
