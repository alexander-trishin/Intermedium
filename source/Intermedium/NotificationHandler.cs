using System.Threading;
using System.Threading.Tasks;

namespace Intermedium
{
    /// <summary>
    /// Serves as the base class for classes that
    /// synchronously handles a <typeparamref name="TNotification"/>.
    /// </summary>
    /// <typeparam name="TNotification">The type of a notification.</typeparam>
    public abstract class NotificationHandler<TNotification> : INotificationHandler<TNotification>
        where TNotification : INotification
    {
        Task INotificationHandler<TNotification>.HandleAsync(
            TNotification notification,
            CancellationToken cancellationToken)
        {
            Handle(notification, cancellationToken);
            return Task.CompletedTask;
        }

        /// <summary>
        /// Handles a <typeparamref name="TNotification"/>.
        /// </summary>
        /// <param name="notification">The notification sent to <see cref="IMediatorPublisher"/>.</param>
        /// <param name="cancellationToken">
        /// A cancellation token that should be used to cancel the work.
        /// </param>
        public abstract void Handle(TNotification notification, CancellationToken cancellationToken);
    }
}
