using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Intermedium.Core;
using Intermedium.Core.Internal;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Intermedium.Tests
{
    [TestClass]
    public class RequestHandlerWrapperTests
    {
        [TestMethod]
        [ExpectedException(typeof(OperationCanceledException))]
        public async Task HandleAsync_CanceledToken_ThrowsOperationCanceledException()
        {
            var wrapper = new RequestHandlerWrapper<Ping, Pong>();
            var provider = new ServiceProvider(serviceType => null);

            using var source = new CancellationTokenSource();
            source.Cancel();

            await wrapper.HandleAsync(new Ping(), provider, source.Token);
        }

        [TestMethod]
        public async Task HandleAsync_CommandAndRegisteredCommandHandler_HandlesTheCommand()
        {
            var wrapper = new RequestHandlerWrapper<Idle, VoidUnit>();
            var provider = new ServiceProvider(serviceType =>
            {
                return serviceType == typeof(IQueryHandler<Idle, VoidUnit>)
                    ? new SyncIdleHandler()
                    : null;
            });

            var request = new Idle();
            var actual = await wrapper.HandleAsync(request, provider, default);

            actual.Should().Be(default);
        }

        [TestMethod]
        public async Task HandleAsync_QueryAndRegisteredQueryHandler_ReturnsResponse()
        {
            var wrapper = new RequestHandlerWrapper<Ping, Pong>();
            var provider = new ServiceProvider(serviceType =>
            {
                return serviceType == typeof(IQueryHandler<Ping, Pong>)
                    ? new SyncPingHandler()
                    : null;
            });

            var request = new Ping();
            var actual = await wrapper.HandleAsync(request, provider, default);

            actual.Should().NotBeNull();
            actual.QueryId.Should().Be(request.Id);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public async Task HandleAsync_QueryAndNotRegisteredHandler_ThrowsInvalidOperationException()
        {
            var wrapper = new RequestHandlerWrapper<Ping, Pong>();
            var provider = new ServiceProvider(serviceType => null);

            await wrapper.HandleAsync(new Ping(), provider, default);
        }

        private sealed class Pong
        {
            public Guid QueryId { get; set; }
        }

        private sealed class Ping : IQuery<Pong>
        {
            public Guid Id { get; } = Guid.NewGuid();
        }

        private sealed class SyncPingHandler : SyncQueryHandler<Ping, Pong>
        {
            protected override Pong Handle(Ping query, CancellationToken cancellationToken)
            {
                return new Pong { QueryId = query.Id };
            }
        }

        private sealed class Idle : ICommand
        {
        }

        private sealed class SyncIdleHandler : SyncCommandHandler<Idle>
        {
            protected override void Handle(Idle command, CancellationToken cancellationToken)
            {
            }
        }
    }
}
