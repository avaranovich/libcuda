using System;
using System.Diagnostics;
using Libcuda.DataTypes;
using Libcuda.Api.Jit;
using Libcuda.Versions;
using XenoGears.Functional;

namespace Libcuda
{
    [DebuggerNonUserCode]
    public static class JitFacade
    {
        public static JittedKernel JitKernel(this String ptx, dim3 blockDim)
        {
            var compiler = new JitCompiler();
            compiler.MaxRegistersPerThread = 0;
            compiler.PlannedThreadsPerBlock = (int)blockDim.Product();
            compiler.TargetFromContext = true;

            var result = compiler.Compile(ptx);
            return new JittedKernel(result);
        }

        public static JittedKernel JitKernel(this String ptx, dim3 blockDim, HardwareIsa target)
        {
            var compiler = new JitCompiler();
            compiler.MaxRegistersPerThread = 0;
            compiler.PlannedThreadsPerBlock = (int)blockDim.Product();
            compiler.Target = target;

            var result = compiler.Compile(ptx);
            return new JittedKernel(result);
        }
    }
}