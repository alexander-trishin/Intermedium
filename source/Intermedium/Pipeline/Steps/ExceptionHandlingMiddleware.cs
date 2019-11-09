using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Intermedium.Compatibility;
using Intermedium.Core.Internal;
using Intermedium.Pipeline.Steps.Internal;

namespace Intermedium.Pipeline.Steps
{
    /// <summary>
    /// The default <see cref="IMiddleware{TQuery, TResult}"/> for exception handling.
    /// </summary>
    /// <typeparam name="TQuery">The type of a query.</typeparam>
    /// <typeparam name="TResult">
    /// The type of the return value of a <typeparamref name="TQuery"/>.
    /// </typeparam>
    public sealed class ExceptionHandlingMiddleware<TQuery, TResult>
        : IMiddleware<TQuery, TResult>

        where TQuery : IQuery<TResult>
    {
        private readonly IEnumerable<ExceptionHandlerWrapper<TQuery, TResult>> _wrappers;

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="ExceptionHandlingMiddleware{TRequest, TResponse}"/> class.
        /// </summary>
        /// <param name="handlers">The list of exception handlers.</param>
        public ExceptionHandlingMiddleware(
            IEnumerable<IQueryExceptionHandler<TQuery, TResult>> handlers)
        {
            _wrappers = handlers
                .EmptyIfNull()
                .Select(x => new ExceptionHandlerWrapper<TQuery, TResult>(x))
                .OrderBy(x => x.ExceptionType, new ExceptionTypeComparer())
                .ToList();
        }

        /// <summary>
        /// Executes the current component in the pipeline.
        /// </summary>
        /// <param name="request">The request to <see cref="IMediator"/>.</param>
        /// <param name="nextAsync">The request handler.</param>
        /// <param name="cancellationToken">
        /// A cancellation token that should be used to cancel the work.
        /// </param>
        /// <returns>A task that represents a pipeline step.</returns>
        public async Task<TResult> ExecuteAsync(
            TQuery request,
            Func<Task<TResult>> nextAsync,
            CancellationToken cancellationToken)
        {
            try
            {
                return await nextAsync().ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                var exceptionType = exception.GetType();
                var context = new ExceptionHandlerContext<TResult>();

                while (!context.Handled || exceptionType != typeof(object))
                {
                    foreach (var wrapper in _wrappers)
                    {
                        if (!wrapper.Skip)
                        {
                            if (wrapper.CanHandle(exceptionType))
                            {
                                await wrapper.Handler
                                    .HandleAsync(request, exception, context, cancellationToken)
                                    .ConfigureAwait(false);

                                if (context.Handled)
                                {
                                    return context.Result;
                                }

                                wrapper.Skip = true;
                            }
                        }
                    }

                    exceptionType = ReflectionBridge.GetBaseType(exceptionType);
                }

                throw;
            }
        }
    }
}
