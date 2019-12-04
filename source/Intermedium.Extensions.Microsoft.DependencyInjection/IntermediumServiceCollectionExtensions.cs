using System;
using System.Linq;
using Intermedium.Internal;
using Microsoft.Extensions.DependencyInjection;

namespace Intermedium
{
    public static class IntermediumServiceCollectionExtensions
    {
        public static IServiceCollection AddIntermedium(
            this IServiceCollection services,
            Action<IntermediumOptions> setupAction)
        {
            if (services is null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            var options = new IntermediumOptions();
            setupAction?.Invoke(options);

            if (!options.ScanTargets.Any())
            {
                throw new InvalidOperationException("Please, specify at least one assembly to scan.");
            }

            if (services.Any(x => x.ServiceType == typeof(IMediator)))
            {
                throw new InvalidOperationException(
                    nameof(IMediator) + " is already registered. "
                    + "Define your mediator implementation using setup action of this method."
                );
            }

            ServiceRegistrar.FindAndRegister(services, options);

            return services;
        }

        public static IServiceCollection AddIntermedium(
            this IServiceCollection services,
            params Type[] assemblyMarkers)
        {
            return services.AddIntermedium(x => x.Scan(assemblyMarkers));
        }
    }
}
