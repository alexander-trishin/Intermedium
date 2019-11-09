namespace Intermedium.Pipeline
{
    /// <summary>
    /// Defines a mechanism which encapsulates the return value of the current request.
    /// </summary>
    /// <typeparam name="TResult">The type of the return value of the current request.</typeparam>
    public interface IPostProcessorContext<TResult>
    {
        /// <summary>
        /// Gets or sets the <typeparamref name="TResult"/> for the current request.
        /// </summary>
        TResult Result { get; set; }
    }
}
