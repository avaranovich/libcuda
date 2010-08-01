using System;
using System.Diagnostics;
using Libcuda.Api.Native;
using Libcuda.Versions;
using XenoGears.Assertions;
using XenoGears.Versioning;

namespace Libcuda
{
    [DebuggerNonUserCode]
    public static class CudaVersions
    {
        public static Version Driver
        {
            get
            {
                CudaDriver.Ensure();
                return CudaDriver.Current.Version();
            }
        }

        public static CudaVersion Cuda
        {
            get
            {
                return (CudaVersion)nvcuda.cuDriverGetVersion();
            }
        }

        public static SoftwareIsa SoftwareIsa
        {
            get
            {
                switch (Cuda)
                {
                    case CudaVersion.CUDA_10:
                        return SoftwareIsa.PTX_10;
                    case CudaVersion.CUDA_11:
                        return SoftwareIsa.PTX_11;
                    case CudaVersion.CUDA_20:
                        return SoftwareIsa.PTX_12;
                    case CudaVersion.CUDA_21:
                        return SoftwareIsa.PTX_13;
                    case CudaVersion.CUDA_22:
                        return SoftwareIsa.PTX_14;
                    case CudaVersion.CUDA_23:
                        (Driver != null).AssertTrue();
                        var r = (Driver.Build % 10) * 100 + Driver.Revision / 100;
                        return r >= 190 ? SoftwareIsa.PTX_15 : SoftwareIsa.PTX_14;
                    case CudaVersion.CUDA_30:
                        return SoftwareIsa.PTX_20;
                    case CudaVersion.CUDA_31:
                        return SoftwareIsa.PTX_21;
                    default:
                        throw AssertionHelper.Fail();
                }
            }
        }

        public static HardwareIsa HardwareIsa
        {
            get
            {
                CudaDriver.Ensure();
                var device = CudaDevices.Current.AssertNotNull();
                return device.Caps.ComputeCaps;
            }
        }
    }
}