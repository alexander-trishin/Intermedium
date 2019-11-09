using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Intermedium.Core;
using Intermedium.Core.Internal;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Intermedium.Tests
{
    [TestClass]
    public class ServiceProviderExtensionsTests
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetService_NullServiceProvider_ThrowsArgumentNullException()
        {
            ServiceProvider provider = null;
            provider.GetService<object>();
        }

        [TestMethod]
        public void GetService_RegisteredService_ReturnsService()
        {
            var registeredService = new Mock<IRegisteredService>().Object;
            var provider = new ServiceProvider(serviceType =>
            {
                return serviceType == typeof(IRegisteredService)
                    ? registeredService
                    : null;
            });
            provider.GetService<IRegisteredService>().Should().NotBeNull();
        }

        [TestMethod]
        public void GetServices_NotRegisteredService_ReturnsNull()
        {
            var provider = new ServiceProvider(serviceType => null);
            provider.GetServices<INotRegisteredService>().Should().BeNull();
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void GetRequiredService_NotRegisteredService_ThrowsInvalidOperationException()
        {
            var provider = new ServiceProvider(serviceType => null);
            provider.GetRequiredService<INotRegisteredService>();
        }

        [TestMethod]
        public void GetRequiredServices_RegisteredService_ReturnsServices()
        {
            const int count = 5;

            var registeredService = new Mock<IRegisteredService>().Object;
            var provider = new ServiceProvider(serviceType =>
            {
                return serviceType == typeof(IEnumerable<IRegisteredService>)
                    ? Enumerable.Repeat(registeredService, count)
                    : null;
            });

            var services = provider.GetRequiredServices<IRegisteredService>();

            services.Should().NotBeNullOrEmpty().And.HaveCount(count);
        }

        public interface IRegisteredService { }

        public interface INotRegisteredService { }
    }
}
