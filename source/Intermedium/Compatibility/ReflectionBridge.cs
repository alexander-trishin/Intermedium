using System;

#if !HAVE_FULL_REFLECTION
using System.Reflection;
using System.Linq;
#endif

namespace Intermedium.Compatibility
{
    internal static class ReflectionBridge
    {
        public static bool CheckIsAssignable(Type from, Type to)
        {
#if HAVE_FULL_REFLECTION
            return to.IsAssignableFrom(from);
#else
            return to.GetTypeInfo().IsAssignableFrom(from.GetTypeInfo());
#endif
        }

        public static bool CheckIsGeneric(Type type)
        {
#if HAVE_FULL_REFLECTION
            return type.IsGenericType;
#else
            return type.GetTypeInfo().IsGenericType;
#endif
        }

        public static Type GetBaseType(Type type)
        {
#if HAVE_FULL_REFLECTION
            return type.BaseType;
#else
            return type.GetTypeInfo().BaseType;
#endif
        }

        public static Type[] GetGenericArguments(Type type)
        {
#if HAVE_FULL_REFLECTION
            return type.GetGenericArguments();
#else
            return type.GetTypeInfo().GenericTypeArguments;
#endif
        }

        public static Type[] FindInterfaces(Type type, Func<Type, bool> searchCriteria)
        {
#if HAVE_FULL_REFLECTION
            return type.FindInterfaces((x, _) => searchCriteria(x), null);
#else
            return type
                .GetTypeInfo()
                .ImplementedInterfaces
                .Where(searchCriteria)
                .ToArray();
#endif
        }
    }
}
