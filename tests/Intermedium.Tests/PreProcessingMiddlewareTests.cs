using System;
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
    public class PreProcessingMiddlewareTests
    {
        [TestMethod]
        public async Task ExecuteAsync_ChangeInput_ReturnsModifiedOutput()
        {
            var middleware = new PreProcessingMiddleware<Calculate, int>(
                new[]
                {
                    new DoubleMultiplier(),
                    new DoubleMultiplier()
                },
                null
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

        [TestMethod]
        public async Task ExecuteAsync_UseSortingAndChangeInput_ReturnsModifiedOutput()
        {
            var middleware = new PreProcessingMiddleware<Calculate, int>(
                new IQueryPreProcessor<Calculate, int>[]
                {
                    new DoubleMultiplier(),
                    new AddTenToMultiplier(),
                    new DoubleMultiplier(),
                    new AddTenToMultiplier(),
                    new AddTenToMultiplier()
                },
                new VeryTrickyComparer()
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

        private sealed class SyncCalculateHandler : SyncQueryHandler<Calculate, int>
        {
            protected override int Handle(Calculate request, CancellationToken cancellationToken)
            {
                return request.Value * request.Multiplier;
            }
        }

        private sealed class DoubleMultiplier : IQueryPreProcessor<Calculate, int>
        {
            public Task ProcessAsync(Calculate request, CancellationToken cancellationToken)
            {
                request.Multiplier *= 2;
                return TaskBridge.CompletedTask;
            }
        }

        private sealed class AddTenToMultiplier : IQueryPreProcessor<Calculate, int>
        {
            public Task ProcessAsync(Calculate request, CancellationToken cancellationToken)
            {
                request.Multiplier += 10;
                return TaskBridge.CompletedTask;
            }
        }

        private sealed class VeryTrickyComparer : IComparer<IQueryPreProcessor<Calculate, int>>
        {
            private readonly List<Type> _order = new List<Type>
            {
                typeof(AddTenToMultiplier),
                typeof(DoubleMultiplier)
            };

            public int Compare(IQueryPreProcessor<Calculate, int> x, IQueryPreProcessor<Calculate, int> y)
            {
                var xIndex = _order.IndexOf(x.GetType());
                var yIndex = _order.IndexOf(y.GetType());

                return xIndex.CompareTo(yIndex);
            }
        }
    }
}
