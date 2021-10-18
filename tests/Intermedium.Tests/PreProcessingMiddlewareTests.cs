using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Intermedium.Pipeline;
using Intermedium.Pipeline.Steps;
using Xunit;

namespace Intermedium.Tests
{
    public class PreProcessingMiddlewareTests
    {
        [Fact]
        public async Task ExecuteAsync_ShouldReturnModifiedOutput_WhenUsesChangeMiddleware()
        {
            var middleware = new PreProcessingMiddleware<Calculate, int>(
                new[]
                {
                    new DoubleMultiplier(),
                    new DoubleMultiplier()
                }
            );

            var query = new Calculate { Value = 5, Multiplier = 1 };
            var handler = (IQueryHandler<Calculate, int>)new SyncCalculateHandler();

            var actual = await middleware.ExecuteAsync(
                query,
                () => handler.HandleAsync(query, default),
                default
            );

            actual.Should().Be(20);
        }

        [Fact]
        public async Task ExecuteAsync_ShouldReturnModifiedOutput_WhenUsesChangeAndSortMiddleware()
        {
            var middleware = new PreProcessingMiddleware<Calculate, int>(
                new IQueryPreProcessor<Calculate, int>[]
                {
                    new DoubleMultiplier { Order = 3 },
                    new AddTenToMultiplier { Order = 0 },
                    new DoubleMultiplier { Order = 4 },
                    new AddTenToMultiplier { Order = 1 },
                    new AddTenToMultiplier { Order = 2 }
                }
            );

            var query = new Calculate { Value = 5, Multiplier = 1 };
            var handler = (IQueryHandler<Calculate, int>)new SyncCalculateHandler();

            var actual = await middleware.ExecuteAsync(
                query,
                () => handler.HandleAsync(query, default),
                default
            );

            actual.Should().Be(620);
        }

        private sealed class Calculate : IQuery<int>
        {
            public int Value { get; set; }
            public int Multiplier { get; set; }
        }

        private sealed class SyncCalculateHandler : QueryHandler<Calculate, int>
        {
            public override int Handle(Calculate request, CancellationToken cancellationToken)
            {
                return request.Value * request.Multiplier;
            }
        }

        private sealed class DoubleMultiplier : IQueryPreProcessor<Calculate, int>, IOrderedProcessor
        {
            public int Order { get; init; }

            public Task ProcessAsync(Calculate request, CancellationToken cancellationToken)
            {
                request.Multiplier *= 2;
                return Task.CompletedTask;
            }
        }

        private sealed class AddTenToMultiplier : IQueryPreProcessor<Calculate, int>, IOrderedProcessor
        {
            public int Order { get; init; }

            public Task ProcessAsync(Calculate request, CancellationToken cancellationToken)
            {
                request.Multiplier += 10;
                return Task.CompletedTask;
            }
        }
    }
}
