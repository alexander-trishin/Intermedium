using System.Threading;
using System.Threading.Tasks;

namespace Intermedium
{
    /// <summary>
    /// Serves as the base class for classes that
    /// synchronously handles a <typeparamref name="TCommand"/>.
    /// </summary>
    /// <typeparam name="TCommand">The type of a command.</typeparam>
    public abstract class SyncCommandHandler<TCommand> : ICommandHandler<TCommand>
        where TCommand : ICommand
    {
        private static readonly Task<VoidUnit> NothingAsTask = Task.FromResult(VoidUnit.Value);

        Task<VoidUnit> IQueryHandler<TCommand, VoidUnit>.HandleAsync(
            TCommand command,
            CancellationToken cancellationToken)
        {
            Handle(command, cancellationToken);
            return NothingAsTask;
        }

        /// <summary>
        /// Handles a <typeparamref name="TCommand"/>.
        /// </summary>
        /// <param name="command">A command sent to <see cref="IMediator"/>.</param>
        /// <param name="cancellationToken">
        /// A cancellation token that should be used to cancel the work.
        /// </param>
        protected abstract void Handle(TCommand command, CancellationToken cancellationToken);
    }
}
