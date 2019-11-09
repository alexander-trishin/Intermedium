using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Intermedium.Pipeline;
using Intermedium.Pipeline.Steps.Internal;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Intermedium.Tests
{
    [TestClass]
    public class ExceptionHandlerWrapperTests
    {
        [DataTestMethod]
        [DynamicData(nameof(CanHandleCheck), DynamicDataSourceType.Property)]
        public void CanHandle(Type type, bool canHandle)
        {
            var wrapper = new ExceptionHandlerWrapper<Ping, int>(new AsyncPingTimeoutExceptionHandler());

            wrapper.CanHandle(type).Should().Be(canHandle);
        }

        private static IEnumerable<object[]> CanHandleCheck
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
