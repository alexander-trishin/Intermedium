using System;
using System.Threading;
using System.Threading.Tasks;

namespace Intermedium.Pipeline
{
    /// <summary>
    /// Defines a mechanism for handling exception of type of <see cref="Exception"/> occured in
    /// <see cref="IQueryHandler{TQuery, TResult}"/>.
    /// </summary>
    /// <typeparam name="TQuery">The type of a query.</typeparam>
    /// <typeparam name="TResult">
    /// The type of the return value of a <typeparamref name="TQuery"/>.
    /// </typeparam>
    public interface IQueryExceptionHandler<in TQuery, TResult>
        where TQuery : IQuery<TResult>
    {
        /// <summary>
        /// Handles an exception. 
        /// </summary>
        /// <param name="request">The request to <see cref="IMediatorSender"/>.</param>
        /// <param name="exception">The instance of the occurred exception.</param>
        /// <param name="context">The information about handling an exception.</param>
        /// <param name="cancellationToken">
        /// A cancellation token that should be used to cancel the work.
        /// </param>
        /// <returns>A task that represents exception handling.</returns>
        Task HandleAsync(
            TQuery request,
            Exception exception,
            IExceptionHandlerContext<TResult> context,
            CancellationToken cancellationToken
        );
    }

    /// <summary>
    /// Defines a mechanism for handling exception of type of <typeparamref name="TException"/>
    /// occured in <see cref="IQueryHandler{TQuery, TResult}"/>.
    /// </summary>
    /// <typeparam name="TQuery">The type of a query.</typeparam>
    /// <typeparam name="TResult">
    /// The type of the return value of a <typeparamref name="TQuery"/>.
    /// </typeparam>
    /// <typeparam name="TException">The type of an exception.</typeparam>
    public interface IQueryExceptionHandler<in TQuery, TResult, in TException>
        : IQueryExceptionHandler<TQuery, TResult>

        where TQuery : IQuery<TResult>
        where TException : Exception
    {
    }
}
