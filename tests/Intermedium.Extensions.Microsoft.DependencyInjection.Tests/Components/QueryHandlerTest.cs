using System;
using System.Threading;
using System.Threading.Tasks;

namespace Intermedium.Extensions.Microsoft.DependencyInjection.Tests.Components
{
    internal sealed class QueryHandlerTest : IQueryHandler<QueryTest, int>
    {
        public Task<int> HandleAsync(QueryTest request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
