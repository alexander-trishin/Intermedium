using System.Threading;
using System.Threading.Tasks;

namespace Intermedium
{
    /// <summary>
    /// Defines the handler for a <typeparamref name="TNotification"/>.
    /// </summary>
    /// <typeparam name="TNotification">The type of a notification.</typeparam>
    public interface INotificationHandler<in TNotification>
        where TNotification : INotification
    {
        /// <summary>
        /// Handles an <typeparamref name="TNotification"/>.
        /// </summary>
        /// <param name="notification">The notification sent to <see cref="IMediator"/>.</param>
        /// <param name="cancellationToken">
        /// A cancellation token that should be used to cancel the work.
        /// </param>
        /// <returns>A task that represents the way of notification handling.</returns>
        Task HandleAsync(TNotification notification, CancellationToken cancellationToken);
    }
}
