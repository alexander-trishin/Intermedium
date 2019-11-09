using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Intermedium.Compatibility;
using Intermedium.Pipeline;
using Intermedium.Pipeline.Steps;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Intermedium.Tests
{
    [TestClass]
    public class ExceptionHandlingMiddlewareTests
    {
        [TestMethod]
        public async Task ExecuteAsync_ConcreteException_HandlesException()
        {
            var exceptionHandler = new SyncPingConcreteExceptionHandler();

            var middleware = new ExceptionHandlingMiddleware<Ping, int>(new[] { exceptionHandler });

            var query = new Ping();
            var handler = new BrokenPingHandler();

            var actual = await middleware.ExecuteAsync(
                query,
                () => handler.HandleAsync(query, default),
                default
            );

            actual.Should().Be(exceptionHandler.Value);
        }

        [TestMethod]
        public async Task ExecuteAsync_CommonException_HandlesException()
        {
            var exceptionHandler = new SyncPingCommonExceptionHandler();

            var middleware = new ExceptionHandlingMiddleware<Ping, int>(new[] { exceptionHandler });

            var query = new Ping();
            var handler = new BrokenPingHandler();

            var actual = await middleware.ExecuteAsync(
                query,
                () => handler.HandleAsync(query, default),
                default
            );

            actual.Should().Be(exceptionHandler.Value);
        }

        [TestMethod]
        public async Task ExecuteAsync_MultipleExceptionHandlers_UsesConcreteHandler()
        {
            var concreteExceptionHandler = new SyncPingConcreteExceptionHandler();

            var middleware = new ExceptionHandlingMiddleware<Ping, int>(
                new IQueryExceptionHandler<Ping, int>[]
                {
                    new SyncPingCommonExceptionHandler(),
                    concreteExceptionHandler
                }
            );

            var query = new Ping();
            var handler = new BrokenPingHandler();

            var actual = await middleware.ExecuteAsync(
                query,
                () => handler.HandleAsync(query, default),
                default
            );

            actual.Should().Be(concreteExceptionHandler.Value);
        }

        private sealed class Ping : IQuery<int>
        {
        }

        private sealed class BrokenPingHandler : IQueryHandler<Ping, int>
        {
            public Task<int> HandleAsync(Ping request, CancellationToken cancellationToken)
            {
                throw new NotImplementedException();
            }
        }

        private sealed class SyncPingConcreteExceptionHandler
            : IQueryExceptionHandler<Ping, int, NotImplementedException>
        {
            public int Value { get; } = 50;

            public Task HandleAsync(
                Ping request,
                Exception exception,
                IExceptionHandlerContext<int> context,
                CancellationToken cancellationToken)
            {
                context.ExceptionWasHandled(Value);
                return TaskBridge.CompletedTask;
            }
        }

        private sealed class SyncPingCommonExceptionHandler
            : IQueryExceptionHandler<Ping, int>
        {
            public int Value { get; } = 10;

            public Task HandleAsync(
                Ping request,
                Exception exception,
                IExceptionHandlerContext<int> context,
                CancellationToken cancellationToken)
            {
                context.ExceptionWasHandled(Value);
                return TaskBridge.CompletedTask;
            }
        }
    }
}
