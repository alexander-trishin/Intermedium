namespace Intermedium.Pipeline
{
    /// <summary>
    /// Defines a mechanism which encapsulates an action to perform
    /// before <typeparamref name="TCommand"/> was handled.
    /// </summary>
    /// <typeparam name="TCommand">The type of a command.</typeparam>
    public interface ICommandPreProcessor<in TCommand> : IQueryPreProcessor<TCommand, VoidUnit>
        where TCommand : ICommand
    {
    }
}
