using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Intermedium.Core;
using Xunit;

namespace Intermedium.Tests
{
    public class MediatorTests
    {
        [Fact]
        public void Constructor_ShouldThrowArgumentNullException_WhenServiceProviderParameterIsNull()
        {
            Action act = () => new Mediator(null!);

            act.Should()
                .ThrowExactly<ArgumentNullException>()
                .Where(x => x.Message.Contains("serviceProvider"));
        }

        [Fact]
        public void PublishAsync_ShouldThrowArgumentNullException_WhenNotificationIsNull()
        {
            var mediator = new Mediator(x => null!);

            Func<Task> act = async () => await mediator.PublishAsync((News)null!);

            act.Should()
                .ThrowExactly<ArgumentNullException>()
                .Where(x => x.Message.Contains("notification"));
        }

        [Fact]
        public void PublishAsync_ShouldThrowArgumentNullException_WhenPublishStrategyIsNull()
        {
            var mediator = new Mediator(x => null!);

            Func<Task> act = async () => await mediator.PublishAsync(new News(), null!, default);

            act.Should()
                .ThrowExactly<ArgumentNullException>()
                .Where(x => x.Message.Contains("publishStrategy"));
        }

        [Fact]
        public async Task PublishAsync_Cancellation_ThrowsOperationCanceledException()
        {
            var mediator = new Mediator(x => null!);

            using var source = new CancellationTokenSource();
            source.Cancel();

            Func<Task> act = async () => await mediator.PublishAsync(new News(), cancellationToken: source.Token);

            await act.Should().ThrowExactlyAsync<OperationCanceledException>();
        }

        [Fact]
        public async Task PublishAsync_ShouldMarkAllHandlersAsCompleted_WhenUsesAllStrategy()
        {
            var handlers = new InterestedInNews[]
            {
                new FastTvWatcher(),
                new SlowRadioListener()
            };

            var provider = new ServiceProvider(serviceType =>
            {
                return serviceType == typeof(IEnumerable<INotificationHandler<News>>)
                    ? handlers
                    : null!;
            });

            await new Mediator(provider).PublishAsync(new News());

            handlers.All(x => x.Completed).Should().BeTrue();
        }

        [Fact]
        public async Task PublishAsync_ShouldMarkOnlyTheFastestHandlerAsCompleted_WhenUsesAnyStrategy()
        {
            var fastHandler = new FastTvWatcher();
            var slowHandler = new SlowRadioListener();

            var provider = new ServiceProvider(serviceType =>
            {
                return serviceType == typeof(IEnumerable<INotificationHandler<News>>)
                    ? new INotificationHandler<News>[]
                        {
                            fastHandler,
                            slowHandler
                        }
                    : null!;
            });

            await new Mediator(provider).PublishAsync(
                new News(),
                PublishStrategyProvider.ParallelWhenAny
            );

            fastHandler.Completed.Should().BeTrue();
            slowHandler.Completed.Should().BeFalse();
        }

        [Fact]
        public void SendAsync_ShouldThrowArgumentNullException_WhenRequestIsNull()
        {
            var mediator = new Mediator(x => null!);

            Func<Task> act = async () => await mediator.SendAsync((Ping)null!);

            act.Should()
                .ThrowExactly<ArgumentNullException>()
                .Where(x => x.Message.Contains("request"));
        }

        [Fact]
        public async Task SendAsync_Cancellation_ThrowsOperationCanceledException()
        {
            var mediator = new Mediator(x => null!);

            using var source = new CancellationTokenSource();
            source.Cancel();

            Func<Task> act = async () => await mediator.SendAsync(new Ping(), source.Token);

            await act.Should().ThrowExactlyAsync<OperationCanceledException>();
        }

        [Fact]
        public async Task SendAsync_ShouldReturnReponse_WhenRequestIsCorrect()
        {
            var provider = new ServiceProvider(serviceType =>
            {
                return serviceType == typeof(IQueryHandler<Ping, Pong>)
                    ? new SyncPingHandler()
                    : null!;
            });

            var request = new Ping();
            var actual = await new Mediator(provider).SendAsync(request);

            actual.Should().NotBeNull();
            actual.RequestId.Should().Be(request.Id);
        }

        private sealed class News : INotification
        {
        }

        private abstract class InterestedInNews : INotificationHandler<News>
        {
            private readonly TimeSpan _activeTime;

            public bool Completed { get; set; } = false;

            public InterestedInNews(TimeSpan activeTime)
            {
                _activeTime = activeTime;
            }

            public async Task HandleAsync(News notification, CancellationToken cancellationToken)
            {
                await Task.Delay(_activeTime);

                Completed = true;
            }
        }

        private sealed class FastTvWatcher : InterestedInNews
        {
            public FastTvWatcher()
                : base(TimeSpan.FromMilliseconds(5))
            {
            }
        }

        private sealed class SlowRadioListener : InterestedInNews
        {
            public SlowRadioListener()
                : base(TimeSpan.FromMilliseconds(55))
            {
            }
        }

        private sealed class Ping : IQuery<Pong>
        {
            public Guid Id { get; } = Guid.NewGuid();
        }

        private sealed class Pong
        {
            public Guid RequestId { get; set; }
        }

        private sealed class SyncPingHandler : IQueryHandler<Ping, Pong>
        {
            public Task<Pong> HandleAsync(Ping query, CancellationToken cancellationToken)
            {
                return Task.FromResult(new Pong { RequestId = query.Id });
            }
        }
    }
}
