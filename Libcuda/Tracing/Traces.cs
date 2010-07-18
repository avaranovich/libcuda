using System.Diagnostics;
using XenoGears.Logging;

namespace Libcuda.Tracing
{
    [DebuggerNonUserCode]
    public static class Traces
    {
        internal readonly static Logger Init = Logger.Get("Libcuda.Init");
        internal readonly static Logger Jit = Logger.Get("Libcuda.Jit");
        internal readonly static Logger Run = Logger.Get("Libcuda.Run");
    }
}