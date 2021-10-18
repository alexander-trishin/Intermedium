using System.Threading;
using System.Threading.Tasks;

namespace Intermedium
{
    /// <summary>
    /// Publish a notification or event through the mediator pipeline to be handled by multiple handlers.
    /// </summary>
    public interface IMediatorPublisher
    {

        /// <summary>
        /// Asynchronously sends a <typeparamref name="TNotification"/> to multiple handlers.
        /// </summary>
        /// <typeparam name="TNotification">The type of a notification.</typeparam>
        /// <param name="notification">The notification sent to <see cref="IMediatorPublisher"/>.</param>
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
        /// <param name="notification">The notification sent to <see cref="IMediatorPublisher"/>.</param>
        /// <param name="publishStrategy">
        /// The way <see cref="IMediatorPublisher"/> uses the list of
        /// <see cref="NotificationHandlerMethod{TNotification}"/>.
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
