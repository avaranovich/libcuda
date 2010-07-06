using System.Runtime.InteropServices;
using Libcuda.Native.DataTypes;
using Libcuda.Native.Exceptions;

namespace Libcuda.Native
{
    internal static partial class nvcuda
    {
        [DllImport("nvcuda")]
        // http://developer.download.nvidia.com/compute/cuda/3_1/toolkit/docs/online/group__CUEVENT_g433317083f929b9298f8a88d57aa5017.html
        public static extern CUresult cuEventCreate(out CUevent phEvent, CUevent_flags Flags);
        [DllImport("nvcuda")]
        // http://developer.download.nvidia.com/compute/cuda/3_1/toolkit/docs/online/group__CUEVENT_g349006734f6e7378ea36cb57c239d4c7.html
        public static extern CUresult cuEventDestroy(CUevent hEvent);

        [DllImport("nvcuda")]
        //http://developer.download.nvidia.com/compute/cuda/3_1/toolkit/docs/online/group__CUEVENT_g93468fbdae4190b79926381a90a94301.html
        public static extern CUresult cuEventRecord(CUevent hEvent, CUstream hStream);
        [DllImport("nvcuda")]
        // http://developer.download.nvidia.com/compute/cuda/3_1/toolkit/docs/online/group__CUEVENT_ge3ed6a308c602d139373895cb99cb7ab.html
        public static extern CUresult cuEventSynchronize(CUevent hEvent);
        [DllImport("nvcuda")]
        // http://developer.download.nvidia.com/compute/cuda/3_1/toolkit/docs/online/group__CUEVENT_g7895332c94680b174ef41373af09d9ce.html
        public static extern CUresult cuEventElapsedTime(out float pMilliseconds, CUevent hStart, CUevent hEnd);
    }
}
