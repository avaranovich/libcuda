namespace Libcuda.Api.Native.DataTypes
{
    // http://developer.download.nvidia.com/compute/cuda/3_1/toolkit/docs/online/group__CUDA__TYPES_g5d731dfd360f2a68ae45a4df46089af4.html
    public enum CUfunc_cache
    {
        PreferNone = 0,
        PreferShared = 1,
        PreferL1 = 2,
    }
}