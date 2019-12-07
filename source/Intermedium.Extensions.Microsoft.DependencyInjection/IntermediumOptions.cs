using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Intermedium.Core;
using Microsoft.Extensions.DependencyInjection;

namespace Intermedium
{
    /// <summary>
    /// Provides programmatic configuration for components registration dependent on Intermedium package.
    /// </summary>
    public sealed class IntermediumOptions
    {
        internal IntermediumOptions()
        {
        }

        internal Type MediatorImplementationType { get; private set; } = typeof(Mediator);
        internal ServiceLifetime MediatorLifetime { get; private set; } = ServiceLifetime.Transient;

        internal IEnumerable<Assembly> ScanTargets { get; private set; } = Enumerable.Empty<Assembly>();

        internal bool RegisterExceptionHandlingMiddleware { get; private set; } = true;
        internal bool RegisterPreProcessingMiddleware { get; private set; } = true;
        internal bool RegisterPostProcessingMiddleware { get; private set; } = true;

        /// <summary>
        /// Register mediator implementation.
        /// </summary>
        /// <typeparam name="TMediator">The <see cref="Type"/> implementing <see cref="IMediator"/>.</typeparam>
        /// <param name="lifetime">The lifetime of the <typeparamref name="TMediator"/> instance.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
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
        /// <returns>A reference to this instance after the operation has completed.</returns>
        public IntermediumOptions Scan(params Type[] assemblyMarkers)
        {
            ScanTargets = new HashSet<Assembly>(assemblyMarkers.Select(x => x.GetTypeInfo().Assembly));

            return this;
        }

        /// <summary>
        /// Define whether to register default middlewares or not.
        /// </summary>
        /// <param name="exceptionHandling">Register exception handling middleware.</param>
        /// <param name="postProcessing">Register post processing middleware.</param>
        /// <param name="preProcessing">Register pre processing middleware.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        public IntermediumOptions ConfigurePipeline(
            bool? exceptionHandling = null,
            bool? postProcessing = null,
            bool? preProcessing = null)
        {
            static void TrySet(bool? set, Action<bool> setAction)
            {
                if (set != null)
                {
                    setAction(set.Value);
                }
            }

            TrySet(exceptionHandling, x => RegisterExceptionHandlingMiddleware = x);
            TrySet(postProcessing, x => RegisterPostProcessingMiddleware = x);
            TrySet(preProcessing, x => RegisterPreProcessingMiddleware = x);

            return this;
        }
    }
}
