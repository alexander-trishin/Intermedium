using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Intermedium
{
    /// <summary>
    /// Represents the method that will handle the way <see cref="IMediator"/>
    /// uses the list of <see cref="NotificationHandler{TNotification}"/>.
    /// </summary>
    /// <typeparam name="TNotification">The type of notification.</typeparam>
    /// <param name="notification">The notification sent to <see cref="IMediator"/>.</param>
    /// <param name="handlers">The list of <see cref="NotificationHandler{TNotification}"/>.</param>
    /// <param name="cancellationToken">
    /// A cancellation token that should be used to cancel the work.
    /// </param>
    /// <returns>A task that represents the publish strategy.</returns>
    public delegate Task PublishStrategy<TNotification>(
        TNotification notification,
        IEnumerable<NotificationHandler<TNotification>> handlers,
        CancellationToken cancellationToken
    )
        where TNotification : INotification;
}
