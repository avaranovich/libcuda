using System;
using System.Runtime.InteropServices;
using Libcuda.Native.DataTypes;
using Libcuda.Native.Exceptions;

namespace Libcuda.Native
{
    internal static partial class nvcuda
    {
        [DllImport("nvcuda")]
        // http://developer.download.nvidia.com/compute/cuda/3_1/toolkit/docs/online/group__CUMODULE_ge18a9f0d853ae3a96a38416a0671606b.html
        public static extern CUresult cuModuleGetFunction(out CUfunction hfunc, CUmodule hmod, String name);
        [DllImport("nvcuda")]
        // http://developer.download.nvidia.com/compute/cuda/3_1/toolkit/docs/online/group__CUMODULE_gbfbf77eb2a307af8aa81376ecc909bd3.html
        public static extern CUresult cuModuleLoadDataEx(out CUmodule module, IntPtr image, uint numOptions, CUjit_option[] options, IntPtr optionValues);
        [DllImport("nvcuda")]
        // http://developer.download.nvidia.com/compute/cuda/3_1/toolkit/docs/online/group__CUMODULE_g35a621d73ee186733f051de672fbe02b.html
        public static extern CUresult cuModuleUnload(CUmodule mod);
    }
}