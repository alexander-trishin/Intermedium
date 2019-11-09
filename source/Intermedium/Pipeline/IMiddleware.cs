using System;
using System.Threading;
using System.Threading.Tasks;

namespace Intermedium.Pipeline
{
    /// <summary>
    /// Defines a component that's assembled into a pipeline to handle requests and responses. 
    /// Can perform work before and after the next component in the pipeline.
    /// </summary>
    /// <typeparam name="TQuery">The type of a query.</typeparam>
    /// <typeparam name="TResult">
    /// The type of the return value of a <typeparamref name="TQuery"/>.
    /// </typeparam>
    public interface IMiddleware<TQuery, TResult>
        where TQuery : IQuery<TResult>
    {
        /// <summary>
        /// Executes the current component in the pipeline.
        /// </summary>
        /// <param name="request">The request to <see cref="IMediator"/>.</param>
        /// <param name="nextAsync">The request handler.</param>
        /// <param name="cancellationToken">
        /// A cancellation token that should be used to cancel the work.
        /// </param>
        /// <returns>A task that represents a pipeline step.</returns>
        Task<TResult> ExecuteAsync(
            TQuery request,
            Func<Task<TResult>> nextAsync,
            CancellationToken cancellationToken
        );
    }
}
