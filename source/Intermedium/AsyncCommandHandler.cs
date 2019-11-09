using System.Threading;
using System.Threading.Tasks;

namespace Intermedium
{
    /// <summary>
    /// Serves as the base class for classes that
    /// asynchronously handles a <typeparamref name="TCommand"/>.
    /// </summary>
    /// <typeparam name="TCommand">The type of a command.</typeparam>
    public abstract class AsyncCommandHandler<TCommand> : ICommandHandler<TCommand>
        where TCommand : ICommand
    {
        async Task<VoidUnit> IQueryHandler<TCommand, VoidUnit>.HandleAsync(
            TCommand command,
            CancellationToken cancellationToken)
        {
            await HandleAsync(command, cancellationToken).ConfigureAwait(false);
            return VoidUnit.Value;
        }

        /// <summary>
        /// Handles a <typeparamref name="TCommand"/>.
        /// </summary>
        /// <param name="command">A command to <see cref="IMediator"/>.</param>
        /// <param name="cancellationToken">
        /// A cancellation token that should be used to cancel the work.
        /// </param>
        /// <returns>A task that represents the command handling.</returns>
        protected abstract Task HandleAsync(TCommand command, CancellationToken cancellationToken);
    }
}
