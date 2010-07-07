using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Libcuda.Api.Devices;
using Libcuda.Api.Native.DataTypes;
using Libcuda.DataTypes;
using Libcuda.Exceptions;
using XenoGears.Assertions;

namespace Libcuda.Api.Native
{
    public static partial class nvcuda
    {
        [DllImport("nvcuda")]
        // http://developer.download.nvidia.com/compute/cuda/3_1/toolkit/docs/online/group__CUEXEC_g5cfb58eb90f8b45b0db50bd1321b321a.html
        private static extern CUresult nativeParamSeti(CUfunction hfunc, int offset, uint value);

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void cuParamSeti(CUfunction hfunc, int offset, uint value)
        {
            try
            {
                var error = nativeParamSeti(hfunc, offset, value);
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
        }

        [DllImport("nvcuda")]
        // http://developer.download.nvidia.com/compute/cuda/3_1/toolkit/docs/online/group__CUEXEC_g27981d94ac79ac7ebe8590a858785b3b.html
        private static extern CUresult nativeParamSetf(CUfunction hfunc, int offset, float value);

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void cuParamSetf(CUfunction hfunc, int offset, float value)
        {
            try
            {
                var error = nativeParamSetf(hfunc, offset, value);
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
        }

        [DllImport("nvcuda")]
        // http://developer.download.nvidia.com/compute/cuda/3_1/toolkit/docs/online/group__CUEXEC_gf4466f8fe7043de8c97137990bb2e1e9.html
        private static extern CUresult nativeParamSetv(CUfunction hfunc, int offset, IntPtr ptr, uint numbytes);

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void cuParamSetv(CUfunction hfunc, int offset, Object obj)
        {
            try
            {
                obj.AssertNotNull().GetType().IsValueType.AssertTrue();
                var size = Marshal.SizeOf(obj);
                var ptr = Marshal.AllocHGlobal(size);

                try
                {
                    Marshal.StructureToPtr(obj, ptr, false);
                    var error = nativeParamSetv(hfunc, offset, ptr, (uint)size);
                    if (error != CUresult.CUDA_SUCCESS) throw new CudaException(error);
                }
                finally
                {
                    Marshal.FreeHGlobal(ptr);
                }
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
        }

        [DllImport("nvcuda")]
        // http://developer.download.nvidia.com/compute/cuda/3_1/toolkit/docs/online/group__CUEXEC_g1946eac8c4d9d74f6aba01b0aab2c3dd.html
        private static extern CUresult nativeParamSetSize(CUfunction hfunc, uint numbytes);

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void cuParamSetSize(CUfunction hfunc, uint numbytes)
        {
            try
            {
                var error = nativeParamSetSize(hfunc, numbytes);
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
        }

        [DllImport("nvcuda")]
        // http://developer.download.nvidia.com/compute/cuda/3_1/toolkit/docs/online/group__CUEXEC_g7bde0e4a4ce32ce7460348e01a91f45f.html
        private static extern CUresult nativeFuncGetAttribute(out int pi, CUfunction_attribute attrib, CUfunction hfunc);

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static int cuFuncGetAttribute(CUfunction_attribute attrib, CUfunction hfunc)
        {
            try
            {
                int i;
                var error = nativeFuncGetAttribute(out i, attrib, hfunc);
                if (error != CUresult.CUDA_SUCCESS) throw new CudaException(error);
                return i;
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
        }

        [DllImport("nvcuda")]
        // http://developer.download.nvidia.com/compute/cuda/3_1/toolkit/docs/online/group__CUEXEC_g65bc1dcf1127400d8f7ff935edbd333c.html
        private static extern CUresult nativeFuncSetSharedSize(CUfunction hfunc, uint bytes);

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void cuFuncSetSharedSize(CUfunction hfunc, uint bytes)
        {
            try
            {
                var error = nativeFuncSetSharedSize(hfunc, bytes);
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
        }

        [DllImport("nvcuda")]
        // http://developer.download.nvidia.com/compute/cuda/3_1/toolkit/docs/online/group__CUEXEC_g91d75e10ed90df3fd3ecf2488f2cb27f.html
        private static extern CUresult nativeFuncSetCacheConfig(CUfunction hfunc, CUfunc_cache config);

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void cuFuncSetCacheConfig(CUfunction hfunc, CUfunc_cache config)
        {
            try
            {
                var error = nativeFuncSetCacheConfig(hfunc, config);
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
        }

        [DllImport("nvcuda")]
        // http://developer.download.nvidia.com/compute/cuda/3_1/toolkit/docs/online/group__CUEXEC_g3196abfe0d52f6806eced043ea1e1fb4.html
        private static extern CUresult nativeFuncSetBlockShape(CUfunction hfunc, int x, int y, int z);

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void cuFuncSetBlockShape(CUfunction hfunc, dim3 dim)
        {
            cuFuncSetBlockShape(hfunc, dim.X, dim.Y, dim.Z);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void cuFuncSetBlockShape(CUfunction hfunc, int x, int y, int z)
        {
            try
            {
                // if we don't verify this here, we'll get a strange message from the driver
                var caps = CudaDevice.Current.Caps.GridCaps;
                (caps.MaxBlockDim >= new dim3(x, y, z)).AssertTrue();

                var error = nativeFuncSetBlockShape(hfunc, x, y, z);
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
        }

        [DllImport("nvcuda")]
        // http://developer.download.nvidia.com/compute/cuda/3_1/toolkit/docs/online/group__CUEXEC_g1a99f308b2f1655df734eb62452fafd3.html
        private static extern CUresult nativeLaunchGrid(CUfunction f, int grid_width, int grid_height);

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void cuLaunchGrid(CUfunction f, dim3 dim)
        {
            // wow here we ain't able to specify the Z dimension
            (dim.Z == 1).AssertTrue();
            cuLaunchGrid(f, dim.X, dim.Y);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void cuLaunchGrid(CUfunction f, int grid_width, int grid_height)
        {
            try
            {
                // if we don't verify this here, we'll get a strange message from the driver
                var caps = CudaDevice.Current.Caps.GridCaps;
                (caps.MaxGridDim >= new dim3(grid_width, grid_width)).AssertTrue();

                var error = nativeLaunchGrid(f, grid_width, grid_height);
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
        }
    }
}