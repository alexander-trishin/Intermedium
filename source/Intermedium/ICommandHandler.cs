namespace Intermedium
{
    /// <summary>
    /// Defines the handler for a <typeparamref name="TCommand"/>.
    /// </summary>
    /// <typeparam name="TCommand">The type of a command.</typeparam>
    public interface ICommandHandler<in TCommand> : IQueryHandler<TCommand, VoidUnit>
        where TCommand : ICommand
    {
    }
}
