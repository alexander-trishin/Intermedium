using System;
using System.Threading;
using Intermedium.Pipeline;

namespace Intermedium.Extensions.Microsoft.DependencyInjection.Tests.Components
{
    internal sealed class SyncCommandExceptionHandlerTest : CommandExceptionHandler<CommandTest>
    {
        public override void Handle(
            CommandTest command,
            Exception exception,
            IExceptionHandlerContext context,
            CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
