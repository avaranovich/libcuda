using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using Libcuda.Api.Devices;
using Libcuda.Api.Native.DataTypes;
using Libcuda.Exceptions;
using Libcuda.Versions;
using XenoGears.Assertions;
using XenoGears.Logging;
using XenoGears.Threading;

namespace Libcuda.Api.Native
{
    public static partial class nvcuda
    {
        // todo. so far we use a single context to launch kernel invocations
        // this context is global to all users of this assembly instance
        // and has a lifetime of the entire application (this is created on-demand)
        //
        // in future it might be necessary to support different contexts
        // e.g. to use multiple devices or to service multiple clients at once
        // but for now I use the simplest option possible

        private static GlobalContext _ctx;
        private static int _affinity;
        static nvcuda()
        {
            using (NativeThread.Affinity(out _affinity))
            {
                _ctx = new GlobalContext();
            }
        }

        [DebuggerNonUserCode]
        private class GlobalContext : CriticalFinalizerObject
        {
            private readonly CUcontext _ctx;
            public static implicit operator CUcontext(GlobalContext ctx) { return ctx == null ? CUcontext.Null : ctx._ctx; }

            public GlobalContext()
            {
                Log.TraceLine("Dynamically linking to CUDA driver...");
                var cudaVersion = CudaVersions.Cuda;
                if (cudaVersion != 0)
                {
                    Log.TraceLine("CUDA driver not found!");
                    throw new CudaException(CudaError.NoDriver);
                }
                else
                {
                    (cudaVersion >= CudaVersion.CUDA_31).AssertTrue();
                    Log.TraceLine("Successfully linked to {0} {1} (CUDA {1}.{2}).",
                        CudaDriver.Name, CudaDriver.Version, (int)cudaVersion / 1000, (int)cudaVersion % 100);
                    Log.TraceLine();
                }

                Log.TraceLine("Initializing CUDA driver...");
                cuInit(CUinit_flags.None);
                Log.TraceLine("Success.");
                Log.TraceLine();

                // Step 3. Verify that we've got at least one CUDA capable device
                Log.TraceLine("Acquiring number of CUDA-capable devices...");
                var deviceCount = cuDeviceGetCount();
                Log.TraceLine("{0} device(s) found.", cuDeviceGetCount());
                (deviceCount > 0).AssertTrue();

                // Step 4. Get the device (only 1 device is supported at the moment)
                Log.TraceLine("Accessing device #0...");
                var device = CudaDevice.First;
                Log.TraceLine("Success.");
                Log.TraceLine(Environment.NewLine + device);

                // Step 5. Create the context - kernel execution environment
                Log.TraceLine("Creating CUDA context on device #0...");
                _ctx = cuCtxCreate(CUctx_flags.None, device.Handle);
                Log.TraceLine("Success.");
                Log.TraceLine(String.Empty);
            }

            ~GlobalContext()
            {
                if (_ctx.Handle != IntPtr.Zero)
                {
                    // todo. here we've got a resource leak
                    // finalizers cannot call nvcuda, since they run on another thread
                    cuCtxDestroy(_ctx);
                }
            }
        }

        [DllImport("nvcuda")]
        // http://developer.download.nvidia.com/compute/cuda/3_1/toolkit/docs/online/group__CUCTX_g02b31a192b32043c0787696292f1ffbe.html
        private static extern CUresult nativeCtxCreate(out CUcontext pctx, CUctx_flags flags, CUdevice dev);

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static CUcontext cuCtxCreate(CUctx_flags flags, CUdevice dev)
        {
            try
            {
                CUcontext ctx;
                var error = nativeCtxCreate(out ctx, flags, dev);
                if (error != CUresult.CUDA_SUCCESS) throw new CudaException(error);
                return ctx;
            }
            catch (CudaException)
            {
                throw;
            }
            catch (DllNotFoundException dnfe)
            {
                throw new CudaException(CudaError.NoDriver, dnfe);
            }
            catch (Exception e)
            {
                throw new CudaException(CudaError.Unknown, e);
            }
        }

        [DllImport("nvcuda")]
        // http://developer.download.nvidia.com/compute/cuda/3_1/toolkit/docs/online/group__CUCTX_g23bf81c24c28be3495fec41146f9e025.html
        private static extern CUresult nativeCtxDestroy(CUcontext ctx);

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static void cuCtxDestroy(CUcontext ctx)
        {
            try
            {
                var error = nativeCtxDestroy(ctx);
                if (error != CUresult.CUDA_SUCCESS) throw new CudaException(error);
            }
            catch (CudaException)
            {
                throw;
            }
            catch (DllNotFoundException dnfe)
            {
                throw new CudaException(CudaError.NoDriver, dnfe);
            }
            catch (Exception e)
            {
                throw new CudaException(CudaError.Unknown, e);
            }
        }
    }
}
