using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Xunit;

namespace Intermedium.Tests
{
    public class VoidUnitTests
    {
        [Fact]
        public void CompareTo_ShouldReturnDefaultValue_WhenAnotherVoidUnitProvided()
        {
            var actual = VoidUnit.Value.CompareTo(default);

            actual.Should().Be(default);
        }

        [Theory]
        [MemberData(nameof(ComparisonCheck))]
        public void CompareTo_ShouldCompareValue_WhenAnotherVoidUnitProvided(object value)
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

        [Fact]
        public void Equals_ShouldReturnTrue_WhenAnotherVoidUnitProvided()
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

        [Theory]
        [MemberData(nameof(EqualityCheck))]
        public void Equals_ShouldBeEqualToAnotherVoidUnit_WhenAnotherVoidUnitProvided(object value, bool isEqual)
        {
            var nothing = VoidUnit.Value;

            (nothing == value).Should().Be(isEqual);
            (nothing != value).Should().Be(!isEqual);

            (value == nothing).Should().Be(isEqual);
            (value != nothing).Should().Be(!isEqual);

            nothing.Equals(value).Should().Be(isEqual);
        }

        [Fact]
        public void GetHashCode_ShouldReturnDefaultValue_WhenAnyVoidUnitProvided()
        {
            var actual = VoidUnit.Value.GetHashCode();

            actual.Should().Be(default);
        }

        [Fact]
        public void ToString_ShouldReturnTypeFullName_WhenAnyVoidUnitProvided()
        {
            var expected = typeof(VoidUnit).FullName;
            var actual = VoidUnit.Value.ToString();

            actual.Should().Be(expected);
        }

        public static IEnumerable<object[]> EqualityCheck
        {
            get
            {
                yield return new object[] { null!, false };
                yield return new object[] { new object(), false };
                yield return new object[] { VoidUnit.Value, true };
                yield return new object[] { new VoidUnit(), true };
                yield return new object[] { default(VoidUnit), true };
                yield return new object[] { VoidUnit.Value.ToString(), false };
                yield return new object[] { string.Empty, false };
            }
        }

        public static IEnumerable<object[]> ComparisonCheck => EqualityCheck.Select(x => new[] { x[0] });
    }
}
