using System;
using System.Diagnostics;
using Libcuda.Native;
using Libcuda.Native.DataTypes;
using Libcuda.Native.Exceptions;
using XenoGears.Traits.Disposable;

namespace Libcuda.Jit
{
    [DebuggerNonUserCode]
    public class JittedFunction : Disposable
    {
        public JittedModule Module { get; private set; }

        public String Name { get; private set; }
        internal CUfunction Handle { get; private set; }

        public int MaxThreadsPerBlock { get; private set; }
        public int SharedSizeBytes { get; private set; }
        public int ConstSizeBytes { get; private set; }
        public int LocalSizeBytes { get; private set; }
        public int NumRegs { get; private set; }
        public int PtxVersion { get; private set; }
        public int BinaryVersion { get; private set; }

        internal JittedFunction(JittedModule module, String name)
        {
            Module = module;
            Name = name;

            CUfunction h_function;
            var error1 = nvcuda.cuModuleGetFunction(out h_function, Module.Handle, name);
            if (error1 != CUresult.Success) throw new CudaException(error1);
            Handle = h_function;

            int maxThreadsPerBlock;
            var error2 = nvcuda.cuFuncGetAttribute(out maxThreadsPerBlock, CUfunction_attribute.MaxThreadsPerBlock, h_function);
            if (error2 != CUresult.Success) throw new CudaException(error2);
            MaxThreadsPerBlock = maxThreadsPerBlock;

            int sharedSizeBytes;
            var error3 = nvcuda.cuFuncGetAttribute(out sharedSizeBytes, CUfunction_attribute.SharedSizeBytes, h_function);
            if (error3 != CUresult.Success) throw new CudaException(error3);
            SharedSizeBytes = sharedSizeBytes;

            int constSizeBytes;
            var error4 = nvcuda.cuFuncGetAttribute(out constSizeBytes, CUfunction_attribute.ConstSizeBytes, h_function);
            if (error4 != CUresult.Success) throw new CudaException(error4);
            ConstSizeBytes = constSizeBytes;

            int localSizeBytes;
            var error5 = nvcuda.cuFuncGetAttribute(out localSizeBytes, CUfunction_attribute.LocalSizeBytes, h_function);
            if (error5 != CUresult.Success) throw new CudaException(error5);
            LocalSizeBytes = localSizeBytes;

            int numRegs;
            var error6 = nvcuda.cuFuncGetAttribute(out numRegs, CUfunction_attribute.NumRegs, h_function);
            if (error6 != CUresult.Success) throw new CudaException(error6);
            NumRegs = numRegs;

            int ptxVersion;
            var error7 = nvcuda.cuFuncGetAttribute(out ptxVersion, CUfunction_attribute.PtxVersion, h_function);
            if (error7 != CUresult.Success) throw new CudaException(error7);
            PtxVersion = ptxVersion;

            int binaryVersion;
            var error8 = nvcuda.cuFuncGetAttribute(out binaryVersion, CUfunction_attribute.BinaryVersion, h_function);
            if (error8 != CUresult.Success) throw new CudaException(error8);
            BinaryVersion = binaryVersion;

            // note. there's no necessity in unloading the function
            // it'll be unloaded together with the module
            SuppressDispose();
        }
    }
}