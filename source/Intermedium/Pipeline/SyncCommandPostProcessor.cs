using System.Threading;
using System.Threading.Tasks;
using Intermedium.Compatibility;

namespace Intermedium.Pipeline
{
    /// <summary>
    /// Serves as the base class for classes that synchronously performs
    /// post-processing of a <typeparamref name="TCommand"/>.
    /// </summary>
    /// <typeparam name="TCommand">The type of a command.</typeparam>
    public abstract class SyncCommandPostProcessor<TCommand> : ICommandPostProcessor<TCommand>
        where TCommand : ICommand
    {
        Task IQueryPostProcessor<TCommand, VoidUnit>.ProcessAsync(
            TCommand command,
            IPostProcessorContext<VoidUnit> context,
            CancellationToken cancellationToken)
        {
            Process(command, cancellationToken);
            return TaskBridge.CompletedTask;
        }

        /// <summary>
        /// Performs post-processing of <typeparamref name="TCommand"/>.
        /// </summary>
        /// <param name="command">A command sent to <see cref="IMediator"/>.</param>
        /// <param name="cancellationToken">
        /// A cancellation token that should be used to cancel the work.
        /// </param>
        protected abstract void Process(TCommand command, CancellationToken cancellationToken);
    }
}
