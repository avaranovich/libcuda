using System;
using System.Diagnostics;
using System.Runtime.ConstrainedExecution;
using Libcuda.Api.Devices;
using Libcuda.Api.Native.DataTypes;
using Libcuda.Exceptions;
using Libcuda.Versions;
using XenoGears.Assertions;
using XenoGears.Threading;

namespace Libcuda.Api.Native
{
    [DebuggerNonUserCode]
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

        private static Object _initializationLock = new Object();
        private static bool _hasBeenInitialized = false;
        private static Exception _initializationException = null;
        private class nvcudaInitializationException : Exception { public nvcudaInitializationException(Exception reason) : base(null, reason) { } }
        private static void EnsureInitialized()
        {
            if (!_hasBeenInitialized)
            {
                lock (_initializationLock)
                {
                    if (!_hasBeenInitialized)
                    {
                        try
                        {
                            InitializeGlobalContext();
                        }
                        catch(Exception exn)
                        {
                            _initializationException = exn;
                            throw new nvcudaInitializationException(exn);
                        }
                        finally
                        {
                            _hasBeenInitialized = true;
                        }
                    }
                }
            }
            else
            {
                if (_initializationException != null)
                {
                    throw new nvcudaInitializationException(_initializationException);
                }
                else
                {
                    return;
                }
            }
        }

        private static GlobalContext _globalContext;
        private static void InitializeGlobalContext()
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

            _globalContext = new GlobalContext();
        }

        [DebuggerNonUserCode]
        private class GlobalContext : CriticalFinalizerObject
        {
            private readonly CUcontext _handle;
            public static implicit operator CUcontext(GlobalContext ctx) { return ctx == null ? CUcontext.Null : ctx._handle; }

            public GlobalContext()
            {
                Log.WriteLine("Acquiring number of CUDA-capable devices...");
                var deviceCount = cuDeviceGetCount();
                Log.WriteLine("{0} device(s) found.", cuDeviceGetCount());
                (deviceCount > 0).AssertTrue();

                Log.WriteLine("Accessing device #0...");
                var device = CudaDevice.First;
                Log.WriteLine("Success.");
                Log.WriteLine(Environment.NewLine + device);

                Log.WriteLine("Creating CUDA context for device #0...");
                _handle = cuCtxCreate(CUctx_flags.None, device.Handle);
                Log.WriteLine("Success.");
                Log.WriteLine();
            }

            ~GlobalContext()
            {
                Wrap(() => cuCtxDestroy(_handle));
            }
        }
    }
}
