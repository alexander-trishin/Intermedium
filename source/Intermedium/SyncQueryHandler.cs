using System.Threading;
using System.Threading.Tasks;

namespace Intermedium
{
    /// <summary>
    /// Serves as the base class for classes that synchronously handles a <typeparamref name="TQuery"/>
    /// and produces the <typeparamref name="TResult"/>.
    /// </summary>
    /// <typeparam name="TQuery">The type of a query.</typeparam>
    /// <typeparam name="TResult">
    /// The type of the return value of a <typeparamref name="TQuery"/>.
    /// </typeparam>
    public abstract class SyncQueryHandler<TQuery, TResult> : IQueryHandler<TQuery, TResult>
        where TQuery : IQuery<TResult>
    {
        Task<TResult> IQueryHandler<TQuery, TResult>.HandleAsync(
            TQuery query,
            CancellationToken cancellationToken)
        {
            return Task.FromResult(Handle(query, cancellationToken));
        }

        /// <summary>
        /// Handles a <typeparamref name="TQuery"/> and produces the <typeparamref name="TResult"/>.
        /// </summary>
        /// <param name="query">A query sent to <see cref="IMediator"/>.</param>
        /// <param name="cancellationToken">
        /// A cancellation token that should be used to cancel the work.
        /// </param>
        /// <returns>A return value of <typeparamref name="TQuery"/>.</returns>
        protected abstract TResult Handle(TQuery query, CancellationToken cancellationToken);
    }
}
