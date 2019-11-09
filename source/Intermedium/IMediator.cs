using System.Threading;
using System.Threading.Tasks;

namespace Intermedium
{
    /// <summary>
    /// Defines a mechanism which encapsulates how a set of objects interact with each other.
    /// </summary>
    public interface IMediator
    {
        /// <summary>
        /// Asynchronously sends a request to the single handler.
        /// </summary>
        /// <typeparam name="TResult">
        /// The type of the return value of a <see cref="IQuery{TResult}"/>.
        /// </typeparam>
        /// <param name="request">The request to <see cref="IMediator"/>.</param>
        /// <param name="cancellationToken">
        /// A cancellation token that should be used to cancel the work.
        /// </param>
        /// <returns>A task that represents the send operation.</returns>
        Task<TResult> SendAsync<TResult>(
            IQuery<TResult> request,
            CancellationToken cancellationToken = default
        );

        /// <summary>
        /// Asynchronously sends a <typeparamref name="TNotification"/> to multiple handlers.
        /// </summary>
        /// <typeparam name="TNotification">The type of a notification.</typeparam>
        /// <param name="notification">The notification sent to <see cref="IMediator"/>.</param>
        /// <param name="cancellationToken">
        /// A cancellation token that should be used to cancel the work.
        /// </param>
        /// <returns>A task that represents the publish operation.</returns>
        Task PublishAsync<TNotification>(
            TNotification notification,
            CancellationToken cancellationToken = default
        )
            where TNotification : INotification;

        /// <summary>
        /// Asynchronously sends a <typeparamref name="TNotification"/> to multiple handlers.
        /// </summary>
        /// <typeparam name="TNotification">The type of a notification.</typeparam>
        /// <param name="notification">The notification sent to <see cref="IMediator"/>.</param>
        /// <param name="publishStrategy">
        /// The way <see cref="IMediator"/> uses the list of
        /// <see cref="NotificationHandler{TNotification}"/>.
        /// </param>
        /// <param name="cancellationToken">
        /// A cancellation token that should be used to cancel the work.
        /// </param>
        /// <returns>A task that represents the publish operation.</returns>
        Task PublishAsync<TNotification>(
            TNotification notification,
            IPublishStrategyProvider publishStrategy,
            CancellationToken cancellationToken = default
        )
            where TNotification : INotification;
    }
}
