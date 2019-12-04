using System;
using System.Threading;

namespace Intermedium.Extensions.Microsoft.DependencyInjection.Tests.Components
{
    internal sealed class SyncNotificationHandlerTest : SyncNotificationHandler<NotificationTest>
    {
        protected override void Handle(NotificationTest notification, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
