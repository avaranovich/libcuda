using System;
using Libcuda.Api.Native;
using Libcuda.Api.Native.DataTypes;
using Libcuda.Tracing;
using Libcuda.Versions;
using XenoGears.Functional;
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
        // this is of little priority though, since driver caches kernels as well
        public JitResult Compile(String ptx)
        {
            var log = Traces.Jit.Info;
            log.EnsureBlankLine();

            log.WriteLine("Peforming JIT compilation...");
            log.WriteLine("    PTX source text                              : {0}", "(see below)");
            log.WriteLine("    Target hardware ISA                          : {0}", TargetFromContext ? "(determined from context)" : Target.ToString());
            log.WriteLine("    Actual hardware ISA                          : {0}", CudaVersions.HardwareIsa);
            log.WriteLine("    Max registers per thread                     : {0}", MaxRegistersPerThread);
            log.WriteLine("    Planned threads per block                    : {0}", PlannedThreadsPerBlock);
            log.WriteLine("    Optimization level (0 - 4, higher is better) : {0}", OptimizationLevel);

            log.EnsureBlankLine();
            log.WriteLine("*".Repeat(120));
            log.WriteLine(ptx.TrimEnd());
            log.WriteLine(120.Times("*"));

            var options = new CUjit_options();
            options.MaxRegistersPerThread = MaxRegistersPerThread;
            options.PlannedThreadsPerBlock = PlannedThreadsPerBlock;
            options.OptimizationLevel = OptimizationLevel;
            // todo. an attempt to pass the Target value directly leads to CUDA_ERROR_INVALID_VALUE
            // as of now, this feature is not really important, so I'm marking it as TBI
            options.TargetFromContext = TargetFromContext.AssertTrue();
            options.Target = Target.ToCUjit_target();
            options.FallbackStrategy = CUjit_fallbackstrategy.PreferPtx;

            var native_result = nvcuda.cuModuleLoadDataEx(ptx, options);
            return new JitResult(this, ptx, native_result);
        }
    }
}