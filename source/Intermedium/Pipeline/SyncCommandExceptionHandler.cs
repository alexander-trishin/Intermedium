using System;
using System.Threading;
using System.Threading.Tasks;
using Intermedium.Compatibility;
using Intermedium.Pipeline.Steps.Internal;

namespace Intermedium.Pipeline
{
    /// <summary>
    /// Serves as the base class for classes that synchronously handles an exception of type of
    /// <typeparamref name="TException"/> occured in <see cref="ICommandHandler{TCommand}"/>.
    /// </summary>
    /// <typeparam name="TCommand">The type of a command.</typeparam>
    /// <typeparam name="TException">The type of an exception.</typeparam>
    public abstract class SyncCommandExceptionHandler<TCommand, TException>
        : ICommandExceptionHandler<TCommand, TException>

        where TCommand : ICommand
        where TException : Exception
    {
        Task IQueryExceptionHandler<TCommand, VoidUnit>.HandleAsync(
            TCommand command,
            Exception exception,
            IExceptionHandlerContext<VoidUnit> context,
            CancellationToken cancellationToken)
        {
            using var localContext = new ExceptionHandlerContext(context);
            Handle(command, (TException)exception, localContext, cancellationToken);
            return TaskBridge.CompletedTask;
        }


        /// <summary>
        /// Handles a <typeparamref name="TException"/>
        /// occured in <see cref="ICommandHandler{TCommand}"/>.
        /// </summary>
        /// <param name="command">A command sent to <see cref="IMediator"/>.</param>
        /// <param name="exception">
        /// An exception occured in <see cref="ICommandHandler{TCommand}"/>.
        /// </param>
        /// <param name="context">
        /// An encapsulated information about the progress of exception handling.
        /// </param>
        /// <param name="cancellationToken">
        /// A cancellation token that should be used to cancel the work.
        /// </param>
        protected abstract void Handle(
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
    public abstract class SyncCommandExceptionHandler<TCommand>
        : SyncCommandExceptionHandler<TCommand, Exception>, ICommandExceptionHandler<TCommand>

        where TCommand : ICommand
    {
    }
}
