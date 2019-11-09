using System;

namespace Intermedium.Core
{
    /// <summary>
    /// Represents the method that will receive the service object of the specified type.
    /// </summary>
    /// <param name="serviceType">An object that specifies the type of service object to get.</param>
    /// <returns>
    /// A service object of type serviceType.
    /// -or- null if there is no service object of type serviceType.
    /// </returns>
    /// <exception cref="ArgumentNullException"/>
    public delegate object ServiceProvider(Type serviceType);
}
