using System;
using System.Collections.Generic;
using Intermedium.Compatibility;

namespace Intermedium.Pipeline.Steps.Internal
{
    internal sealed class ExceptionTypeComparer : IComparer<Type>
    {
        public int Compare(Type x, Type y)
        {
            if (ReflectionBridge.CheckIsAssignable(x, y))
            {
                return -1;
            }

            if (ReflectionBridge.CheckIsAssignable(y, x))
            {
                return 1;
            }

            return 0;
        }
    }
}
