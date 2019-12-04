using System;
using FluentAssertions;
using Intermedium.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Intermedium.Extensions.Microsoft.DependencyInjection.Tests
{
    [TestClass]
    public class IntermediumServiceCollectionExtensionsTests
    {
        [TestMethod]
        public void AddIntermedium_ServicesAreNull_ThrowsArgumentNullException()
        {
            Action overload1 = () => IntermediumServiceCollectionExtensions.AddIntermedium(null);
            Action overload2 = () => IntermediumServiceCollectionExtensions.AddIntermedium(null, x => { });

            overload1.Should().ThrowExactly<ArgumentNullException>();
            overload2.Should().ThrowExactly<ArgumentNullException>();
        }

        [TestMethod]
        public void AddIntermedium_AssemblyMarkersAreNotDefined_ThrowsInvalidOperationException()
        {
            Action overload1 = () => new ServiceCollection().AddIntermedium();
            Action overload2 = () => new ServiceCollection().AddIntermedium(x => x.Scan());

            overload1.Should().ThrowExactly<InvalidOperationException>();
            overload2.Should().ThrowExactly<InvalidOperationException>();
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void AddIntermedium_MediatorIsAlreadyRegistered_ThrowsInvalidOperationException()
        {
            new ServiceCollection()
                .AddTransient<IMediator, Mediator>()
                .AddIntermedium(typeof(IntermediumServiceCollectionExtensionsTests));
        }
    }
}
