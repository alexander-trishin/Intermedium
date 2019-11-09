using System;
using System.Threading;
using System.Threading.Tasks;
using Intermedium.Compatibility;

namespace Intermedium.Pipeline
{
    /// <summary>
    /// Serves as the base class for classes that synchronously handles an exception of type of
    /// <typeparamref name="TException"/> occured in <see cref="IQueryHandler{TQuery, TResult}"/>.
    /// </summary>
    /// <typeparam name="TQuery">The type of a query.</typeparam>
    /// <typeparam name="TResult">
    /// The type of the return value of a <typeparamref name="TQuery"/>.
    /// </typeparam>
    /// <typeparam name="TException">The type of an exception.</typeparam>
    public abstract class SyncQueryExceptionHandler<TQuery, TResult, TException>
        : IQueryExceptionHandler<TQuery, TResult, TException>

        where TQuery : IQuery<TResult>
        where TException : Exception
    {
        Task IQueryExceptionHandler<TQuery, TResult>.HandleAsync(
            TQuery query,
            Exception exception,
            IExceptionHandlerContext<TResult> context,
            CancellationToken cancellationToken)
        {
            Handle(query, (TException)exception, context, cancellationToken);
            return TaskBridge.CompletedTask;
        }

        /// <summary>
        /// Handles a <typeparamref name="TException"/>
        /// occured in <see cref="IQueryHandler{TQuery, TResult}"/>.
        /// </summary>
        /// <param name="query"></param>
        /// <param name="exception">
        /// An exception occured in <see cref="IQueryHandler{TQuery, TResult}"/>.
        /// </param>
        /// <param name="context">
        /// An information about the progress of exception handling.
        /// </param>
        /// <param name="cancellationToken">
        /// A cancellation token that should be used to cancel the work.
        /// </param>
        protected abstract void Handle(
            TQuery query,
            TException exception,
            IExceptionHandlerContext<TResult> context,
            CancellationToken cancellationToken
        );
    }

    /// <summary>
    /// Serves as the base class for classes that synchronously handles an exception of type of
    /// <see cref="Exception"/> occured in <see cref="IQueryHandler{TQuery, TResult}"/>.
    /// </summary>
    /// <typeparam name="TQuery">The type of a query.</typeparam>
    /// <typeparam name="TResult">
    /// The type of the return value of a <typeparamref name="TQuery"/>.
    /// </typeparam>
    public abstract class SyncQueryExceptionHandler<TQuery, TResult>
        : SyncQueryExceptionHandler<TQuery, TResult, Exception>

        where TQuery : IQuery<TResult>
    {
    }
}
