using System.Threading;
using System.Threading.Tasks;

namespace Intermedium.Pipeline
{
    /// <summary>
    /// Serves as the base class for classes that synchronously performs
    /// pre-processing of a <typeparamref name="TCommand"/>.
    /// </summary>
    /// <typeparam name="TCommand">The type of a command.</typeparam>
    public abstract class CommandPreProcessor<TCommand> : ICommandPreProcessor<TCommand>
        where TCommand : ICommand
    {
        Task IQueryPreProcessor<TCommand, VoidUnit>.ProcessAsync(
            TCommand command,
            CancellationToken cancellationToken)
        {
            Process(command, cancellationToken);
            return Task.CompletedTask;
        }

        /// <summary>
        /// Performs pre-processing of <typeparamref name="TCommand"/>.
        /// </summary>
        /// <param name="command">A command sent to <see cref="IMediatorSender"/>.</param>
        /// <param name="cancellationToken">
        /// A cancellation token that should be used to cancel the work.
        /// </param>
        public abstract void Process(TCommand command, CancellationToken cancellationToken);
    }
}
