using System.Diagnostics;

namespace Libcuda.Api.Native.DataTypes
{
    [DebuggerNonUserCode]
    public class CUjit_options
    {
        public int MaxRegistersPerThread { get; set; }
        public int PlannedThreadsPerBlock { get; set; }
        private int _optimizationLevel = 4; // 0..4, higher is better
        public int OptimizationLevel { get { return _optimizationLevel; } set { _optimizationLevel = value; } }
        public bool TargetFromContext { get; set; }
        public CUjit_target Target { get; set; }
        public CUjit_fallbackstrategy FallbackStrategy { get; set; }
    }
}