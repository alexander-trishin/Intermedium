using System;
using FluentAssertions;
using Intermedium.Core.Internal;
using Xunit;

namespace Intermedium.Tests
{
    public class GuardTests
    {
        [Fact]
        public void ThrowIfNull_ShouldReturnSameValue_WhenValueIsValueType()
        {
            int number = default;

            Guard.ThrowIfNull(number, nameof(number)).Should().Be(number);
        }

        [Fact]
        public void ThrowIfNull_ShouldReturnSameInstance_WhenValueIsReferenceType()
        {
            var text = "text";

            Guard.ThrowIfNull(text, nameof(text)).Should().BeSameAs(text);
        }

        [Fact]
        public void ThrowIfNull_ShouldThrowArgumentNullException_WhenValueIsNull()
        {
            bool? boolean = null;

            Action act = () => Guard.ThrowIfNull(boolean, nameof(boolean));

            act.Should()
                .ThrowExactly<ArgumentNullException>()
                .Where(x => x.Message.Contains(nameof(boolean)));
        }
    }
}
