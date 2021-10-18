using System.Linq;
using FluentAssertions;
using Intermedium.Core;
using Intermedium.Extensions.Microsoft.DependencyInjection.Tests.Components;
using Intermedium.Internal;
using Intermedium.Pipeline;
using Intermedium.Pipeline.Steps;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Intermedium.Extensions.Microsoft.DependencyInjection.Tests
{
    public class ServiceRegistrarTests
    {
        [Fact]
        public void FindAndRegister_ShouldRegisterAllComponentsFromExecutingAssembly_WhenServicesWereRegistered()
        {
            var services = new ServiceCollection();
            var options = new IntermediumOptions().WithCoreMiddleware();

            options.Scan(typeof(ServiceRegistrarTests));
            ServiceRegistrar.FindAndRegister(services, options);

            var provider = services.BuildServiceProvider();

            provider.GetService<Core.ServiceProvider>().Should().NotBeNull();
            provider.GetService<IMediator>().Should().BeOfType<Mediator>();

            new[]
            {
                typeof(ExceptionHandlingMiddleware<,>),
                typeof(PostProcessingMiddleware<,>),
                typeof(PreProcessingMiddleware<,>)
            }
            .All(middleware => services.Any(x => x.ImplementationType == middleware))
            .Should()
            .BeTrue();

            provider.GetServices<IQueryHandler<QueryTest, int>>().Should().HaveCount(1);
            provider.GetServices<IQueryHandler<CommandTest, VoidUnit>>().Should().HaveCount(1);
            provider.GetServices<INotificationHandler<NotificationTest>>().Should().HaveCount(2);
            provider.GetServices<IQueryExceptionHandler<CommandTest, VoidUnit>>().Should().HaveCount(3);
        }
    }
}
