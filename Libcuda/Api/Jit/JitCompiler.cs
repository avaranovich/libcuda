using System;
using Libcuda.Api.Native;
using Libcuda.Api.Native.DataTypes;
using Libcuda.Versions;
using XenoGears.Functional;
using XenoGears.Strings;
using XenoGears.Assertions;

namespace Libcuda.Api.Jit
{
    public class JitCompiler
    {
        public bool TargetFromContext { get; set; }
        public HardwareIsa Target { get; set; }
        private int _optimizationLevel = 4; // 0..4, higher is better
        public int OptimizationLevel { get { return _optimizationLevel; } set { _optimizationLevel = value; } }
        public int MaxRegistersPerThread { get; set; }
        public int PlannedThreadsPerBlock { get; set; }

        // todo. cache jitted kernels
        public JitResult Compile(String ptx)
        {
            Log.WriteLine("Peforming JIT compilation...");
            Log.WriteLine("    PTX source text                              : {0}", "(see below)");
            Log.WriteLine("    Target hardware ISA                          : {0}", TargetFromContext ? "(determined from context)" : Target.ToString());
            Log.WriteLine("    Actual hardware ISA                          : {0}", CudaVersions.HardwareIsa);
            Log.WriteLine("    Max registers per thread                     : {0}", MaxRegistersPerThread);
            Log.WriteLine("    Planned threads per block                    : {0}", PlannedThreadsPerBlock);
            Log.WriteLine("    Optimization level (0 - 4, higher is better) : {0}", OptimizationLevel);
            Log.WriteLine(Environment.NewLine + "*".Repeat(120));
            Log.WriteLine(ptx.QuoteBraces().TrimEnd());
            Log.WriteLine(120.Times("*"));
            Log.WriteLine();

            var options = new CUjit_options();
            options.MaxRegistersPerThread = MaxRegistersPerThread;
            options.PlannedThreadsPerBlock = PlannedThreadsPerBlock;
            options.OptimizationLevel = OptimizationLevel;
            // todo. an attempt to pass the Target value directly leads to CUDA_ERROR_INVALID_VALUE
            // as of now, this feature is not really important, so I'm marking it as TBI
            options.TargetFromContext = TargetFromContext.AssertTrue();
            options.Target = Target.ToCUjit_target();
            options.FallbackStrategy = CUjit_fallbackstrategy.PreferPtx;

            try
            {
                var result = nvcuda.cuModuleLoadDataEx(ptx, options);
                return new JitResult(this, ptx, result);
            }
            catch (CUjit_exception jex)
            {
                var result = new JitResult(this, ptx, jex.JitResult);
                throw new JitException(result, jex);
            }
        }
    }
}