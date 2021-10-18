using System;
using System.Collections.Generic;

namespace Intermedium.Pipeline.Steps.Internal
{
    internal sealed class ExceptionTypeComparer : IComparer<Type>
    {
        public int Compare(Type x, Type y)
        {
            if (y.IsAssignableFrom(x))
            {
                return -1;
            }

            if (x.IsAssignableFrom(y))
            {
                return 1;
            }

            return 0;
        }
    }
}
