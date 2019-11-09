namespace Intermedium.Pipeline
{
    /// <summary>
    /// Defines a mechanism which encapsulates an action to perform
    /// after <typeparamref name="TCommand"/> was handled.
    /// </summary>
    /// <typeparam name="TCommand">The type of a command.</typeparam>
    public interface ICommandPostProcessor<TCommand> : IQueryPostProcessor<TCommand, VoidUnit>
        where TCommand : ICommand
    {
    }
}
