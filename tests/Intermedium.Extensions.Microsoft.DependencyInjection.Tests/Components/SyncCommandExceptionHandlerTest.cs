using System;
using System.Threading;
using Intermedium.Pipeline;

namespace Intermedium.Extensions.Microsoft.DependencyInjection.Tests.Components
{
    internal sealed class SyncCommandExceptionHandlerTest : SyncCommandExceptionHandler<CommandTest>
    {
        protected override void Handle(
            CommandTest command,
            Exception exception,
            IExceptionHandlerContext context,
            CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
