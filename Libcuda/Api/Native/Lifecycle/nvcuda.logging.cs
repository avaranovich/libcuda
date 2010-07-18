using Libcuda.Tracing;
using XenoGears.Logging;

namespace Libcuda.Api.Native
{
    public static partial class nvcuda
    {
        private static LevelLogger Log { get { return Traces.Init.Info; } }
    }
}