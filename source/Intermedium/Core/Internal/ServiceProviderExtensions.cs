using System;
using System.Collections.Generic;

namespace Intermedium.Core.Internal
{
    internal static class ServiceProviderExtensions
    {
        public static T GetService<T>(this ServiceProvider provider)
        {
            Guard.ThrowIfNull(provider, nameof(provider));

            return (T)provider(typeof(T));
        }

        public static IEnumerable<T> GetServices<T>(this ServiceProvider provider)
        {
            return GetService<IEnumerable<T>>(provider);
        }

        public static T GetRequiredService<T>(this ServiceProvider provider)
        {
            var service = GetService<T>(provider);

            if (service == null)
            {
                throw new InvalidOperationException($"No service registered for type '{typeof(T).FullName}'");
            }

            return service;
        }

        public static IEnumerable<T> GetRequiredServices<T>(this ServiceProvider provider)
        {
            return GetRequiredService<IEnumerable<T>>(provider);
        }
    }
}
