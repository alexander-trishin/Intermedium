using System;
using System.Threading;
using System.Threading.Tasks;
using Intermedium.Pipeline;

namespace Intermedium.Extensions.Microsoft.DependencyInjection.Tests.Components
{
    internal sealed class AsyncCommandExceptionHandlerTest : AsyncCommandExceptionHandler<CommandTest>
    {
        protected override Task HandleAsync(
            CommandTest command,
            Exception exception,
            IExceptionHandlerContext context,
            CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
