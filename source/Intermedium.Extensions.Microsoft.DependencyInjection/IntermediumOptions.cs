using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Intermedium.Core;
using Microsoft.Extensions.DependencyInjection;

namespace Intermedium
{
    /// <summary>
    /// Provides programmatic configuration for components registration dependent on <see cref="Intermedium"/> package.
    /// </summary>
    public sealed class IntermediumOptions
    {
        internal IntermediumOptions()
        {
        }

        internal Type MediatorImplementationType { get; private set; } = typeof(Mediator);
        internal ServiceLifetime MediatorLifetime { get; private set; } = ServiceLifetime.Transient;

        internal IEnumerable<Assembly> ScanTargets { get; private set; } = Enumerable.Empty<Assembly>();

        internal bool RegisterExceptionHandlingMiddleware { get; private set; }
        internal bool RegisterPreProcessingMiddleware { get; private set; }
        internal bool RegisterPostProcessingMiddleware { get; private set; }

        /// <summary>
        /// Register mediator implementation.
        /// </summary>
        /// <typeparam name="TMediator">The <see cref="Type"/> implementing <see cref="IMediator"/>.</typeparam>
        /// <param name="lifetime">The lifetime of the <typeparamref name="TMediator"/> instance.</param>
        /// <returns>The current <see cref="IntermediumOptions"/>.</returns>
        public IntermediumOptions Use<TMediator>(ServiceLifetime lifetime = ServiceLifetime.Transient)
            where TMediator : IMediator
        {
            MediatorImplementationType = typeof(TMediator);
            MediatorLifetime = lifetime;

            return this;
        }

        /// <summary>
        /// Define assemblies to scan.
        /// </summary>
        /// <param name="assemblyMarkers">The types used to define assemblies for scanning.</param>
        /// <returns>The current <see cref="IntermediumOptions"/>.</returns>
        public IntermediumOptions Scan(params Type[] assemblyMarkers)
        {
            ScanTargets = new HashSet<Assembly>(assemblyMarkers.Select(x => x.GetTypeInfo().Assembly));

            return this;
        }

        /// <summary>
        /// Configures registration of the exception handling middleware.
        /// </summary>
        /// <param name="enabled">A value indicating whether
        /// the exception handling middleware should be registered.</param>
        /// <returns>The current <see cref="IntermediumOptions"/>.</returns>
        public IntermediumOptions WithExceptionHandling(bool enabled = true)
        {
            RegisterExceptionHandlingMiddleware = enabled;

            return this;
        }

        /// <summary>
        /// Configures registration of the pre-processing middleware.
        /// </summary>
        /// <param name="enabled">A value indicating whether
        /// the pre-processing middleware should be registered.</param>
        /// <returns>The current <see cref="IntermediumOptions"/>.</returns>
        public IntermediumOptions WithPreProcessing(bool enabled = true)
        {
            RegisterPreProcessingMiddleware = enabled;

            return this;
        }

        /// <summary>
        /// Configures registration of the post-processing middleware.
        /// </summary>
        /// <param name="enabled">A value indicating whether
        /// the post-processing middleware should be registered.</param>
        /// <returns>The current <see cref="IntermediumOptions"/>.</returns>
        public IntermediumOptions WithPostProcessing(bool enabled = true)
        {
            RegisterPostProcessingMiddleware = enabled;

            return this;
        }

        /// <summary>
        /// Registers all the middleware provided by <see cref="Intermedium"/> package.
        /// </summary>
        /// <returns>The current <see cref="IntermediumOptions"/>.</returns>
        public IntermediumOptions WithCoreMiddleware()
        {
            return WithExceptionHandling(true)
                .WithPreProcessing(true)
                .WithPostProcessing(true);
        }
    }
}
