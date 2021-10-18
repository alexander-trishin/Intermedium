using System;
using System.Linq;

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
            return exceptionType.IsAssignableFrom(ExceptionType);
        }

        private static Type GetExceptionType(IQueryExceptionHandler<TRequest, TResponse> handler)
        {
            var interfaceType = handler
                .GetType()
                .FindInterfaces((x, _) =>
                    x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IQueryExceptionHandler<,,>),
                    null)
                .SingleOrDefault();

            return interfaceType is null
                ? typeof(Exception)
                : interfaceType.GetGenericArguments()[2];
        }
    }
}
