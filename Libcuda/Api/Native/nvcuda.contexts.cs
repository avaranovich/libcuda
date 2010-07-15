using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Libcuda.Api.Native.DataTypes;
using Libcuda.Exceptions;

namespace Libcuda.Api.Native
{
    public static partial class nvcuda
    {
        [DllImport("nvcuda", EntryPoint = "cuCtxCreate")]
        // http://developer.download.nvidia.com/compute/cuda/3_1/toolkit/docs/online/group__CUCTX_g02b31a192b32043c0787696292f1ffbe.html
        private static extern CUresult nativeCtxCreate(out CUcontext pctx, CUctx_flags flags, CUdevice dev);

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static CUcontext cuCtxCreate(CUctx_flags flags, CUdevice dev)
        {
            return Wrap(() =>
            {
                try
                {
                    CUcontext ctx;
                    var error = nativeCtxCreate(out ctx, flags, dev);
                    if (error != CUresult.CUDA_SUCCESS) throw new CudaException(error);
                    return ctx;
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

        [DllImport("nvcuda", EntryPoint = "cuCtxDestroy")]
        // http://developer.download.nvidia.com/compute/cuda/3_1/toolkit/docs/online/group__CUCTX_g23bf81c24c28be3495fec41146f9e025.html
        private static extern CUresult nativeCtxDestroy(CUcontext ctx);

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static void cuCtxDestroy(CUcontext ctx)
        {
            Wrap(() =>
            {
                try
                {
                    var error = nativeCtxDestroy(ctx);
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
