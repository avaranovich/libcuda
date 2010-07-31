using System;
using System.Diagnostics;
using Libcuda.Api.Jit;
using Libcuda.Versions;

namespace Libcuda
{
    [DebuggerNonUserCode]
    public static class JitFacade
    {
        public static JittedKernel JitKernel(this String ptx)
        {
            return ptx.JitKernel(null);
        }

        public static JittedKernel JitKernel(this String ptx, JitTuning tuning)
        {
            var compiler = new JitCompiler();
            compiler.TargetFromContext = true;
            compiler.Tuning = tuning;

            var result = compiler.Compile(ptx);
            return new JittedKernel(result);
        }

        public static JittedKernel JitKernel(this String ptx, HardwareIsa target)
        {
            return ptx.JitKernel(null, target);
        }

        public static JittedKernel JitKernel(this String ptx, JitTuning tuning, HardwareIsa target)
        {
            var compiler = new JitCompiler();
            compiler.Target = target;
            compiler.Tuning = tuning;

            var result = compiler.Compile(ptx);
            return new JittedKernel(result);
        }
    }
}