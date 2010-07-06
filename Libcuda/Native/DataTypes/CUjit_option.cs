namespace Libcuda.Native.DataTypes
{
    // http://developer.download.nvidia.com/compute/cuda/3_1/toolkit/docs/online/group__CUDA__TYPES_gfaa9995214a4f3341f48c5830cea0d8a.html
    internal enum CUjit_option
    {
        MaxRegisters = 0,
        ThreadsPerBlock = 1,
        WallTime = 2,
        InfoLogBuffer = 3,
        InfoLogBufferSizeBytes = 4,
        ErrorLogBuffer = 5,
        ErrorLogBufferSizeBytes = 6,
        OptimizationLevel = 7,
        TargetFromContext = 8,
        Target = 9,
        FallbackStrategy = 10,
    }
}