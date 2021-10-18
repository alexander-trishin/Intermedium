using System.Threading;
using System.Threading.Tasks;

namespace Intermedium
{
    /// <summary>
    /// Send a request through the mediator pipeline to be handled by a single handler.
    /// </summary>
    public interface IMediatorSender
    {
        /// <summary>
        /// Asynchronously sends a request to the single handler.
        /// </summary>
        /// <typeparam name="TResult">
        /// The type of the return value of a <see cref="IQuery{TResult}"/>.
        /// </typeparam>
        /// <param name="request">The request to <see cref="IMediatorSender"/>.</param>
        /// <param name="cancellationToken">
        /// A cancellation token that should be used to cancel the work.
        /// </param>
        /// <returns>A task that represents the send operation.</returns>
        Task<TResult> SendAsync<TResult>(
            IQuery<TResult> request,
            CancellationToken cancellationToken = default
        );
    }
}
