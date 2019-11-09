using System.Collections.Generic;
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
    public class PostProcessingMiddlewareTests
    {
        [TestMethod]
        public async Task ExecuteAsync_ChangeOutput_ReturnsModifiedOutput()
        {
            var middleware = new PostProcessingMiddleware<Calculate, int>(
                new[]
                {
                    new UseConstantValue()
                },
                null
            );

            var handler = (IQueryHandler<Calculate, int>)new SyncCalculateHandler();
            var query = new Calculate();

            var result = await middleware.ExecuteAsync(
                new Calculate(),
                () => handler.HandleAsync(query, default),
                default
            );

            result.Should().Be(UseConstantValue.Value);
        }

        [TestMethod]
        public async Task ExecuteAsync_UseSortingAndChangeOutput_ReturnsModifiedOutput()
        {
            var middleware = new PostProcessingMiddleware<Calculate, int>(
                new IQueryPostProcessor<Calculate, int>[]
                {
                    new DoubleOutput(),
                    new DoubleOutput(),
                    new UseConstantValue()
                },
                new OrderComparer<Calculate, int>()
            );

            var handler = (IQueryHandler<Calculate, int>)new SyncCalculateHandler();
            var query = new Calculate();

            var result = await middleware.ExecuteAsync(
                query,
                () => handler.HandleAsync(query, default),
                default
            );

            result.Should().Be(UseConstantValue.Value * 4);
        }

        private sealed class Calculate : IQuery<int>
        {
            public int Value { get; set; }
            public int Multiplier { get; set; }
        }

        private sealed class SyncCalculateHandler : SyncQueryHandler<Calculate, int>
        {
            protected override int Handle(Calculate request, CancellationToken cancellationToken)
            {
                return request.Value * request.Multiplier;
            }
        }

        private interface IOrderedPostProcessor
        {
            int Order { get; }
        }

        private sealed class UseConstantValue : IQueryPostProcessor<Calculate, int>, IOrderedPostProcessor
        {
            public int Order { get; } = 1;

            public static int Value { get; } = 13;

            public Task ProcessAsync(
                Calculate request,
                IPostProcessorContext<int> context,
                CancellationToken cancellationToken)
            {
                context.Result = Value;
                return TaskBridge.CompletedTask;
            }
        }

        private sealed class DoubleOutput : IQueryPostProcessor<Calculate, int>, IOrderedPostProcessor
        {
            public int Order { get; } = 2;

            public Task ProcessAsync(
                Calculate request,
                IPostProcessorContext<int> context,
                CancellationToken cancellationToken)
            {
                context.Result *= 2;
                return TaskBridge.CompletedTask;
            }
        }

        private sealed class OrderComparer<TRequest, TResponse>
            : IComparer<IQueryPostProcessor<TRequest, TResponse>>, IComparer<IOrderedPostProcessor>

            where TRequest : IQuery<TResponse>
        {
            public int Compare(
                IQueryPostProcessor<TRequest, TResponse> x,
                IQueryPostProcessor<TRequest, TResponse> y)
            {
                return Compare(x as IOrderedPostProcessor, y as IOrderedPostProcessor);
            }

            public int Compare(IOrderedPostProcessor x, IOrderedPostProcessor y)
            {
                return (x?.Order ?? default).CompareTo(y?.Order ?? default);
            }
        }
    }
}
