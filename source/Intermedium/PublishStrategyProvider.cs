using System;
using System.Linq;
using System.Threading.Tasks;

namespace Intermedium
{
    /// <summary>
    /// The default <see cref="IPublishStrategyProvider"/>.
    /// </summary>
    public class PublishStrategyProvider : IPublishStrategyProvider
    {
        /// <summary>
        /// The name of the publish strategy.
        /// </summary>
        protected string Name { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="PublishStrategyProvider"/> class.
        /// </summary>
        /// <param name="name">The name of the publish strategy.</param>
        protected PublishStrategyProvider(string name)
        {
            Name = name;
        }

        /// <summary>
        /// Initializes a new instance of the class which resolves <see cref="ParallelWhenAll"/> strategy.
        /// </summary>
        public PublishStrategyProvider() : this(nameof(ParallelWhenAll))
        {
        }

        /// <summary>
        /// Finds publish strategy for a <typeparamref name="TNotification"/>.
        /// </summary>
        /// <typeparam name="TNotification">The type of a notification.</typeparam>
        /// <returns>
        /// A <see cref="PublishStrategy{TNotification}"/> for a particular
        /// <typeparamref name="TNotification"/>.
        /// </returns>
        /// <exception cref="NotSupportedException">
        /// Thrown when there is no publish strategy found for a <typeparamref name="TNotification"/>.
        /// </exception>
        public virtual PublishStrategy<TNotification> Resolve<TNotification>()
            where TNotification : INotification
        {
            return Name switch
            {
                nameof(ParallelWhenAll) => StrategyFor<TNotification>.ParallelWhenAll,
                nameof(ParallelWhenAny) => StrategyFor<TNotification>.ParallelWhenAny,
                nameof(Sequentially) => StrategyFor<TNotification>.Sequentially,
                _ => throw new NotSupportedException($"Unknown publish strategy: {Name}")
            };
        }

        /// <summary>
        /// Run each notification handler at parallel using <see cref="Task.WhenAll(Task[])"/>.
        /// </summary>
        public static IPublishStrategyProvider ParallelWhenAll { get; }
            = new PublishStrategyProvider(nameof(ParallelWhenAll));

        /// <summary>
        /// Run each notification handler at parallel using <see cref="Task.WhenAny(Task[])"/>.
        /// </summary>
        public static IPublishStrategyProvider ParallelWhenAny { get; }
            = new PublishStrategyProvider(nameof(ParallelWhenAny));

        /// <summary>
        /// Run each notification handler after one another.
        /// </summary>
        public static IPublishStrategyProvider Sequentially { get; }
            = new PublishStrategyProvider(nameof(Sequentially));

        private static class StrategyFor<TNotification>
            where TNotification : INotification
        {
            public static PublishStrategy<TNotification> ParallelWhenAll { get; }
                = async (notification, handlers, cancellationToken) =>
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    var controller = new TaskCompletionSource<object>();

                    using var registration = cancellationToken.Register(
                        () => controller.TrySetCanceled(),
                        useSynchronizationContext: false
                    );

                    var tasks = handlers.Select(x => x(notification, cancellationToken));

                    await Task.WhenAny(Task.WhenAll(tasks), controller.Task).ConfigureAwait(false);
                };

            public static PublishStrategy<TNotification> ParallelWhenAny { get; }
                = (notification, handlers, cancellationToken) =>
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    return Task.WhenAny(handlers.Select(x => x(notification, cancellationToken)));
                };

            public static PublishStrategy<TNotification> Sequentially { get; }
                = async (notification, handlers, cancellationToken) =>
                {
                    foreach (var handler in handlers)
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        await handler(notification, cancellationToken).ConfigureAwait(false);
                    }
                };
        }
    }
}
