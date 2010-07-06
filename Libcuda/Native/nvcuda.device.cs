using System.Runtime.InteropServices;
using System.Text;
using Libcuda.Native.DataTypes;
using Libcuda.Native.Exceptions;

namespace Libcuda.Native
{
    internal static partial class nvcuda
    {
        [DllImport("nvcuda")]
        // http://developer.download.nvidia.com/compute/cuda/3_1/toolkit/docs/online/group__CUDEVICE_g09377966b78423897495d7ee6816ab17.html
        public static extern CUresult cuDeviceGetCount(out int count);
        [DllImport("nvcuda")]
        // http://developer.download.nvidia.com/compute/cuda/3_1/toolkit/docs/online/group__CUDEVICE_g0864f76e0c101311b2e1eb5c40a54ea9.html
        public static extern CUresult cuDeviceGet(out CUdevice device, int ordinal);

        [DllImport("nvcuda", CharSet = CharSet.Ansi)]
        // http://developer.download.nvidia.com/compute/cuda/3_1/toolkit/docs/online/group__CUDEVICE_gea189b5fe198ceb909f2a8bc3188e36f.html
        public static extern CUresult cuDeviceGetName(StringBuilder name, int len, CUdevice dev);
        [DllImport("nvcuda")]
        // http://developer.download.nvidia.com/compute/cuda/3_1/toolkit/docs/online/group__CUDEVICE_g2f36c6412efa2b6b89feefa233fb7519.html
        public static extern CUresult cuDeviceComputeCapability(out int major, out int minor, CUdevice dev);
        [DllImport("nvcuda")]
        // http://developer.download.nvidia.com/compute/cuda/3_1/toolkit/docs/online/group__CUDEVICE_ge283d0251a80fe5a82ec8f6e552eb248.html
        public static extern CUresult cuDeviceGetAttribute(out int pi, CUdevice_attribute attrib, CUdevice dev);
        [DllImport("nvcuda")]
        // http://developer.download.nvidia.com/compute/cuda/3_1/toolkit/docs/online/group__CUDEVICE_g7ff0e9bbf5268a053a77e7063b8a6bec.html
        public static extern CUresult cuDeviceTotalMem(out uint bytes, CUdevice dev);
    }
}
