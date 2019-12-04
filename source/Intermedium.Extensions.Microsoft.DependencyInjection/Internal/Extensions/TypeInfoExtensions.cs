using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Intermedium.Internal.Extensions
{
    internal static class TypeInfoExtensions
    {
        public static bool CanCloseTo(this TypeInfo openConcretion, TypeInfo closedInterface)
        {
            var openInterface = closedInterface.GetGenericTypeDefinition().GetTypeInfo();
            var arguments = closedInterface.GenericTypeArguments;

            var concreteArguments = openConcretion.GenericTypeArguments;

            return arguments.Length == concreteArguments.Length
                && openInterface.IsAssignableFrom(openConcretion);
        }

        public static bool IsConcrete(this TypeInfo type)
        {
            return !type.IsAbstract && !type.IsInterface;
        }

        public static bool MatchInterface(this TypeInfo current, TypeInfo target)
        {
            if (current is null || target is null)
            {
                return false;
            }

            if (current.IsInterface)
            {
                if (current.GenericTypeArguments.SequenceEqual(target.GenericTypeArguments))
                {
                    return true;
                }
            }
            else
            {
                var newType = current
                    .ImplementedInterfaces
                    .FirstOrDefault(x => x.Name == target.Name);

                return MatchInterface(newType.GetTypeInfo(), target);
            }

            return false;
        }

        public static bool IsOpenGeneric(this TypeInfo type)
        {
            return type.IsGenericTypeDefinition || type.ContainsGenericParameters;
        }

        public static IEnumerable<TypeInfo> FindCloseInterfaces(this TypeInfo current, TypeInfo target)
        {
            return FindAllCloseInterfaces(current, target).Distinct();
        }

        private static IEnumerable<TypeInfo> FindAllCloseInterfaces(TypeInfo current, TypeInfo target)
        {
            if (current is null || !current.IsConcrete())
            {
                yield break;
            }

            if (target.IsInterface)
            {
                foreach (var @interface in current.ImplementedInterfaces.Select(x => x.GetTypeInfo()))
                {
                    if (@interface.IsGenericType && @interface.GetGenericTypeDefinition() == target.AsType())
                    {
                        yield return @interface;
                    }
                }
            }
            else
            {
                var baseInfo = current.BaseType.GetTypeInfo();
                if (baseInfo.IsGenericType && baseInfo.GetGenericTypeDefinition() == target.AsType())
                {
                    yield return baseInfo;
                }
            }

            if (current.BaseType == typeof(object))
            {
                yield break;
            }

            foreach (var @interface in FindAllCloseInterfaces(current.BaseType.GetTypeInfo(), target))
            {
                yield return @interface;
            }
        }
    }
}
