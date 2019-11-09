using System;
using System.Threading.Tasks;

namespace Intermedium.Compatibility
{
    internal static class TaskBridge
    {
        public static Task CompletedTask => Task.Delay(TimeSpan.Zero);
    }
}
