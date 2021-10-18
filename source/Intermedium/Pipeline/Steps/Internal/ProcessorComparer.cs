using System.Collections.Generic;

namespace Intermedium.Pipeline.Steps.Internal
{
    internal sealed class ProcessorComparer<T> : IComparer<T>
    {
        public int Compare(T x, T y)
        {
            var xIndex = (x as IOrderedProcessor)?.Order ?? 0;
            var yIndex = (y as IOrderedProcessor)?.Order ?? 0;

            return xIndex.CompareTo(yIndex);
        }
    }
}
