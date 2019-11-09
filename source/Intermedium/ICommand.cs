namespace Intermedium
{
    /// <summary>
    /// Defines a request that does not produce a value.
    /// </summary>
    public interface ICommand : IQuery<VoidUnit>
    {
    }
}
