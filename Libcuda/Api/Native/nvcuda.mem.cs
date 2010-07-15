using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Libcuda.Api.Native.DataTypes;
using Libcuda.Exceptions;

namespace Libcuda.Api.Native
{
    public static partial class nvcuda
    {
        [DllImport("nvcuda", EntryPoint = "cuMemAlloc")]
        // http://developer.download.nvidia.com/compute/cuda/3_1/toolkit/docs/online/group__CUMEM_g5f743cd3c24032f3ce9bd1019b8e769b.html
        private static extern CUresult nativeMemAlloc(out CUdeviceptr dptr, uint bytesize);

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static CUdeviceptr cuMemAlloc(uint bytesize)
        {
            return Wrap(() =>
            {
                try
                {
                    CUdeviceptr dptr;
                    var error = nativeMemAlloc(out dptr, bytesize);
                    if (error != CUresult.CUDA_SUCCESS) throw new CudaException(error);
                    return dptr;
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

        [DllImport("nvcuda", EntryPoint = "cuMemFree")]
        // http://developer.download.nvidia.com/compute/cuda/3_1/toolkit/docs/online/group__CUMEM_g94c2bc68edecc4d7ba3be3805f27ebc0.html
        private static extern CUresult nativeMemFree(CUdeviceptr dptr);

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void cuMemFree(CUdeviceptr dptr)
        {
            Wrap(() =>
            {
                try
                {
                    var error = nativeMemFree(dptr);
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

        [DllImport("nvcuda", EntryPoint = "cuMemcpyHtoD")]
        // http://developer.download.nvidia.com/compute/cuda/3_1/toolkit/docs/online/group__CUMEM_g7104efead2137b6551f7cf1c13d51a67.html
        private static extern CUresult nativeMemcpyHtoD(CUdeviceptr dstDevice, IntPtr srcHost, uint ByteCount);

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void cuMemcpyHtoD(CUdeviceptr dstDevice, IntPtr srcHost, uint ByteCount)
        {
            Wrap(() =>
            {
                try
                {
                    var error = nativeMemcpyHtoD(dstDevice, srcHost, ByteCount);
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

        [DllImport("nvcuda", EntryPoint = "cuMemcpyDtoH")]
        // http://developer.download.nvidia.com/compute/cuda/3_1/toolkit/docs/online/group__CUMEM_g7e0dc519d430790de8f4793cbafe3e15.html
        private static extern CUresult nativeMemcpyDtoH(IntPtr dstHost, CUdeviceptr srcDevice, uint ByteCount);

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void cuMemcpyDtoH(IntPtr dstHost, CUdeviceptr srcDevice, uint ByteCount)
        {
            Wrap(() =>
            {
                try
                {
                    var error = nativeMemcpyDtoH(dstHost, srcDevice, ByteCount);
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