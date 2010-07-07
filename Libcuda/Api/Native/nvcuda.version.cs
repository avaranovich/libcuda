using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Libcuda.Api.Native.DataTypes;
using Libcuda.Exceptions;

namespace Libcuda.Api.Native
{
    public static partial class nvcuda
    {
        [DllImport("nvcuda", EntryPoint = "cuDriverGetVersion")]
        // http://developer.download.nvidia.com/compute/cuda/3_1/toolkit/docs/online/group__CUVERSION_gf83e088e9433ce2e9ce87203791dd122.html
        private static extern CUresult nativeDriverGetVersion(out int version);

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static int cuDriverGetVersion()
        {
            try
            {
                int version;
                var error = nativeDriverGetVersion(out version);
                if (error != CUresult.CUDA_SUCCESS) throw new CudaException(error);
                return version;
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
