using System;
using FluentAssertions;
using Intermedium.Core;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Intermedium.Extensions.Microsoft.DependencyInjection.Tests
{
    public class IntermediumServiceCollectionExtensionsTests
    {
        [Fact]
        public void AddIntermedium_ShouldThrowArgumentNullException_WhenServicesAreNull()
        {
            Action act = () => IntermediumServiceCollectionExtensions.AddIntermedium(null!);

            act.Should()
                .ThrowExactly<ArgumentNullException>()
                .Where(x => x.Message.Contains("services"));
        }

        [Fact]
        public void AddIntermedium_ShouldThrowInvalidOperationException_WhenAssemblyMarkersAreNotDefined()
        {
            Action overload1 = () => new ServiceCollection().AddIntermedium();
            Action overload2 = () => new ServiceCollection().AddIntermedium(x => x.Scan());

            overload1.Should().ThrowExactly<InvalidOperationException>();
            overload2.Should().ThrowExactly<InvalidOperationException>();
        }

        [Fact]
        public void AddIntermedium_ShouldThrowInvalidOperationException_WhenMediatorIsAlreadyRegistered()
        {
            Action act = () => new ServiceCollection()
                .AddTransient<IMediator, Mediator>()
                .AddIntermedium(typeof(IntermediumServiceCollectionExtensionsTests));

            act.Should().ThrowExactly<InvalidOperationException>();
        }

        [Fact]
        public void AddIntermedium_ShouldRegisterAllMediatorServicesFromTargetAssembly_WhenRequirementsAreMet()
        {
            var services = new ServiceCollection();

            services.AddIntermedium(typeof(IntermediumServiceCollectionExtensionsTests));

            var provider = services.BuildServiceProvider();

            provider.GetService<IMediator>().Should().NotBeNull();
        }
    }
}
