using System.Threading;
using System.Threading.Tasks;

namespace Intermedium.Pipeline
{
    /// <summary>
    /// Defines a mechanism which encapsulates an action to perform
    /// before <typeparamref name="TQuery"/> was handled.
    /// </summary>
    /// <typeparam name="TQuery">The type of a query.</typeparam>
    /// <typeparam name="TResult">
    /// The type of the return value of a <typeparamref name="TQuery"/>.
    /// </typeparam>
    public interface IQueryPreProcessor<in TQuery, TResult>
        where TQuery : IQuery<TResult>
    {
        /// <summary>
        /// Executes an action before <typeparamref name="TQuery"/> was handled.
        /// </summary>
        /// <param name="request">The request to <see cref="IMediator"/>.</param>
        /// <param name="cancellationToken">
        /// A cancellation token that should be used to cancel the work.
        /// </param>
        /// <returns>A task that represents request pre-processing.</returns>
        Task ProcessAsync(TQuery request, CancellationToken cancellationToken);
    }
}
