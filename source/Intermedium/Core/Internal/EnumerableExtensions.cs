using System.Collections.Generic;
using System.Linq;

namespace Intermedium.Core.Internal
{
    internal static class EnumerableExtensions
    {
        public static IEnumerable<T> EmptyIfNull<T>(this IEnumerable<T> values)
        {
            return values ?? Enumerable.Empty<T>();
        }

        public static IEnumerable<T> Sort<T, TComparable>(
            this IEnumerable<T> values,
            IComparer<TComparable> comparer
        )
            where T : TComparable
        {
            return comparer is null
                ? values
                : values.OrderBy(x => x, comparer);
        }
    }
}
