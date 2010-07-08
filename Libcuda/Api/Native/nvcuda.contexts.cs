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
using XenoGears.Threading;

namespace Libcuda.Api.Native
{
    public static partial class nvcuda
    {
        // todo. so far we use a single context to launch kernel invocations
        // this context is global to all users of this assembly instance
        // and has a lifetime of the entire application (this is created on-demand)
        //
        // note. this means that so far Libcuda is effectively single-threaded
        // in future it might be necessary to support different contexts
        // e.g. to use multiple devices or to service multiple clients at once
        // but for now I use the simplest option possible
        //
        // note. if you decide to make Libcuda multi-threaded
        // you must also insert affinity checks for high-level objects
        // so that they don't accidentally get accessed from an erroneous thread
        // CUDA driver checks this anyways, but we should provide high-level checks too
        // upd. it could also be a good idea to configure logging at thread level

        private static GlobalContext _ctx;
        private static int _affinity;
        static nvcuda()
        {
            using (NativeThread.Affinitize(out _affinity))
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
                cuInit(CUinit_flags.None);
                Log.WriteLine("Success.");
                Log.WriteLine();

                // Step 3. Verify that we've got at least one CUDA capable device
                Log.WriteLine("Acquiring number of CUDA-capable devices...");
                var deviceCount = cuDeviceGetCount();
                Log.WriteLine("{0} device(s) found.", cuDeviceGetCount());
                (deviceCount > 0).AssertTrue();

                // Step 4. Get the device (only 1 device is supported at the moment)
                Log.WriteLine("Accessing device #0...");
                var device = CudaDevice.First;
                Log.WriteLine("Success.");
                Log.WriteLine(Environment.NewLine + device);

                // Step 5. Create the context - kernel execution environment
                Log.WriteLine("Creating CUDA context for device #0...");
                _ctx = cuCtxCreate(CUctx_flags.None, device.Handle);
                Log.WriteLine("Success.");
                Log.WriteLine();
            }

            ~GlobalContext()
            {
                // todo. here we've got a resource leak
                // finalizers cannot call nvcuda, since they run on a different thread
            }
        }

        [DllImport("nvcuda", EntryPoint = "cuCtxCreate")]
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

        [DllImport("nvcuda", EntryPoint = "cuCtxDestroy")]
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
