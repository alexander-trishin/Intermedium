using System;
using System.Threading;

namespace Intermedium.Extensions.Microsoft.DependencyInjection.Tests.Components
{
    internal sealed class SyncNotificationHandlerTest : NotificationHandler<NotificationTest>
    {
        public override void Handle(NotificationTest notification, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
