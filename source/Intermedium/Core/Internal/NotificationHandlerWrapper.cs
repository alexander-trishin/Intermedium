using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Intermedium.Core.Internal
{
    internal sealed class NotificationHandlerWrapper<TNotification>
        where TNotification : INotification
    {
        public Task HandleAsync(
            TNotification notification,
            PublishStrategy<TNotification> publishStrategy,
            ServiceProvider serviceProvider,
            CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var services = serviceProvider
                .GetServices<INotificationHandler<TNotification>>()
                ?? Enumerable.Empty<INotificationHandler<TNotification>>();

            var handlers = services.Select(service => new NotificationHandler<TNotification>(
                (serviceNotification, serviceCancellationToken) => service.HandleAsync(
                    serviceNotification,
                    serviceCancellationToken
                )
            ));

            return publishStrategy(notification, handlers, cancellationToken);
        }
    }
}
