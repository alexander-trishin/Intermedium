using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Intermedium.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Intermedium.Tests
{
    [TestClass]
    public class MediatorTests
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_ServiceProviderIsNull_ThrowsArgumentNullException()
        {
            new Mediator(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task PublishAsync_NotificationIsNull_ThrowsArgumentNullException()
        {
            await new Mediator(x => null).PublishAsync((News)null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task PublishAsync_PublishStrategyIsNull_ThrowsArgumentNullException()
        {
            await new Mediator(x => null).PublishAsync(new News(), null, default);
        }

        [TestMethod]
        [ExpectedException(typeof(OperationCanceledException))]
        public async Task PublishAsync_Cancellation_ThrowsOperationCanceledException()
        {
            using var source = new CancellationTokenSource();
            source.Cancel();

            await new Mediator(x => null).PublishAsync(new News(), cancellationToken: source.Token);
        }

        [TestMethod]
        public async Task PublishAsync_WhenAllStrategy_AllHandlersCompleted()
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
                    : null;
            });

            await new Mediator(provider).PublishAsync(new News());

            handlers.All(x => x.Completed).Should().BeTrue();
        }

        [TestMethod]
        public async Task PublishAsync_WhenAnyStrategy_OnlyTheFastestHandlerCompleted()
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
                    : null;
            });

            await new Mediator(provider).PublishAsync(
                new News(),
                PublishStrategyProvider.ParallelWhenAny
            );

            fastHandler.Completed.Should().BeTrue();
            slowHandler.Completed.Should().BeFalse();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task SendAsync_RequestIsNull_ThrowsArgumentNullException()
        {
            await new Mediator(x => null).SendAsync((Ping)null);
        }

        [TestMethod]
        [ExpectedException(typeof(OperationCanceledException))]
        public async Task SendAsync_Cancellation_ThrowsOperationCanceledException()
        {
            using var source = new CancellationTokenSource();
            source.Cancel();

            await new Mediator(x => null).SendAsync(new Ping(), source.Token);
        }

        [TestMethod]
        public async Task SendAsync_CorrectRequest_ReturnsReponse()
        {
            var provider = new ServiceProvider(serviceType =>
            {
                return serviceType == typeof(IQueryHandler<Ping, Pong>)
                    ? new SyncPingHandler()
                    : null;
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
