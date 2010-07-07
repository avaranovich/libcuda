using System.Diagnostics;
using XenoGears.Logging;

namespace Libcuda.Tracing
{
    [DebuggerNonUserCode]
    public static class Traces
    {
        internal static Logger Init { get; private set; }
        internal static Logger Jit { get; private set; }
        internal static Logger Run { get; private set; }

        static Traces()
        {
            Init = new Logger("Libcuda.Init");
            Jit = new Logger("Libcuda.Jit");
            Run = new Logger("Libcuda.Run");
        }
    }
}