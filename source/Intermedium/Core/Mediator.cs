using System;
using System.Threading;
using System.Threading.Tasks;
using Intermedium.Compatibility;
using Intermedium.Core.Internal;

namespace Intermedium.Core
{
    /// <summary>
    /// The default <see cref="IMediator"/>.
    /// </summary>
    public class Mediator : IMediator
    {
        private static readonly ConcurrentStore<Type, object> RequestHandlers
            = new ConcurrentStore<Type, object>();

        private static readonly ConcurrentStore<Type, object> NotificationHandlers
            = new ConcurrentStore<Type, object>();

        /// <summary>
        /// A mechanism for retrieving a service object.
        /// </summary>
        protected ServiceProvider ServiceProvider { get; }

        /// <summary>
        /// The default way <see cref="IMediator"/> uses the list of
        /// <see cref="NotificationHandler{TNotification}"/>.
        /// </summary>
        protected IPublishStrategyProvider DefaultPublishStrategy { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Mediator"/> class.
        /// </summary>
        /// <param name="serviceProvider">A mechanism for retrieving a service object.</param>
        /// <param name="defaultPublishStrategy">
        /// The default way <see cref="IMediator"/> uses the list of
        /// <see cref="NotificationHandler{TNotification}"/>. If not specified the value will be
        /// <see cref="PublishStrategyProvider.ParallelWhenAll" />.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="serviceProvider"/> is null.
        /// </exception>
        public Mediator(
            ServiceProvider serviceProvider,
            IPublishStrategyProvider defaultPublishStrategy = null)
        {
            ServiceProvider = Guard.ThrowIfNull(serviceProvider, nameof(serviceProvider));

            DefaultPublishStrategy = defaultPublishStrategy ?? PublishStrategyProvider.ParallelWhenAll;
        }

        /// <summary>
        /// Asynchronously sends a request to the single handler.
        /// </summary>
        /// <typeparam name="TResult">
        /// The type of the return value of a <see cref="IQuery{TResult}"/>.
        /// </typeparam>
        /// <param name="request">The request to <see cref="IMediator"/>.</param>
        /// <param name="cancellationToken">
        /// A cancellation token that should be used to cancel the work.
        /// </param>
        /// <returns>A task that represents the send operation.</returns>
        public Task<TResult> SendAsync<TResult>(
            IQuery<TResult> request,
            CancellationToken cancellationToken = default)
        {
            Guard.ThrowIfNull(request, nameof(request));

            cancellationToken.ThrowIfCancellationRequested();

            var handler = (RequestHandlerWrapper<TResult>)RequestHandlers.GetOrAdd(
                request.GetType(),
                type => Activator.CreateInstance(
                    typeof(RequestHandlerWrapper<,>).MakeGenericType(type, typeof(TResult))
                )
            );

            return handler.HandleAsync(request, ServiceProvider, cancellationToken);
        }

        /// <summary>
        /// Asynchronously sends a <typeparamref name="TNotification"/> to multiple handlers.
        /// </summary>
        /// <typeparam name="TNotification">The type of a notification.</typeparam>
        /// <param name="notification">The notification sent to <see cref="IMediator"/>.</param>
        /// <param name="cancellationToken">
        /// A cancellation token that should be used to cancel the work.
        /// </param>
        /// <returns>A task that represents the publish operation.</returns>
        public Task PublishAsync<TNotification>(
            TNotification notification,
            CancellationToken cancellationToken = default
        )
            where TNotification : INotification
        {
            return PublishAsync(notification, DefaultPublishStrategy, cancellationToken);
        }

        /// <summary>
        /// Asynchronously sends a <typeparamref name="TNotification"/> to multiple handlers.
        /// </summary>
        /// <typeparam name="TNotification">The type of a notification.</typeparam>
        /// <param name="notification">The notification sent to <see cref="IMediator"/>.</param>
        /// <param name="publishStrategy">
        /// The way <see cref="IMediator"/> uses the list of
        /// <see cref="NotificationHandler{TNotification}"/>.
        /// </param>
        /// <param name="cancellationToken">
        /// A cancellation token that should be used to cancel the work.
        /// </param>
        /// <returns>A task that represents the publish operation.</returns>
        public Task PublishAsync<TNotification>(
            TNotification notification,
            IPublishStrategyProvider publishStrategy,
            CancellationToken cancellationToken = default
        )
            where TNotification : INotification
        {
            Guard.ThrowIfNull(notification, nameof(notification));
            Guard.ThrowIfNull(publishStrategy, nameof(publishStrategy));

            cancellationToken.ThrowIfCancellationRequested();

            var handler = (NotificationHandlerWrapper<TNotification>)NotificationHandlers.GetOrAdd(
                typeof(TNotification),
                type => new NotificationHandlerWrapper<TNotification>()
            );

            return handler.HandleAsync(
                notification,
                publishStrategy.Resolve<TNotification>(),
                ServiceProvider,
                cancellationToken
            );
        }
    }
}
