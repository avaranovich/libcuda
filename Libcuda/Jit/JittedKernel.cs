using System;
using System.Collections.Generic;
using System.Diagnostics;
using Libcuda.DataTypes;
using Libcuda.Run;
using XenoGears.Assertions;
using XenoGears.Traits.Disposable;

namespace Libcuda.Jit
{
    [DebuggerNonUserCode]
    public class JittedKernel : Disposable
    {
        public String Ptx { get; private set; }
        public JitResult Result { get; private set; }

        private JittedModule Module { get { return Result.Module; } }
        private JittedFunction Function { get { return Result.Functions.AssertSingle(); } }
        protected override void DisposeManagedResources() { Module.Dispose(); }

        public JittedKernel(String ptx, JitResult result)
        {
            Ptx = ptx;
            Result = result;
        }

        public KernelResult Run(dim3 gridDim, dim3 blockDim, params KernelArgument[] args)
        {
            return Run(gridDim, blockDim, (IEnumerable<KernelArgument>)args);
        }

        public KernelResult Run(dim3 gridDim, dim3 blockDim, IEnumerable<KernelArgument> args)
        {
            var invocation = new KernelInvocation(Function, args);
            return invocation.Launch(gridDim, blockDim);
        }
    }
}