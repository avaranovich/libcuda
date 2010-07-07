using System.Diagnostics;
using Libcuda.Versions;
using XenoGears.Assertions;

namespace Libcuda.Api.Native.DataTypes
{
    // http://developer.download.nvidia.com/compute/cuda/3_1/toolkit/docs/online/group__CUDA__TYPES_g63f8302653e377f3dbb4b4c4a6ba0283.html
    public enum CUjit_target
    {
        Compute10 = 0,
        Compute11 = 1,
        Compute12 = 2,
        Compute13 = 3,
        Compute20 = 4,
    }

    [DebuggerNonUserCode]
    public static class CUjit_targetExtensions
    {
        public static CUjit_target ToCUjit_target(this HardwareIsa hardwareIsa)
        {
            switch (hardwareIsa)
            {
                case HardwareIsa.SM_10:
                    return CUjit_target.Compute10;
                case HardwareIsa.SM_11:
                    return CUjit_target.Compute11;
                case HardwareIsa.SM_12:
                    return CUjit_target.Compute12;
                case HardwareIsa.SM_13:
                    return CUjit_target.Compute13;
                case HardwareIsa.SM_20:
                    return CUjit_target.Compute20;
                default:
                    throw AssertionHelper.Fail();
            }
        }
    }
}