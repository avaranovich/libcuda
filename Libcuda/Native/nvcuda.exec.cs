using System;
using System.Runtime.InteropServices;
using Libcuda.Native.DataTypes;
using Libcuda.Native.Exceptions;

namespace Libcuda.Native
{
    internal static partial class nvcuda
    {
        [DllImport("nvcuda")]
        // http://developer.download.nvidia.com/compute/cuda/3_1/toolkit/docs/online/group__CUEXEC_g5cfb58eb90f8b45b0db50bd1321b321a.html
        public static extern CUresult cuParamSeti(CUfunction hfunc, int offset, uint value);
        [DllImport("nvcuda")]
        // http://developer.download.nvidia.com/compute/cuda/3_1/toolkit/docs/online/group__CUEXEC_g27981d94ac79ac7ebe8590a858785b3b.html
        public static extern CUresult cuParamSetf(CUfunction hfunc, int offset, float value);
        [DllImport("nvcuda")]
        // http://developer.download.nvidia.com/compute/cuda/3_1/toolkit/docs/online/group__CUEXEC_gf4466f8fe7043de8c97137990bb2e1e9.html
        public static extern CUresult cuParamSetv(CUfunction hfunc, int offset, IntPtr ptr, uint numbytes);
        [DllImport("nvcuda")]
        // http://developer.download.nvidia.com/compute/cuda/3_1/toolkit/docs/online/group__CUEXEC_g1946eac8c4d9d74f6aba01b0aab2c3dd.html
        public static extern CUresult cuParamSetSize(CUfunction hfunc, uint numbytes);

        [DllImport("nvcuda")]
        // http://developer.download.nvidia.com/compute/cuda/3_1/toolkit/docs/online/group__CUEXEC_g7bde0e4a4ce32ce7460348e01a91f45f.html
        public static extern CUresult cuFuncGetAttribute(out int pi, CUfunction_attribute attrib, CUfunction hfunc);

        [DllImport("nvcuda")]
        // http://developer.download.nvidia.com/compute/cuda/3_1/toolkit/docs/online/group__CUEXEC_g65bc1dcf1127400d8f7ff935edbd333c.html
        public static extern CUresult cuFuncSetSharedSize(CUfunction hfunc, uint bytes);
        [DllImport("nvcuda")]
        // http://developer.download.nvidia.com/compute/cuda/3_1/toolkit/docs/online/group__CUEXEC_g91d75e10ed90df3fd3ecf2488f2cb27f.html
        public static extern CUresult cuFuncSetCacheConfig(CUfunction hfunc, CUfunc_cache config);
        [DllImport("nvcuda")]
        // http://developer.download.nvidia.com/compute/cuda/3_1/toolkit/docs/online/group__CUEXEC_g3196abfe0d52f6806eced043ea1e1fb4.html
        public static extern CUresult cuFuncSetBlockShape(CUfunction hfunc, int x, int y, int z);
        [DllImport("nvcuda")]
        // http://developer.download.nvidia.com/compute/cuda/3_1/toolkit/docs/online/group__CUEXEC_g1a99f308b2f1655df734eb62452fafd3.html
        public static extern CUresult cuLaunchGrid(CUfunction f, int grid_width, int grid_height);
    }
}