using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Intermedium.Pipeline;

namespace Intermedium.Core.Internal
{
    internal abstract class RequestHandlerWrapper<TResponse>
    {
        public abstract Task<TResponse> HandleAsync(
            IQuery<TResponse> request,
            ServiceProvider serviceProvider,
            CancellationToken cancellationToken
        );
    }

    internal sealed class RequestHandlerWrapper<TRequest, TResponse> : RequestHandlerWrapper<TResponse>
        where TRequest : IQuery<TResponse>
    {
        public sealed override Task<TResponse> HandleAsync(
            IQuery<TResponse> request,
            ServiceProvider serviceProvider,
            CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var pipeline = serviceProvider
                .GetServices<IMiddleware<TRequest, TResponse>>()
                .EmptyIfNull();

            var comparer = serviceProvider.GetService<IComparer<IMiddleware<TRequest, TResponse>>>();

            pipeline = comparer is null ? pipeline.Reverse() : pipeline.Sort(comparer);

            return pipeline.Aggregate(
                new Func<Task<TResponse>>(() => serviceProvider
                    .GetRequiredService<IQueryHandler<TRequest, TResponse>>()
                    .HandleAsync((TRequest)request, cancellationToken)
                ),
                (next, middleware) => () => middleware.ExecuteAsync((TRequest)request, next, cancellationToken)
            )();
        }
    }
}
