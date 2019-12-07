using System;
using System.Linq;
using Intermedium.Internal;
using Microsoft.Extensions.DependencyInjection;

namespace Intermedium
{
    /// <summary>
    /// Extension methods for adding <see cref="Intermedium"/> services to an <see cref="IServiceCollection"/>.
    /// </summary>
    public static class IntermediumServiceCollectionExtensions
    {
        /// <summary>
        /// Register all <see cref="Intermedium"/> services.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection" /> to add the service to.</param>
        /// <param name="setupAction">
        /// An <see cref="Action{T}"/> to configure the provided <see cref="IntermediumOptions"/>.
        /// </param>
        /// <returns>A <paramref name="services"/> parameter after the operation has completed.</returns>
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

        /// <summary>
        /// Register all <see cref="Intermedium"/> services from assemblies
        /// defined by <paramref name="assemblyMarkers"/>.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection" /> to add the service to.</param>
        /// <param name="assemblyMarkers">The types used to define assemblies for scanning.</param>
        /// <returns>A <paramref name="services"/> parameter after the operation has completed.</returns>
        public static IServiceCollection AddIntermedium(
            this IServiceCollection services,
            params Type[] assemblyMarkers)
        {
            return services.AddIntermedium(x => x.Scan(assemblyMarkers));
        }
    }
}
