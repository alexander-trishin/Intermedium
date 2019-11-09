using System.Threading;
using System.Threading.Tasks;
using Intermedium.Compatibility;

namespace Intermedium
{
    /// <summary>
    /// Serves as the base class for classes that
    /// synchronously handles a <typeparamref name="TNotification"/>.
    /// </summary>
    /// <typeparam name="TNotification">The type of a notification.</typeparam>
    public abstract class SyncNotificationHandler<TNotification> : INotificationHandler<TNotification>
        where TNotification : INotification
    {
        Task INotificationHandler<TNotification>.HandleAsync(
            TNotification notification,
            CancellationToken cancellationToken)
        {
            Handle(notification, cancellationToken);
            return TaskBridge.CompletedTask;
        }

        /// <summary>
        /// Handles a <typeparamref name="TNotification"/>.
        /// </summary>
        /// <param name="notification">The notification sent to <see cref="IMediator"/>.</param>
        /// <param name="cancellationToken">
        /// A cancellation token that should be used to cancel the work.
        /// </param>
        protected abstract void Handle(TNotification notification, CancellationToken cancellationToken);
    }
}
