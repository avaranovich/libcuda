namespace Libcuda.Api.Native.DataTypes
{
    // http://developer.download.nvidia.com/compute/cuda/3_1/toolkit/docs/online/group__CUDA__TYPES_gb67889469b7e66d222f5dbe69bbfdf36.html
    public enum CUfunction_attribute
    {
        MaxThreadsPerBlock = 0,
        SharedSizeBytes = 1,
        ConstSizeBytes = 2,
        LocalSizeBytes = 3,
        NumRegs = 4,
        PtxVersion = 5,
        BinaryVersion = 6,
    }
}