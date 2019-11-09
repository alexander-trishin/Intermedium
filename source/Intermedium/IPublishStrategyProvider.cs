namespace Intermedium
{
    /// <summary>
    /// Defines a mechanism for retrieving a <see cref="PublishStrategy{TNotification}"/>.
    /// </summary>
    public interface IPublishStrategyProvider
    {
        /// <summary>
        /// Gets publish strategy for a <typeparamref name="TNotification"/>.
        /// </summary>
        /// <typeparam name="TNotification">The type of a notification.</typeparam>
        /// <returns>
        /// A <see cref="PublishStrategy{TNotification}"/> for a particular
        /// <typeparamref name="TNotification"/>.
        /// </returns>
        PublishStrategy<TNotification> Resolve<TNotification>() where TNotification : INotification;
    }
}
