using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Intermedium.Compatibility;
using Intermedium.Pipeline;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Intermedium.Tests
{
    [TestClass]
    public class ReflectionBridgeTests
    {
        [DataTestMethod]
        [DynamicData(nameof(AssignableCheck), DynamicDataSourceType.Property)]
        public void CheckIsAssignable(Type from, Type to, bool isAssignable)
        {
            ReflectionBridge.CheckIsAssignable(from, to).Should().Be(isAssignable);
        }

        [DataTestMethod]
        [DynamicData(nameof(GenericCheck), DynamicDataSourceType.Property)]
        public void CheckIsGeneric(Type type, bool isGeneric)
        {
            ReflectionBridge.CheckIsGeneric(type).Should().Be(isGeneric);
        }

        [DataTestMethod]
        [DynamicData(nameof(BaseTypeCheck), DynamicDataSourceType.Property)]
        public void GetBaseType(Type type, Type parent)
        {
            ReflectionBridge.GetBaseType(type).Should().Be(parent);
        }

        [DataTestMethod]
        [DynamicData(nameof(GenericArgumentCheck), DynamicDataSourceType.Property)]
        public void GetGenericArguments(Type type, int index, Type target)
        {
            ReflectionBridge.GetGenericArguments(type)[index].Should().Be(target);
        }

        [TestMethod]
        public void FindInterfaces_CommandTimeoutExceptionHandler_HandlesTimeoutException()
        {
            var interfaces = ReflectionBridge.FindInterfaces(
                typeof(CommandTimeoutExceptionHandler),
                x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IQueryExceptionHandler<,,>)
            );

            interfaces.Should().HaveCount(1);

            var actual = ReflectionBridge.GetGenericArguments(interfaces[0])[2];

            actual.Should().Be(typeof(TimeoutException));
        }

        private static IEnumerable<object[]> AssignableCheck
        {
            get
            {
                yield return new object[] { typeof(Action), typeof(MulticastDelegate), true };
                yield return new object[] { typeof(ArgumentException), typeof(Exception), true };
                yield return new object[] { typeof(Array), typeof(List<>), false };
            }
        }

        private static IEnumerable<object[]> GenericCheck
        {
            get
            {
                yield return new object[] { typeof(object), false };
                yield return new object[] { typeof(IEnumerable<>), true };
                yield return new object[] { typeof(IEnumerable<object>), true };
            }
        }

        private static IEnumerable<object[]> BaseTypeCheck
        {
            get
            {
                yield return new object[] { typeof(Task<>), typeof(Task) };
                yield return new object[] { typeof(Exception), typeof(object) };
                yield return new object[] { typeof(MulticastDelegate), typeof(Delegate) };
            }
        }

        private static IEnumerable<object[]> GenericArgumentCheck
        {
            get
            {
                yield return new object[] { typeof(int?), 0, typeof(int) };
                yield return new object[] { typeof(IEnumerable<object>), 0, typeof(object) };
                yield return new object[] { typeof(Dictionary<int, string>), 1, typeof(string) };
            }
        }

        private sealed class DoNothing : ICommand { }

        private sealed class CommandTimeoutExceptionHandler
            : ICommandExceptionHandler<DoNothing, TimeoutException>
        {
            public Task HandleAsync(
                DoNothing request,
                Exception exception,
                IExceptionHandlerContext<VoidUnit> context,
                CancellationToken cancellationToken)
            {
                throw new NotImplementedException();
            }
        }
    }
}
