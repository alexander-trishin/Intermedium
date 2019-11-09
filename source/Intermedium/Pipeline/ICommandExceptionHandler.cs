using System;

namespace Intermedium.Pipeline
{
    /// <summary>
    /// Defines a mechanism for handling exception of type of <typeparamref name="TException"/>
    /// occured in <see cref="ICommandHandler{TCommand}"/>.
    /// </summary>
    /// <typeparam name="TCommand">The type of a command.</typeparam>
    /// <typeparam name="TException">The type of an exception.</typeparam>
    public interface ICommandExceptionHandler<in TCommand, in TException>
        : IQueryExceptionHandler<TCommand, VoidUnit, TException>

        where TCommand : ICommand
        where TException : Exception
    {
    }

    /// <summary>
    /// DDefines a mechanism for handling exception of type of <see cref="Exception"/> occured in
    /// <see cref="ICommandHandler{TCommand}"/>.
    /// </summary>
    /// <typeparam name="TCommand">The type of a command.</typeparam>
    public interface ICommandExceptionHandler<in TCommand>
        : ICommandExceptionHandler<TCommand, Exception>

        where TCommand : ICommand
    {
    }
}
