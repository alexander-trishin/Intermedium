using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Intermedium.Pipeline;
using Intermedium.Pipeline.Steps.Internal;
using Xunit;

namespace Intermedium.Tests
{
    public class ExceptionHandlerWrapperTests
    {
        [Theory]
        [MemberData(nameof(CanHandleCheck))]
        public void CanHandle_ShouldHandleException_WhenThrownExceptionIsAssignableToTarget(Type type, bool canHandle)
        {
            var wrapper = new ExceptionHandlerWrapper<Ping, int>(new AsyncPingTimeoutExceptionHandler());

            wrapper.CanHandle(type).Should().Be(canHandle);
        }

        public static IEnumerable<object[]> CanHandleCheck
        {
            get
            {
                yield return new object[] { typeof(Exception), true };
                yield return new object[] { typeof(SystemException), true };
                yield return new object[] { typeof(TimeoutException), true };
                yield return new object[] { typeof(StackOverflowException), false };
            }
        }

        private sealed class Ping : IQuery<int>
        {
        }

        private sealed class AsyncPingTimeoutExceptionHandler
            : IQueryExceptionHandler<Ping, int, TimeoutException>
        {
            public Task HandleAsync(
                Ping request,
                Exception exception,
                IExceptionHandlerContext<int> context,
                CancellationToken cancellationToken)
            {
                throw new NotImplementedException();
            }
        }
    }
}
