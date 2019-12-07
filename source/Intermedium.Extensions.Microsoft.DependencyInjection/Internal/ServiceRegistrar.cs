using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Intermedium.Core;
using Intermedium.Internal.Extensions;
using Intermedium.Pipeline;
using Intermedium.Pipeline.Steps;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Intermedium.Internal
{
    internal static class ServiceRegistrar
    {
        public static void FindAndRegister(IServiceCollection services, IntermediumOptions options)
        {
            RegisterCoreServices(services, options);

            var definedTypes = new HashSet<TypeInfo>(options.ScanTargets.SelectMany(x => x.DefinedTypes));
            var openGenericInterfaces = GetOpenGenericInterfaces(services, options).ToList();
            var closedGenericTypes = definedTypes.Where(x => !x.IsOpenGeneric()).ToList();

            FindAndRegister(services, closedGenericTypes, typeof(IQueryHandler<,>).GetTypeInfo(), true);

            foreach (var openGenericInterface in openGenericInterfaces)
            {
                FindAndRegister(services, closedGenericTypes, openGenericInterface, false);

                foreach (var definedType in definedTypes)
                {
                    if (definedType.IsConcrete()
                        && definedType.IsOpenGeneric()
                        && definedType.FindCloseInterfaces(openGenericInterface).Any())
                    {
                        services.AddTransient(openGenericInterface.AsType(), definedType.AsType());
                    }
                }
            }
        }

        private static void RegisterCoreServices(IServiceCollection services, IntermediumOptions options)
        {
            services.TryAddTransient<ServiceProvider>(x => x.GetService);

            services.Add(new ServiceDescriptor(
                typeof(IMediator),
                options.MediatorImplementationType,
                options.MediatorLifetime
            ));
        }

        private static IEnumerable<TypeInfo> GetOpenGenericInterfaces(
            IServiceCollection services,
            IntermediumOptions options)
        {
            yield return typeof(INotificationHandler<>).GetTypeInfo();

            void AddMiddleware(Type middleware) => services.AddTransient(typeof(IMiddleware<,>), middleware);

            if (options.RegisterExceptionHandlingMiddleware)
            {
                AddMiddleware(typeof(ExceptionHandlingMiddleware<,>));
                yield return typeof(IQueryExceptionHandler<,>).GetTypeInfo();
            }

            if (options.RegisterPostProcessingMiddleware)
            {
                AddMiddleware(typeof(PostProcessingMiddleware<,>));
                yield return typeof(IQueryPostProcessor<,>).GetTypeInfo();
            }

            if (options.RegisterPreProcessingMiddleware)
            {
                AddMiddleware(typeof(PreProcessingMiddleware<,>));
                yield return typeof(IQueryPreProcessor<,>).GetTypeInfo();
            }
        }

        private static void FindAndRegister(
            IServiceCollection services,
            IEnumerable<TypeInfo> definedTypes,
            TypeInfo targetInterface,
            bool unique)
        {
            var concretions = new HashSet<TypeInfo>();
            var interfaces = new HashSet<TypeInfo>();

            foreach (var definedType in definedTypes)
            {
                var closeInterfaces = definedType.FindCloseInterfaces(targetInterface).ToList();
                if (closeInterfaces.Count == 0)
                {
                    continue;
                }

                if (definedType.IsConcrete())
                {
                    concretions.Add(definedType);
                }

                interfaces.UnionWith(closeInterfaces);
            }

            foreach (var @interface in interfaces)
            {
                var exactMatches = concretions.Where(x => @interface.IsAssignableFrom(x)).ToList();

                if (unique)
                {
                    if (exactMatches.Count > 1)
                    {
                        ThrowNonUniqueImplementationsException(@interface, exactMatches);
                    }
                    else if (exactMatches.Count > 0)
                    {
                        var descriptors = services
                            .Where(x => x.ServiceType == @interface.AsType())
                            .Select(x =>
                                (x.ImplementationType
                                    ?? x.ImplementationInstance?.GetType()
                                    ?? (Type)Type.Missing
                                ).GetTypeInfo()
                            )
                            .ToList();

                        if (descriptors.Count > 0)
                        {
                            ThrowNonUniqueImplementationsException(@interface, descriptors.Concat(new[] { exactMatches[0] }));
                        }
                    }
                }

                foreach (var match in exactMatches)
                {
                    services.AddTransient(@interface.AsType(), match.AsType());
                }
            }
        }

        private static void ThrowNonUniqueImplementationsException(
            TypeInfo @interface,
            IEnumerable<TypeInfo> concretions)
        {
            var implementations = string.Join(
                $"{Environment.NewLine}- ",
                new[] { string.Empty }.Concat(concretions.Select(x => x.FullName))
            );

            throw new InvalidOperationException(string.Format(
                CultureInfo.InvariantCulture,
                "{0} has multiple implementations (should be only one):{1}",
                @interface.FullName,
                implementations
            ));
        }
    }
}
