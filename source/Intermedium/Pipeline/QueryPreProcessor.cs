using System.Threading;
using System.Threading.Tasks;

namespace Intermedium.Pipeline
{
    /// <summary>
    /// Serves as the base class for classes that synchronously performs
    /// pre-processing of a <typeparamref name="TQuery"/>.
    /// </summary>
    /// <typeparam name="TQuery">The type of a query.</typeparam>
    /// <typeparam name="TResult">
    /// The type of the return value of a <typeparamref name="TQuery"/>.
    /// </typeparam>
    public abstract class QueryPreProcessor<TQuery, TResult> : IQueryPreProcessor<TQuery, TResult>
        where TQuery : IQuery<TResult>
    {
        Task IQueryPreProcessor<TQuery, TResult>.ProcessAsync(
            TQuery query,
            CancellationToken cancellationToken)
        {
            Process(query, cancellationToken);
            return Task.CompletedTask;
        }

        /// <summary>
        /// Executes an action before <typeparamref name="TQuery"/> was handled.
        /// </summary>
        /// <param name="query">The query to <see cref="IMediatorSender"/>.</param>
        /// <param name="cancellationToken">
        /// A cancellation token that should be used to cancel the work.
        /// </param>
        public abstract void Process(TQuery query, CancellationToken cancellationToken);
    }
}
