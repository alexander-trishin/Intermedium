using System;
using System.Threading;
using System.Threading.Tasks;
using Intermedium.Pipeline;

namespace Intermedium.Extensions.Microsoft.DependencyInjection.Tests.Components
{
    internal sealed class CommandExceptionHandlerTest : ICommandExceptionHandler<CommandTest>
    {
        public Task HandleAsync(
            CommandTest request,
            Exception exception,
            IExceptionHandlerContext<VoidUnit> context,
            CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
