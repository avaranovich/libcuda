using System;
using System.Runtime.InteropServices;
using Libcuda.Native.DataTypes;
using Libcuda.Native.Exceptions;

namespace Libcuda.Native
{
    internal static partial class nvcuda
    {
        [DllImport("nvcuda")]
        // http://developer.download.nvidia.com/compute/cuda/3_1/toolkit/docs/online/group__CUMEM_g5f743cd3c24032f3ce9bd1019b8e769b.html
        public static extern CUresult cuMemAlloc(out CUdeviceptr dptr, uint bytesize);
        [DllImport("nvcuda")]
        // http://developer.download.nvidia.com/compute/cuda/3_1/toolkit/docs/online/group__CUMEM_g94c2bc68edecc4d7ba3be3805f27ebc0.html
        public static extern CUresult cuMemFree(CUdeviceptr dptr);
        [DllImport("nvcuda")]
        // http://developer.download.nvidia.com/compute/cuda/3_1/toolkit/docs/online/group__CUMEM_g7104efead2137b6551f7cf1c13d51a67.html
        public static extern CUresult cuMemcpyHtoD(CUdeviceptr dstDevice, IntPtr srcHost, uint ByteCount);
        [DllImport("nvcuda")]
        // http://developer.download.nvidia.com/compute/cuda/3_1/toolkit/docs/online/group__CUMEM_g7e0dc519d430790de8f4793cbafe3e15.html
        public static extern CUresult cuMemcpyDtoH(IntPtr dstHost, CUdeviceptr srcDevice, uint ByteCount);
    }
}