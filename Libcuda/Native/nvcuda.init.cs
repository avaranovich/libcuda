using System;
using System.Diagnostics;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using Libcuda.Devices;
using Libcuda.Native.DataTypes;
using Libcuda.Native.Exceptions;
using XenoGears.Assertions;
using XenoGears.Logging;
using XenoGears.Reflection.Attributes;

namespace Libcuda.Native
{
    [DebuggerNonUserCode]
    internal static partial class nvcuda
    {
        // todo. so far we use a single context to launch kernel invocations
        // this context is global to all users of this assembly instance
        // and has a lifetime of the entire application (this is created on-demand)
        //
        // in future it might be necessary to support different contexts
        // e.g. to use multiple devices or to service multiple clients at once
        // but for now I use the simplest option possible

        private static Object ctorLock = new Object();
        private static AutoFinalizableContext ctx;

        static nvcuda()
        {
            lock (ctorLock)
            {
                if (ctx == null)
                {
                    ctx = new AutoFinalizableContext();
                }
            }
        }

        [DebuggerNonUserCode]
        private class AutoFinalizableContext : CriticalFinalizerObject
        {
            private readonly CUcontext _ctx;

            public AutoFinalizableContext()
            {
                Log.TraceLine("Dynamically linking to CUDA driver...");
                int driverVersion;
                var error1 = cuDriverGetVersion(out driverVersion);
                if (error1 != CUresult.Success) throw new CudaException(error1);
                var driverName = typeof(nvcuda).GetMethod("cuDriverGetVersion").Attr<DllImportAttribute>().Value;
                Log.TraceLine("Successfully linked to {0} {1}.{2}.", driverName, driverVersion / 1000, driverVersion % 100);
                (driverVersion >= 3010).AssertTrue();
                Log.TraceLine();

                Log.TraceLine("Initializing CUDA driver...");
                var error2 = cuInit(CUinit_flags.None);
                if (error2 != CUresult.Success) throw new CudaException(error2);
                Log.TraceLine("Success.");
                Log.TraceLine();

                // Step 3. Verify that we've got at least one CUDA capable device
                Log.TraceLine("Acquiring number of CUDA-capable devices...");
                int deviceCount;
                var error3 = cuDeviceGetCount(out deviceCount);
                if (error3 != CUresult.Success) throw new CudaException(error3);
                Log.TraceLine("{0} device(s) found.", deviceCount);
                (deviceCount > 0).AssertTrue();
    
                // Step 4. Get the device (only 1 device is supported at the moment)
                Log.TraceLine("Accessing device #0...");
                var device = CudaDevice.First;
                Log.TraceLine("Success.");
                Log.TraceLine(Environment.NewLine + device);
    
                // Step 5. Create the context - kernel execution environment
                Log.TraceLine("Creating CUDA context on device #0...");
                var error5 = cuCtxCreate(out _ctx, CUctx_flags.None, device.Handle);
                if (error5 != CUresult.Success) throw new CudaException(error5);
                Log.TraceLine("Success.");
                Log.TraceLine(String.Empty);
            }

            ~AutoFinalizableContext()
            {
                if (_ctx.Handle != IntPtr.Zero)
                {
                    cuCtxDestroy(_ctx);
                }
            }
        }

        [DllImport("nvcuda")]
        // http://developer.download.nvidia.com/compute/cuda/3_1/toolkit/docs/online/group__CUVERSION_gf83e088e9433ce2e9ce87203791dd122.html
        public static extern CUresult cuDriverGetVersion(out int version);

        [DllImport("nvcuda")]
        // http://developer.download.nvidia.com/compute/cuda/3_1/toolkit/docs/online/group__CUINIT_g4703189f4c7f490c73f77942a3fa8443.html
        private static extern CUresult cuInit(CUinit_flags Flags);

        [Flags]
        private enum CUinit_flags
        {
            None = 0
        }

        [DllImport("nvcuda")]
        // http://developer.download.nvidia.com/compute/cuda/3_1/toolkit/docs/online/group__CUCTX_g02b31a192b32043c0787696292f1ffbe.html
        private static extern CUresult cuCtxCreate(out CUcontext pctx, CUctx_flags flags, CUdevice dev);
        [DllImport("nvcuda")]
        // http://developer.download.nvidia.com/compute/cuda/3_1/toolkit/docs/online/group__CUCTX_g23bf81c24c28be3495fec41146f9e025.html
        private static extern CUresult cuCtxDestroy(CUcontext ctx);

        [StructLayout(LayoutKind.Sequential)]
        [DebuggerNonUserCode]
        private struct CUcontext
        {
            public IntPtr Handle;
        }

        [Flags]
        // http://developer.download.nvidia.com/compute/cuda/3_1/toolkit/docs/online/group__CUDA__TYPES_g12d89ce3fea2678bf187aa2876ed67a6.html#g12d89ce3fea2678bf187aa2876ed67a6
        private enum CUctx_flags
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
}