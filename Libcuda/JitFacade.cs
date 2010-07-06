using System;
using System.Diagnostics;
using Libcuda.DataTypes;
using Libcuda.Devices;
using Libcuda.Jit;
using Libcuda.Versions;
using XenoGears.Functional;
using XenoGears.Assertions;

namespace Libcuda
{
    [DebuggerNonUserCode]
    public static class JitFacade
    {
        public static JittedKernel JitKernel(this String ptx, dim3 blockDim)
        {
            var device = CudaDevice.Current.AssertNotNull();
            var auto = device.Caps.ComputeCaps;
            return ptx.JitKernel(blockDim, auto);
        }

        public static JittedKernel JitKernel(this String ptx, dim3 blockDim, HardwareIsa target)
        {
            var compiler = new JitCompiler();
            compiler.MaxRegistersPerThread = 0;
            compiler.PlannedThreadsPerBlock = (int)blockDim.Product();
            compiler.Target = target;

            var result = compiler.Compile(ptx);
            return new JittedKernel(ptx, result);
        }
    }
}