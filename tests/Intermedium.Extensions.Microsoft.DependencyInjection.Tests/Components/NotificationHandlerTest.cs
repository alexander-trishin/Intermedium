using System;
using System.Threading;
using System.Threading.Tasks;

namespace Intermedium.Extensions.Microsoft.DependencyInjection.Tests.Components
{
    internal sealed class NotificationHandlerTest : INotificationHandler<NotificationTest>
    {
        public Task HandleAsync(NotificationTest notification, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
