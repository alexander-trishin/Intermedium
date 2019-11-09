using System;
using System.Diagnostics;

namespace Intermedium.Core.Internal
{
    internal static class Guard
    {
        [DebuggerHidden]
        public static T ThrowIfNull<T>(T value, string name)
        {
            return value == null
                ? throw new ArgumentNullException(name)
                : value;
        }
    }
}
