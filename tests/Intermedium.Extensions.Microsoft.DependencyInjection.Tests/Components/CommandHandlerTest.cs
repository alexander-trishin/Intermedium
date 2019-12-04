using System;
using System.Threading;
using System.Threading.Tasks;

namespace Intermedium.Extensions.Microsoft.DependencyInjection.Tests.Components
{
    internal sealed class CommandHandlerTest : ICommandHandler<CommandTest>
    {
        public Task<VoidUnit> HandleAsync(CommandTest request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
