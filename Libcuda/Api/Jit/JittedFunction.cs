using System;
using System.Collections.Generic;
using Libcuda.Api.Native;
using Libcuda.Api.Native.DataTypes;
using Libcuda.Api.Run;
using Libcuda.DataTypes;
using Libcuda.Versions;
using XenoGears.Assertions;
using XenoGears.Traits.Disposable;
using XenoGears.Functional;

namespace Libcuda.Api.Jit
{
    public class JittedFunction : Disposable
    {
        public String Name { get; private set; }
        public CUfunction Handle { get; private set; }
        public static implicit operator CUfunction(JittedFunction fun) { return fun == null ? CUfunction.Null : fun.Handle; }
        public static explicit operator JittedFunction(CUfunction fun) { return fun.IsNull ? null : new JittedFunction(fun); }
        public override String ToString() { var emptyName = Name.IsNullOrEmpty() || Name == "N/A"; return emptyName ? Handle.ToString() : String.Format("{0} ({1})", Name, Handle); }

        public int MaxThreadsPerBlock { get; private set; }
        public int SharedSizeBytes { get; private set; }
        public int ConstSizeBytes { get; private set; }
        public int LocalSizeBytes { get; private set; }
        public int NumRegs { get; private set; }
        public HardwareIsa PtxVersion { get; private set; }
        public HardwareIsa BinaryVersion { get; private set; }

        public JittedFunction(CUfunction handle)
            : this(handle, null)
        {
        }

        public JittedFunction(CUfunction handle, String name)
        {
            CudaDriver.Ensure();
            Handle = handle.AssertThat(h => h.IsNotNull);
            Name = name ?? "N/A";

            MaxThreadsPerBlock = nvcuda.cuFuncGetAttribute(CUfunction_attribute.MaxThreadsPerBlock, this);
            SharedSizeBytes = nvcuda.cuFuncGetAttribute(CUfunction_attribute.SharedSizeBytes, this);
            ConstSizeBytes = nvcuda.cuFuncGetAttribute(CUfunction_attribute.ConstSizeBytes, this);
            LocalSizeBytes = nvcuda.cuFuncGetAttribute(CUfunction_attribute.LocalSizeBytes, this);
            NumRegs = nvcuda.cuFuncGetAttribute(CUfunction_attribute.NumRegs, this);
            PtxVersion = (HardwareIsa)nvcuda.cuFuncGetAttribute(CUfunction_attribute.PtxVersion, this);
            BinaryVersion = (HardwareIsa)nvcuda.cuFuncGetAttribute(CUfunction_attribute.BinaryVersion, this);

            // note. there's no necessity in unloading the function
            // it'll be unloaded together with the module
            GC.SuppressFinalize(this);
        }

        public KernelResult Run(dim3 gridDim, dim3 blockDim, params KernelArgument[] args)
        {
            return Run(gridDim, blockDim, (IEnumerable<KernelArgument>)args);
        }

        public KernelResult Run(dim3 gridDim, dim3 blockDim, IEnumerable<KernelArgument> args)
        {
            var invocation = new KernelInvocation(this, args);
            return invocation.Launch(gridDim, blockDim);
        }
    }
}