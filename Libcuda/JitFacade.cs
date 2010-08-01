using System;
using System.Diagnostics;
using Libcuda.Api.Jit;
using Libcuda.DataTypes;
using Libcuda.Versions;
using XenoGears.Assertions;

namespace Libcuda
{
    [DebuggerNonUserCode]
    public static class JitFacade
    {
        public static JittedKernel JitKernel(this String ptx, dim3 reqntid)
        {
            ptx.AssertNotNull();
            CudaDriver.Ensure();

            var tuning = new JitTuning { Reqntid = reqntid };
            return ptx.JitKernel(tuning);
        }

        public static JittedKernel JitKernel(this String ptx, JitTuning tuning)
        {
            ptx.AssertNotNull();
            CudaDriver.Ensure();

            var compiler = new JitCompiler();
            compiler.TargetFromContext = true;
            compiler.Tuning = tuning;

            var result = compiler.Compile(ptx);
            return new JittedKernel(result);
        }

        public static JittedKernel JitKernel(this String ptx, dim3 reqntid, HardwareIsa target)
        {
            ptx.AssertNotNull();
            CudaDriver.Ensure();

            var tuning = new JitTuning { Reqntid = reqntid };
            return ptx.JitKernel(tuning, target);
        }

        public static JittedKernel JitKernel(this String ptx, JitTuning tuning, HardwareIsa target)
        {
            ptx.AssertNotNull();
            CudaDriver.Ensure();

            var compiler = new JitCompiler();
            compiler.Target = target;
            compiler.Tuning = tuning;

            var result = compiler.Compile(ptx);
            return new JittedKernel(result);
        }
    }
}