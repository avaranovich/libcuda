using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Libcuda.Api.Native.DataTypes;
using Libcuda.Exceptions;

namespace Libcuda.Api.Native
{
    public static partial class nvcuda
    {
        [DllImport("nvcuda", EntryPoint = "cuInit")]
        // http://developer.download.nvidia.com/compute/cuda/3_1/toolkit/docs/online/group__CUINIT_g4703189f4c7f490c73f77942a3fa8443.html
        private static extern CUresult nativeInit(CUinit_flags Flags);

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static void cuInit(CUinit_flags Flags)
        {
            Wrap(() =>
            {
                try
                {
                    var error = nativeInit(Flags);
                    if (error != CUresult.CUDA_SUCCESS) throw new CudaException(error);
                }
                catch (CudaException)
                {
                    throw;
                }
                catch (DllNotFoundException dnfe)
                {
                    throw new CudaException(CudaError.NoDriver, dnfe);
                }
                catch (Exception e)
                {
                    throw new CudaException(CudaError.Unknown, e);
                }
            });
        }
    }
}