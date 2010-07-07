using System;

namespace Libcuda.Api.Native.DataTypes
{
    [Flags]
    // http://developer.download.nvidia.com/compute/cuda/3_1/toolkit/docs/online/group__CUDA__TYPES_g12d89ce3fea2678bf187aa2876ed67a6.html#g12d89ce3fea2678bf187aa2876ed67a6
    public enum CUctx_flags
    {
        None = 0,
        SchedAuto = 0,
        SchedSpin = 1,
        SchedYield = 2,
        BlockingSync = 4,
        MapHost = 8,
        LmemResizeToMax = 16,
    }
}