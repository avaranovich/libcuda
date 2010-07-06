using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using Libcuda.Devices;
using Libcuda.Native;
using Libcuda.Native.Exceptions;
using XenoGears.Functional;

namespace Libcuda
{
    [DebuggerNonUserCode]
    public static class CudaDevices
    {
        public static CudaDevice Current
        {
            get
            {
                // todo. implement current device detection for multiple GPUs
                return First;
            }
        }

        public static int Count
        {
            get
            {
                try
                {
                    int deviceCount;
                    var error1 = nvcuda.cuDeviceGetCount(out deviceCount);
                    if (error1 != CUresult.Success) throw new CudaException(error1);
                    return deviceCount;
                }
                catch (DllNotFoundException)
                {
                    return 0;
                }
            }
        }

        public static ReadOnlyCollection<CudaDevice> All
        {
            get
            {
                return 0.UpTo(Count - 1).Select(i => new CudaDevice(i)).ToReadOnly();
            }
        }

        public static CudaDevice First
        {
            get
            {
                return Count < 1 ? null : new CudaDevice(0);
            }
        }

        public static CudaDevice Second
        {
            get
            {
                return Count < 2 ? null : new CudaDevice(1);
            }
        }
    }
}