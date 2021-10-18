using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Intermedium.Core;
using Intermedium.Core.Internal;
using Xunit;

namespace Intermedium.Tests
{
    public class RequestHandlerWrapperTests
    {
        [Fact]
        public async Task HandleAsync_ShouldThrowOperationCanceledException_WhenCancellationRequested()
        {
            var wrapper = new RequestHandlerWrapper<Ping, Pong>();
            var provider = new ServiceProvider(serviceType => null!);

            using var source = new CancellationTokenSource();
            source.Cancel();

            Func<Task> act = async () => await wrapper.HandleAsync(new Ping(), provider, source.Token);

            await act.Should().ThrowExactlyAsync<OperationCanceledException>();
        }

        [Fact]
        public async Task HandleAsync_ShouldHandleCommand_WhenCommandHandlerRegistered()
        {
            var wrapper = new RequestHandlerWrapper<Idle, VoidUnit>();
            var provider = new ServiceProvider(serviceType =>
            {
                return serviceType == typeof(IQueryHandler<Idle, VoidUnit>)
                    ? new IdleHandler()
                    : null!;
            });

            var request = new Idle();
            var actual = await wrapper.HandleAsync(request, provider, default);

            actual.Should().Be(default);
        }

        [Fact]
        public async Task HandleAsync_ShouldReturnResponse_WhenQueryHandlerRegistered()
        {
            var wrapper = new RequestHandlerWrapper<Ping, Pong>();
            var provider = new ServiceProvider(serviceType =>
            {
                return serviceType == typeof(IQueryHandler<Ping, Pong>)
                    ? new SyncPingHandler()
                    : null!;
            });

            var request = new Ping();
            var actual = await wrapper.HandleAsync(request, provider, default);

            actual.Should().NotBeNull();
            actual.QueryId.Should().Be(request.Id);
        }

        [Fact]
        public async Task HandleAsync_ShouldThrowInvalidOperationException_WhenQueryHandlerNotRegistered()
        {
            var wrapper = new RequestHandlerWrapper<Ping, Pong>();
            var provider = new ServiceProvider(serviceType => null!);

            Func<Task> act = async () => await wrapper.HandleAsync(new Ping(), provider, default);

            await act.Should().ThrowExactlyAsync<InvalidOperationException>();
        }

        private sealed class Pong
        {
            public Guid QueryId { get; set; }
        }

        private sealed class Ping : IQuery<Pong>
        {
            public Guid Id { get; } = Guid.NewGuid();
        }

        private sealed class SyncPingHandler : QueryHandler<Ping, Pong>
        {
            public override Pong Handle(Ping query, CancellationToken cancellationToken)
            {
                return new Pong { QueryId = query.Id };
            }
        }

        private sealed class Idle : ICommand
        {
        }

        private sealed class IdleHandler : CommandHandler<Idle>
        {
            public override void Handle(Idle command, CancellationToken cancellationToken)
            {
            }
        }
    }
}
