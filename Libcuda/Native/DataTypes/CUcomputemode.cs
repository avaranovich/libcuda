namespace Libcuda.Native.DataTypes
{
    // http://developer.download.nvidia.com/compute/cuda/3_1/toolkit/docs/online/group__CUDA__TYPES_g409cfd7e4863c34f8430757482886d75.html
    internal enum CUcomputemode
    {
        Default = 0,
        Exclusive = 1,
        Prohibited = 2,
    }
}