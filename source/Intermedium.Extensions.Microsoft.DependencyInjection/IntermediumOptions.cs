using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Intermedium.Core;
using Microsoft.Extensions.DependencyInjection;

namespace Intermedium
{
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

        public IntermediumOptions Use<TMediator>(ServiceLifetime lifetime = ServiceLifetime.Transient)
            where TMediator : IMediator
        {
            MediatorImplementationType = typeof(TMediator);
            MediatorLifetime = lifetime;

            return this;
        }

        public IntermediumOptions RegisterMiddleware(
            bool exceptionHandling = true,
            bool postProcessing = true,
            bool preProcessing = true)
        {
            RegisterExceptionHandlingMiddleware = exceptionHandling;
            RegisterPostProcessingMiddleware = postProcessing;
            RegisterPreProcessingMiddleware = preProcessing;

            return this;
        }

        public IntermediumOptions Scan(params Type[] assemblyMarkers)
        {
            ScanTargets = new HashSet<Assembly>(assemblyMarkers.Select(x => x.GetTypeInfo().Assembly));

            return this;
        }
    }
}
