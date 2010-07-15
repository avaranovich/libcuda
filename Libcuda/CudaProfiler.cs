using System;
using System.Diagnostics;
using Libcuda.Api.DataTypes;
using Libcuda.Api.Native;
using Libcuda.Api.Native.DataTypes;

namespace Libcuda
{
    [DebuggerNonUserCode]
    public static class CudaProfiler
    {
        // note. cannot use TimeSpan here because it ain't work with fractions of milliseconds
        public static ElapsedTime Benchmark(Action action)
        {
            var before = CUevent.Null;
            var after = CUevent.Null;

            try
            {
                before = nvcuda.cuEventCreate(CUevent_flags.Default);
                after = nvcuda.cuEventCreate(CUevent_flags.Default);

                nvcuda.cuEventRecord(before);
                action();
                nvcuda.cuEventRecord(after);

                nvcuda.cuEventSynchronize(after);
                return nvcuda.cuEventElapsedTime(before, after);
            }
            finally 
            {
                // todo. there's a slight possibility of resource leak here
                if (before.IsNotNull) nvcuda.cuEventDestroy(before);
                if (after.IsNotNull) nvcuda.cuEventDestroy(after);
            }
        }
    }
}