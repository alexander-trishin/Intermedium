namespace Intermedium
{
    /// <summary>
    /// Defines a request that produces a value of the type
    /// specified by the <typeparamref name="TResult"/> parameter.
    /// </summary>
    /// <typeparam name="TResult">The type of the return value.</typeparam>
    public interface IQuery<out TResult>
    {
    }
}
