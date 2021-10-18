using FluentAssertions;
using Intermedium.Extensions.Microsoft.DependencyInjection.Tests.Components;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Intermedium.Extensions.Microsoft.DependencyInjection.Tests
{
    public class IntermediumOptionsTests
    {
        [Fact]
        public void Use_WhouldRegisterMediatorImplementation_WhenCustomImplementationProvided()
        {
            var options = new IntermediumOptions();

            options.Use<MediatorTest>(ServiceLifetime.Singleton);

            options.MediatorImplementationType.Should().Be(typeof(MediatorTest));
            options.MediatorLifetime.Should().Be(ServiceLifetime.Singleton);
        }

        [Fact]
        public void Scan_ShouldScanOnlyUniqueAssemblies_WhenSameAssemblyProvidedMultipleTimes()
        {
            var type = typeof(IntermediumOptionsTests);
            var options = new IntermediumOptions();

            options.Scan(type, type, type);

            options.ScanTargets.Should().HaveCount(1);
        }

        [Fact]
        public void WithCoreMiddleware_ShouldRegisterAllCoreMiddleware_WhenMethodCalled()
        {
            var options = new IntermediumOptions();

            options.WithCoreMiddleware();

            options.RegisterExceptionHandlingMiddleware.Should().BeTrue();
            options.RegisterPreProcessingMiddleware.Should().BeTrue();
            options.RegisterPostProcessingMiddleware.Should().BeTrue();
        }

        [Fact]
        public void WithExceptionHandling_ShouldRegisterOnlyExceptionHandlingMiddleware_WhenEnabled()
        {
            var options = new IntermediumOptions();

            options.WithExceptionHandling();

            options.RegisterExceptionHandlingMiddleware.Should().BeTrue();
            options.RegisterPreProcessingMiddleware.Should().BeFalse();
            options.RegisterPostProcessingMiddleware.Should().BeFalse();
        }

        [Fact]
        public void WithPreProcessing_ShouldRegisterOnlyPreProcessingMiddleware_WhenEnabled()
        {
            var options = new IntermediumOptions();

            options.WithPreProcessing();

            options.RegisterExceptionHandlingMiddleware.Should().BeFalse();
            options.RegisterPreProcessingMiddleware.Should().BeTrue();
            options.RegisterPostProcessingMiddleware.Should().BeFalse();
        }

        [Fact]
        public void WithPostProcessing_ShouldRegisterOnlyPostProcessingMiddleware_WhenEnabled()
        {
            var options = new IntermediumOptions();

            options.WithPostProcessing();

            options.RegisterExceptionHandlingMiddleware.Should().BeFalse();
            options.RegisterPreProcessingMiddleware.Should().BeFalse();
            options.RegisterPostProcessingMiddleware.Should().BeTrue();
        }
    }
}
