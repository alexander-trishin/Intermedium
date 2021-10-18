using Intermedium.Core;

namespace Intermedium.Extensions.Microsoft.DependencyInjection.Tests.Components
{
    internal sealed class MediatorTest : Mediator
    {
        public MediatorTest(
            ServiceProvider serviceProvider,
            IPublishStrategyProvider? defaultPublishStrategy = null
        )
            : base(serviceProvider, defaultPublishStrategy)
        {
        }
    }
}
