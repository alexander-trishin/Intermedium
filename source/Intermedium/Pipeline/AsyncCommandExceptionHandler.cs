using System;
using System.Threading;
using System.Threading.Tasks;
using Intermedium.Pipeline.Steps.Internal;

namespace Intermedium.Pipeline
{
    /// <summary>
    /// Serves as the base class for classes that asynchronously handles an exception of type of
    /// <typeparamref name="TException"/> occured in <see cref="ICommandHandler{TCommand}"/>.
    /// </summary>
    /// <typeparam name="TCommand">The type of a command.</typeparam>
    /// <typeparam name="TException">The type of an exception.</typeparam>
    public abstract class AsyncCommandExceptionHandler<TCommand, TException>
        : ICommandExceptionHandler<TCommand, TException>

        where TCommand : ICommand
        where TException : Exception
    {
        async Task IQueryExceptionHandler<TCommand, VoidUnit>.HandleAsync(
            TCommand command,
            Exception exception,
            IExceptionHandlerContext<VoidUnit> context,
            CancellationToken cancellationToken)
        {
            using var localContext = new ExceptionHandlerContext(context);

            await HandleAsync(
                command,
                (TException)exception,
                localContext,
                cancellationToken
            )
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Handles a <typeparamref name="TException"/>
        /// occured in <see cref="ICommandHandler{TCommand}"/>.
        /// </summary>
        /// <param name="command">A command sent to <see cref="IMediatorSender"/>.</param>
        /// <param name="exception">
        /// An exception occured in <see cref="ICommandHandler{TCommand}"/>.
        /// </param>
        /// <param name="context">
        /// An information about the progress of exception handling.
        /// </param>
        /// <param name="cancellationToken">
        /// A cancellation token that should be used to cancel the work.
        /// </param>
        /// <returns>A task that represents exception handling.</returns>
        public abstract Task HandleAsync(
            TCommand command,
            TException exception,
            IExceptionHandlerContext context,
            CancellationToken cancellationToken
        );
    }

    /// <summary>
    /// Serves as the base class for classes that synchronously handles an exception of type of
    /// <see cref="Exception"/> occured in <see cref="ICommandHandler{TCommand}"/>.
    /// </summary>
    /// <typeparam name="TCommand">The type of a command.</typeparam>
    public abstract class AsyncCommandExceptionHandler<TCommand>
        : AsyncCommandExceptionHandler<TCommand, Exception>, ICommandExceptionHandler<TCommand>

        where TCommand : ICommand
    {
    }
}
