using System.Diagnostics;
using XenoGears.Logging;

namespace Libcuda.Tracing
{
    [DebuggerNonUserCode]
    public static class Traces
    {
        public readonly static Logger Init = Logger.Get("Libcuda.Init");
        public readonly static Logger Jit = Logger.Get("Libcuda.Jit");
        public readonly static Logger Run = Logger.Get("Libcuda.Run");
    }
}