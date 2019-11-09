using System;
using System.Linq;
using Intermedium.Compatibility;

namespace Intermedium.Pipeline.Steps.Internal
{
    internal sealed class ExceptionHandlerWrapper<TRequest, TResponse>
        where TRequest : IQuery<TResponse>
    {
        public Type ExceptionType { get; }

        public IQueryExceptionHandler<TRequest, TResponse> Handler { get; }

        public bool Skip { get; set; }

        public ExceptionHandlerWrapper(IQueryExceptionHandler<TRequest, TResponse> handler)
        {
            Handler = handler;
            ExceptionType = GetExceptionType(Handler);
        }

        public bool CanHandle(Type exceptionType)
        {
            return ReflectionBridge.CheckIsAssignable(ExceptionType, exceptionType);
        }

        private static Type GetExceptionType(IQueryExceptionHandler<TRequest, TResponse> handler)
        {
            var interfaceType = ReflectionBridge
                .FindInterfaces(
                    handler.GetType(),
                    x => ReflectionBridge.CheckIsGeneric(x)
                        && x.GetGenericTypeDefinition() == typeof(IQueryExceptionHandler<,,>)
                )
                .SingleOrDefault();

            return interfaceType is null
                ? typeof(Exception)
                : ReflectionBridge.GetGenericArguments(interfaceType)[2];
        }
    }
}
