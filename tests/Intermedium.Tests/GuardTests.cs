using System;
using FluentAssertions;
using Intermedium.Core.Internal;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Intermedium.Tests
{
    [TestClass]
    public class GuardTests
    {
        [TestMethod]
        public void ThrowIfNull_InstanceOfValueType_ReturnsSameValue()
        {
            int number = default;

            Guard.ThrowIfNull(number, nameof(number)).Should().Be(number);
        }

        [TestMethod]
        public void ThrowIfNull_InstanceOfReferenceType_ReturnsSameInstance()
        {
            var text = "text";

            Guard.ThrowIfNull(text, nameof(text)).Should().BeSameAs(text);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ThrowIfNull_NullValue_ThrowsArgumentNullException()
        {
            int? value = null;

            Guard.ThrowIfNull(value, nameof(value));
        }
    }
}
