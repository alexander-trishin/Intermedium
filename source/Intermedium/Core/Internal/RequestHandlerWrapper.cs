using System.Threading;
using System.Threading.Tasks;

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

            return serviceProvider
                .GetRequiredService<IQueryHandler<TRequest, TResponse>>()
                .HandleAsync((TRequest)request, cancellationToken);
        }
    }
}
