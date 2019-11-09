namespace Intermedium.Pipeline
{
    /// <summary>
    /// Encapsulates the information about handling an exception
    /// occured in <see cref="ICommandHandler{TCommand}"/>.
    /// </summary>
    public interface IExceptionHandlerContext
    {
        /// <summary>
        /// Indicates whether the current exception was handled.
        /// </summary>
        bool Handled { get; set; }
    }

    /// <summary>
    /// Encapsulates the information about handling an exception
    /// occured in <see cref="IQueryHandler{TQuery, TResult}"/>.
    /// </summary>
    /// <typeparam name="TResult">The type of the return value for the current request.</typeparam>
    public interface IExceptionHandlerContext<TResult>
    {
        /// <summary>
        /// Indicates whether the current exception was handled.
        /// </summary>
        bool Handled { get; }

        /// <summary>
        /// The response that is returned if <see cref="Handled"/> is <see langword="true"/>.
        /// </summary>
        TResult Result { get; }

        /// <summary>
        /// Indicates that the current exception was handled.
        /// </summary>
        /// <param name="result">The response to return.</param>
        void ExceptionWasHandled(TResult result);
    }
}
