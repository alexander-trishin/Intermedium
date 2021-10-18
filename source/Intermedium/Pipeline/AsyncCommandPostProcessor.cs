using System.Threading;
using System.Threading.Tasks;

namespace Intermedium.Pipeline
{
    /// <summary>
    /// Serves as the base class for classes that asynchronously performs
    /// post-processing of a <typeparamref name="TCommand"/>.
    /// </summary>
    /// <typeparam name="TCommand">The type of a command.</typeparam>
    public abstract class AsyncCommandPostProcessor<TCommand> : ICommandPostProcessor<TCommand>
        where TCommand : ICommand
    {
        Task IQueryPostProcessor<TCommand, VoidUnit>.ProcessAsync(
            TCommand command,
            IPostProcessorContext<VoidUnit> context,
            CancellationToken cancellationToken)
        {
            return ProcessAsync(command, cancellationToken);
        }

        /// <summary>
        /// Performs post-processing of <typeparamref name="TCommand"/>.
        /// </summary>
        /// <param name="command">A command sent to <see cref="IMediatorSender"/>.</param>
        /// <param name="cancellationToken">
        /// A cancellation token that should be used to cancel the work.
        /// </param>
        /// <returns>A task that represents post-processing of <typeparamref name="TCommand"/>.</returns>
        public abstract Task ProcessAsync(TCommand command, CancellationToken cancellationToken);
    }
}
