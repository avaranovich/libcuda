namespace Libcuda.Native.DataTypes
{
    // http://developer.download.nvidia.com/compute/cuda/3_1/toolkit/docs/online/group__CUDA__TYPES_g82ae27b817e5d8cc95045a5634f671d7.html
    internal enum CUjit_fallbackstrategy
    {
        PreferPtx = 0,
        PreferBinary = 1,
    }
}