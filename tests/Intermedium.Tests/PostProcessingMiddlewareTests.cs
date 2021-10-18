using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Intermedium.Pipeline;
using Intermedium.Pipeline.Steps;
using Xunit;

namespace Intermedium.Tests
{
    public class PostProcessingMiddlewareTests
    {
        public async Task ExecuteAsync_ShouldReturnModifiedOutput_WhenUsesChangeMiddleware()
        {
            var middleware = new PostProcessingMiddleware<Calculate, int>(
                new[]
                {
                    new UseConstantValue()
                }
            );

            var handler = (IQueryHandler<Calculate, int>)new CalculateHandler();
            var query = new Calculate();

            var result = await middleware.ExecuteAsync(
                new Calculate(),
                () => handler.HandleAsync(query, default),
                default
            );

            result.Should().Be(UseConstantValue.Value);
        }

        [Fact]
        public async Task ExecuteAsync_ShouldReturnModifiedOutput_WhenUsesSortAndChangeMiddleware()
        {
            var middleware = new PostProcessingMiddleware<Calculate, int>(
                new IQueryPostProcessor<Calculate, int>[]
                {
                    new DoubleOutput(),
                    new DoubleOutput(),
                    new UseConstantValue()
                }
            );

            var handler = (IQueryHandler<Calculate, int>)new CalculateHandler();
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

        private sealed class CalculateHandler : QueryHandler<Calculate, int>
        {
            public override int Handle(Calculate request, CancellationToken cancellationToken)
            {
                return request.Value * request.Multiplier;
            }
        }

        private sealed class UseConstantValue : IQueryPostProcessor<Calculate, int>, IOrderedProcessor
        {
            public int Order { get; init; } = 1;

            public static int Value { get; } = 13;

            public Task ProcessAsync(
                Calculate request,
                IPostProcessorContext<int> context,
                CancellationToken cancellationToken)
            {
                context.Result = Value;
                return Task.CompletedTask;
            }
        }

        private sealed class DoubleOutput : IQueryPostProcessor<Calculate, int>, IOrderedProcessor
        {
            public int Order { get; init; } = 2;

            public Task ProcessAsync(
                Calculate request,
                IPostProcessorContext<int> context,
                CancellationToken cancellationToken)
            {
                context.Result *= 2;
                return Task.CompletedTask;
            }
        }
    }
}
