using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Intermedium.Core;
using Intermedium.Core.Internal;
using Moq;
using Xunit;

namespace Intermedium.Tests
{
    public class ServiceProviderExtensionsTests
    {
        [Fact]
        public void GetService_ShouldThrowArgumentNullException_WhenServiceProviderParameterIsNull()
        {
            ServiceProvider provider = null!;

            Action act = () => provider.GetService<object>();

            act.Should()
                .ThrowExactly<ArgumentNullException>()
                .Where(x => x.Message.Contains(nameof(provider)));
        }

        [Fact]
        public void GetService_ShouldReturnService_WhenThisServiceWasRegistered()
        {
            var registeredService = new Mock<IRegisteredService>().Object;

            var provider = new ServiceProvider(serviceType =>
            {
                return serviceType == typeof(IRegisteredService)
                    ? registeredService
                    : null!;
            });

            var actual = provider.GetService<IRegisteredService>();

            actual.Should().NotBeNull();
        }

        [Fact]
        public void GetServices_ShouldReturnNull_WhenServiceWasNotRegistered()
        {
            var provider = new ServiceProvider(serviceType => null!);

            var actual = provider.GetServices<INotRegisteredService>();

            actual.Should().BeNull();
        }

        [Fact]
        public void GetRequiredService_ShouldThrowInvalidOperationException_WhenServiceWasNotRegistered()
        {
            var provider = new ServiceProvider(serviceType => null!);

            Action act = () => provider.GetRequiredService<INotRegisteredService>();

            act.Should()
                .ThrowExactly<InvalidOperationException>()
                .WithMessage($"*{typeof(INotRegisteredService).FullName}*");
        }

        [Fact]
        public void GetRequiredServices_ShouldReturnServices_WhenServicesWereRegistered()
        {
            const int count = 5;

            var registeredService = new Mock<IRegisteredService>().Object;
            var provider = new ServiceProvider(serviceType =>
            {
                return serviceType == typeof(IEnumerable<IRegisteredService>)
                    ? Enumerable.Repeat(registeredService, count)
                    : null!;
            });

            var actual = provider.GetRequiredServices<IRegisteredService>();

            actual.Should()
                .NotBeNullOrEmpty()
                .And.HaveCount(count);
        }

        public interface IRegisteredService { }

        public interface INotRegisteredService { }
    }
}
