using System;
using System.Diagnostics;
using Libcuda.Api.Native;
using Libcuda.Api.Native.DataTypes;
using Libcuda.Versions;
using XenoGears.Functional;
using XenoGears.Logging;
using XenoGears.Strings;

namespace Libcuda.Api.Jit
{
    [DebuggerNonUserCode]
    public class JitCompiler
    {
        public HardwareIsa Target { get; set; }
        private int _optimizationLevel = 4; // 0..4, higher is better
        public int OptimizationLevel { get { return _optimizationLevel; } set { _optimizationLevel = value; } }
        public int MaxRegistersPerThread { get; set; }
        public int PlannedThreadsPerBlock { get; set; }

        public JitResult Compile(String ptx)
        {
            Log.TraceLine("Peforming JIT compilation...");
            Log.TraceLine("    PTX source text                              : {0}", "(see below)");
            Log.TraceLine("    Target ISA                                   : {0}", Target);
            Log.TraceLine("    Max registers per thread                     : {0}", MaxRegistersPerThread);
            Log.TraceLine("    Planned threads per block                    : {0}", PlannedThreadsPerBlock);
            Log.TraceLine("    Optimization level (0 - 4, higher is better) : {0}", OptimizationLevel);
            Log.TraceLine(Environment.NewLine + "*".Repeat(120));
            Log.TraceLine(ptx.QuoteBraces().TrimEnd());
            Log.TraceLine(120.Times("*"));
            Log.TraceLine();

            var options = new CUjit_options();
            options.MaxRegistersPerThread = MaxRegistersPerThread;
            options.PlannedThreadsPerBlock = PlannedThreadsPerBlock;
            options.OptimizationLevel = OptimizationLevel;
            options.TargetFromContext = false;
            options.Target = Target.ToCUjit_target();
            options.FallbackStrategy = CUjit_fallbackstrategy.PreferPtx;

            try
            {
                var result = nvcuda.cuModuleLoadDataEx(ptx, options);
                Log.TraceLine(result.InfoLog);
                Log.TraceLine();
                return new JitResult(this, ptx, result);
            }
            catch (JitException jex)
            {
                Log.TraceLine(jex.InfoLog);
                Log.TraceLine();
                throw;
            }
        }
    }
}