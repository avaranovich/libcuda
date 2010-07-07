using System.Collections.Generic;
using System.Diagnostics;
using Libcuda.Api.Jit;
using Libcuda.Api.Native.DataTypes;
using Libcuda.Api.Run;
using Libcuda.DataTypes;

namespace Libcuda
{
    // todo. think better about run API
    // currently it's very tedious to use it correctly (i.e. to guarantee zero leaks)

    [DebuggerNonUserCode]
    public static class RunFacade
    {
        public static KernelResult Run(this JittedKernel kernel, dim3 gridDim, dim3 blockDim, params KernelArgument[] args)
        {
            return kernel.Run(gridDim, blockDim, (IEnumerable<KernelArgument>)args);
        }

        public static KernelResult Run(this JittedKernel kernel, dim3 gridDim, dim3 blockDim, IEnumerable<KernelArgument> args)
        {
            var invocation = new KernelInvocation(kernel.Function, args);
            return invocation.Launch(gridDim, blockDim);
        }

        public static KernelResult Run(this JittedFunction function, dim3 gridDim, dim3 blockDim, params KernelArgument[] args)
        {
            return function.Run(gridDim, blockDim, (IEnumerable<KernelArgument>)args);
        }

        public static KernelResult Run(this JittedFunction function, dim3 gridDim, dim3 blockDim, IEnumerable<KernelArgument> args)
        {
            var invocation = new KernelInvocation(function, args);
            return invocation.Launch(gridDim, blockDim);
        }

        public static KernelResult Run(this CUfunction function, dim3 gridDim, dim3 blockDim, params KernelArgument[] args)
        {
            return function.Run(gridDim, blockDim, (IEnumerable<KernelArgument>)args);
        }

        public static KernelResult Run(this CUfunction function, dim3 gridDim, dim3 blockDim, IEnumerable<KernelArgument> args)
        {
            var invocation = new KernelInvocation((JittedFunction)function, args);
            return invocation.Launch(gridDim, blockDim);
        }
    }
}