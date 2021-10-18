using System.Threading;
using System.Threading.Tasks;

namespace Intermedium
{
    /// <summary>
    /// Represents the method that will handle a <typeparamref name="TNotification"/>.
    /// </summary>
    /// <typeparam name="TNotification">The type of a notification.</typeparam>
    /// <param name="notification">The notification sent to <see cref="IMediatorPublisher"/>.</param>
    /// <param name="cancellationToken">
    /// A cancellation token that should be used to cancel the work.
    /// </param>
    /// <returns>A task that represents the way of notification handling.</returns>
    public delegate Task NotificationHandlerMethod<TNotification>(
        TNotification notification,
        CancellationToken cancellationToken
    )
        where TNotification : INotification;
}
