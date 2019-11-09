using System.Threading;
using System.Threading.Tasks;

namespace Intermedium
{
    /// <summary>
    /// Defines the handler for a <typeparamref name="TQuery"/>
    /// which produces the <typeparamref name="TResult"/>.
    /// </summary>
    /// <typeparam name="TQuery">The type of a query.</typeparam>
    /// <typeparam name="TResult">
    /// The type of the return value of a <typeparamref name="TQuery"/>.
    /// </typeparam>
    public interface IQueryHandler<in TQuery, TResult>
        where TQuery : IQuery<TResult>
    {
        /// <summary>
        /// Handles a request.
        /// </summary>
        /// <param name="request">The request to <see cref="IMediator"/>.</param>
        /// <param name="cancellationToken">
        /// A cancellation token that should be used to cancel the work.
        /// </param>
        /// <returns>A task that represents the way of request handling.</returns>
        Task<TResult> HandleAsync(TQuery request, CancellationToken cancellationToken);
    }
}
