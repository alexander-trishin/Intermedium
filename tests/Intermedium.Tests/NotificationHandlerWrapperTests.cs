using System;
using System.Threading;
using System.Threading.Tasks;
using Intermedium.Core;
using Intermedium.Core.Internal;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Intermedium.Tests
{
    [TestClass]
    public class NotificationHandlerWrapperTests
    {
        [TestMethod]
        [ExpectedException(typeof(OperationCanceledException))]
        public async Task HandleAsync_CanceledToken_ThrowsOperationCanceledException()
        {
            var wrapper = new NotificationHandlerWrapper<News>();
            var provider = new ServiceProvider(serviceType => null);

            using var source = new CancellationTokenSource();
            source.Cancel();

            await wrapper.HandleAsync(new News(), default, provider, source.Token);
        }

        [TestMethod]
        public async Task HandleAsync_WithoutNotificationHandlers_DoesNothing()
        {
            var wrapper = new NotificationHandlerWrapper<News>();
            var provider = new ServiceProvider(serviceType => null);

            await wrapper.HandleAsync(
                new News(),
                PublishStrategyProvider.Sequentially.Resolve<News>(),
                provider,
                default
            );
        }

        [TestMethod]
        public async Task HandleAsync_RegisteredNotificationHandlers_HandlesNotification()
        {
            var wrapper = new NotificationHandlerWrapper<News>();
            var provider = new ServiceProvider(serviceType => new INotificationHandler<News>[]
            {
                new WatchNews(),
                new WatchNews5Milliseconds()
            });

            await wrapper.HandleAsync(
                new News(),
                PublishStrategyProvider.ParallelWhenAll.Resolve<News>(),
                provider,
                default
            );
        }

        private sealed class News : INotification
        {
        }

        private sealed class WatchNews : SyncNotificationHandler<News>
        {
            protected override void Handle(News notification, CancellationToken cancellationToken)
            {
            }
        }

        private sealed class WatchNews5Milliseconds : INotificationHandler<News>
        {
            public Task HandleAsync(News notification, CancellationToken cancellationToken)
            {
                return Task.Delay(TimeSpan.FromMilliseconds(5));
            }
        }
    }
}
