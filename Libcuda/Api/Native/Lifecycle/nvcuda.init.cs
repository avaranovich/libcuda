using System.Runtime.InteropServices;
using Libcuda.Api.Native.DataTypes;
using Libcuda.Exceptions;
using Libcuda.Versions;
using XenoGears.Assertions;

namespace Libcuda.Api.Native
{
    public static partial class nvcuda
    {
        [DllImport("nvcuda", EntryPoint = "cuInit")]
        // http://developer.download.nvidia.com/compute/cuda/3_1/toolkit/docs/online/group__CUINIT_g4703189f4c7f490c73f77942a3fa8443.html
        private static extern CUresult nativeInit(CUinit_flags Flags);

        private static void InitializeDriver()
        {
            Log.WriteLine("Dynamically linking to CUDA driver...");
            var cudaVersion = CudaVersions.Cuda;
            if (cudaVersion == 0)
            {
                Log.WriteLine("CUDA driver not found!");
                throw new CudaException(CudaError.NoDriver);
            }
            else
            {
                (cudaVersion >= CudaVersion.CUDA_31).AssertTrue();
                Log.WriteLine("Successfully linked to {0} v{1} (CUDA {2}.{3}).",
                    CudaDriver.Name,
                    CudaDriver.Version,
                    (int)cudaVersion / 1000, (int)cudaVersion % 100);
                Log.WriteLine();
            }

            Log.WriteLine("Initializing CUDA driver...");
            Wrap(() =>
            {
                var error = nativeInit(CUinit_flags.None);
                if (error != CUresult.CUDA_SUCCESS) throw new CudaException(error);
            });

            Log.WriteLine("Success.");
            Log.WriteLine();
        }
    }
}