using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Intermedium.Tests
{
    [TestClass]
    public class VoidUnitTests
    {
        [TestMethod]
        public void CompareTo_AnotherVoidUnit_ReturnsDefault()
        {
            VoidUnit.Value.CompareTo(default).Should().Be(default);
        }

        [DataTestMethod]
        [DynamicData(nameof(ComparisonCheck), DynamicDataSourceType.Property)]
        public void CompareTo_AnotherObject(object value)
        {
            var nothing = VoidUnit.Value;

            nothing.CompareTo(value).Should().Be(default);

            (nothing < value).Should().BeFalse();
            (nothing <= value).Should().BeTrue();
            (nothing > value).Should().BeFalse();
            (nothing >= value).Should().BeTrue();

            (value < nothing).Should().BeFalse();
            (value <= nothing).Should().BeTrue();
            (value > nothing).Should().BeFalse();
            (value >= nothing).Should().BeTrue();
        }

        [TestMethod]
        public void Equals_AnotherVoidUnit_ReturnsTrue()
        {
            var left = default(VoidUnit);
            var right = VoidUnit.Value;

            (left == right).Should().BeTrue();
            (left != right).Should().BeFalse();
            (left > right).Should().BeFalse();
            (left >= right).Should().BeTrue();
            (left < right).Should().BeFalse();
            (left <= right).Should().BeTrue();

            left.Equals(right).Should().BeTrue();
        }

        [DataTestMethod]
        [DynamicData(nameof(EqualityCheck), DynamicDataSourceType.Property)]
        public void Equals_AnotherObject(object value, bool isEqual)
        {
            var nothing = VoidUnit.Value;

            (nothing == value).Should().Be(isEqual);
            (nothing != value).Should().Be(!isEqual);

            (value == nothing).Should().Be(isEqual);
            (value != nothing).Should().Be(!isEqual);

            nothing.Equals(value).Should().Be(isEqual);
        }

        [TestMethod]
        public void GetHashCode_ReturnsDefaultValue()
        {
            VoidUnit.Value.GetHashCode().Should().Be(default);
        }

        [TestMethod]
        public void ToString_ReturnsTypeFullName()
        {
            VoidUnit.Value.ToString().Should().Be(typeof(VoidUnit).FullName);
        }

        private static IEnumerable<object[]> EqualityCheck
        {
            get
            {
                yield return new object[] { null, false };
                yield return new object[] { new object(), false };
                yield return new object[] { VoidUnit.Value, true };
                yield return new object[] { new VoidUnit(), true };
                yield return new object[] { default(VoidUnit), true };
                yield return new object[] { VoidUnit.Value.ToString(), false };
                yield return new object[] { string.Empty, false };
            }
        }

        private static IEnumerable<object[]> ComparisonCheck => EqualityCheck.Select(x => new[] { x[0] });
    }
}
