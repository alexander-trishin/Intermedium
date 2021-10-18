using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Intermedium.Core;
using Intermedium.Core.Internal;
using Xunit;

namespace Intermedium.Tests
{
    public class NotificationHandlerWrapperTests
    {
        [Fact]
        public async Task HandleAsync_ShouldThrowOperationCanceledException_WhenCancellationRequested()
        {
            var wrapper = new NotificationHandlerWrapper<News>();
            var provider = new ServiceProvider(serviceType => null!);

            using var source = new CancellationTokenSource();
            source.Cancel();

            Func<Task> act = async () => await wrapper.HandleAsync(new News(), default!, provider, source.Token);

            await act.Should().ThrowExactlyAsync<OperationCanceledException>();
        }

        [Fact]
        public async Task HandleAsync_ShouldDoNothing_WhenNotificationHandlersNotRegistered()
        {
            var news = new News();
            var wrapper = new NotificationHandlerWrapper<News>();
            var provider = new ServiceProvider(serviceType => null!);

            await wrapper.HandleAsync(
                news,
                PublishStrategyProvider.Sequentially.Resolve<News>(),
                provider,
                default
            );

            news.ProgramWatchCount.Should().Be(0);
        }

        [Fact]
        public async Task HandleAsync_ShouldHandleNotification_WhenNotificationHandlerRegistered()
        {
            var news = new News();
            var wrapper = new NotificationHandlerWrapper<News>();
            var provider = new ServiceProvider(serviceType => new INotificationHandler<News>[]
            {
                new WatchNews(),
                new WatchNews5Milliseconds()
            });

            await wrapper.HandleAsync(
                news,
                PublishStrategyProvider.ParallelWhenAll.Resolve<News>(),
                provider,
                default
            );

            news.ProgramWatchCount.Should().Be(6);
        }

        private sealed class News : INotification
        {
            public int ProgramWatchCount { get; set; }
        }

        private sealed class WatchNews : NotificationHandler<News>
        {
            public override void Handle(News notification, CancellationToken cancellationToken)
            {
                notification.ProgramWatchCount++;
            }
        }

        private sealed class WatchNews5Milliseconds : INotificationHandler<News>
        {
            public async Task HandleAsync(News notification, CancellationToken cancellationToken)
            {
                for (var i = 0; i < 5; i++)
                {
                    await Task.Delay(TimeSpan.FromMilliseconds(1));
                    notification.ProgramWatchCount++;
                }
            }
        }
    }
}
