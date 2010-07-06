using System;
using System.Diagnostics;
using System.IO;
using Libcuda.Native;
using Libcuda.Native.Exceptions;
using Libcuda.Versions;
using XenoGears.Assertions;

namespace Libcuda
{
    [DebuggerNonUserCode]
    public static class CudaVersions
    {
        public static Version Driver
        {
            get
            {
                var system_dir = Environment.GetFolderPath(Environment.SpecialFolder.System);
                var nvcuda_dll = Path.Combine(system_dir, "nvcuda.dll");
                if (!File.Exists(nvcuda_dll)) return null;

                var fvi = FileVersionInfo.GetVersionInfo(nvcuda_dll);
                return new Version(fvi.FileMajorPart, fvi.FileMinorPart, fvi.FileBuildPart, fvi.FilePrivatePart);
            }
        }

        public static CudaVersion Cuda
        {
            get
            {
                try
                {
                    int version;
                    var error = nvcuda.cuDriverGetVersion(out version);
                    if (error != CUresult.Success) throw new CudaException(error);

                    return (CudaVersion)version;
                }
                catch (DllNotFoundException)
                {
                    return 0;
                }
            }
        }

        public static SoftwareIsa SoftwareIsa
        {
            get
            {
                var r = (Driver.Build % 10) * 100 + Driver.Revision / 100;
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
                var device = CudaDevices.Current;
                return device == null ? 0 : device.Caps.ComputeCaps;
            }
        }
    }
}