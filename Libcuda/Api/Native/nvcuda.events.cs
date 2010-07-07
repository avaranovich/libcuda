using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Libcuda.Api.Native.DataTypes;
using Libcuda.Exceptions;

namespace Libcuda.Api.Native
{
    public static partial class nvcuda
    {
        [DllImport("nvcuda")]
        // http://developer.download.nvidia.com/compute/cuda/3_1/toolkit/docs/online/group__CUEVENT_g433317083f929b9298f8a88d57aa5017.html
        private static extern CUresult nativeEventCreate(out CUevent phEvent, CUevent_flags Flags);

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static CUevent cuEventCreate(CUevent_flags Flags)
        {
            try
            {
                CUevent hevent;
                var error = nativeEventCreate(out hevent, Flags);
                if (error != CUresult.CUDA_SUCCESS) throw new CudaException(error);
                return hevent;
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
        // http://developer.download.nvidia.com/compute/cuda/3_1/toolkit/docs/online/group__CUEVENT_g349006734f6e7378ea36cb57c239d4c7.html
        private static extern CUresult nativeEventDestroy(CUevent hEvent);

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void cuEventDestroy(CUevent hEvent)
        {
            try
            {
                var error = nativeEventDestroy(hEvent);
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
        //http://developer.download.nvidia.com/compute/cuda/3_1/toolkit/docs/online/group__CUEVENT_g93468fbdae4190b79926381a90a94301.html
        private static extern CUresult nativeEventRecord(CUevent hEvent, CUstream hStream);

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void cuEventRecord(CUevent hEvent)
        {
            cuEventRecord(hEvent, CUstream.Null);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void cuEventRecord(CUevent hEvent, CUstream hStream)
        {
            try
            {
                var error = nativeEventRecord(hEvent, hStream);
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
        // http://developer.download.nvidia.com/compute/cuda/3_1/toolkit/docs/online/group__CUEVENT_ge3ed6a308c602d139373895cb99cb7ab.html
        private static extern CUresult nativeEventSynchronize(CUevent hEvent);

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void cuEventSynchronize(CUevent hEvent)
        {
            try
            {
                var error = nativeEventSynchronize(hEvent);
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
        // http://developer.download.nvidia.com/compute/cuda/3_1/toolkit/docs/online/group__CUEVENT_g7895332c94680b174ef41373af09d9ce.html
        private static extern CUresult nativeEventElapsedTime(out float pMilliseconds, CUevent hStart, CUevent hEnd);

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static CUelapsed_time cuEventElapsedTime(CUevent hStart, CUevent hEnd)
        {
            try
            {
                float milliseconds;
                var error = nativeEventElapsedTime(out milliseconds, hStart, hEnd);
                if (error != CUresult.CUDA_SUCCESS) throw new CudaException(error);

                // note. cannot use TimeSpan here because it ain't work with fractions of milliseconds
                return new CUelapsed_time(milliseconds);
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
