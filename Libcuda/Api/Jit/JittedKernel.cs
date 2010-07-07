using System;
using System.Diagnostics;
using Libcuda.Api.Native.DataTypes;
using XenoGears.Assertions;
using XenoGears.Traits.Disposable;

namespace Libcuda.Api.Jit
{
    [DebuggerNonUserCode]
    public class JittedKernel : Disposable
    {
        public JitResult Result { get; private set; }
        public String Ptx { get { return Result.Ptx; } }

        public JittedModule Module { get { return Result.Module; } }
        public JittedFunction Function { get { return Result.Functions.AssertSingle(); } }
        public static implicit operator CUfunction(JittedKernel kernel) { return kernel == null ? CUfunction.Null : kernel.Function; }
        public static implicit operator CUmodule(JittedKernel kernel) { return kernel == null ? CUmodule.Null : kernel.Module; }
        protected override void DisposeManagedResources() { Module.Dispose(); }

        public JittedKernel(JitResult result)
        {
            Result = result.AssertNotNull();
        }
    }
}