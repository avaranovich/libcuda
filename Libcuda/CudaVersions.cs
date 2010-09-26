using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Libcuda.Api.Native;
using Libcuda.Versions;
using XenoGears.Assertions;
using XenoGears.Collections.Dictionaries;
using XenoGears.Versioning;
using XenoGears.Functional;

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
                (Driver != null).AssertTrue();
                var v = Cuda;
                var t = (Driver.Build % 10) * 100 + Driver.Revision / 100;

                var map = new OrderedDictionary<Tuple<CudaVersion, int?>, SoftwareIsa>();
                Action<CudaVersion, int?, SoftwareIsa> reg = (cuv, drv, isa) => map.Add(Tuple.Create(cuv, drv), isa);
                Func<Tuple<CudaVersion, int?>, bool> gte = pair => pair == null || v > pair.Item1 || (v == pair.Item1 && t >= (pair.Item2 ?? 0));
                Func<Tuple<CudaVersion, int?>, bool> lte = pair => pair == null || v < pair.Item1 || (v == pair.Item1 && t <= (pair.Item2 ?? 0));
                reg(CudaVersion.CUDA_10, null, SoftwareIsa.PTX_10);
                reg(CudaVersion.CUDA_11, null, SoftwareIsa.PTX_11);
                reg(CudaVersion.CUDA_20, null, SoftwareIsa.PTX_12);
                reg(CudaVersion.CUDA_21, null, SoftwareIsa.PTX_13);
                reg(CudaVersion.CUDA_22, null, SoftwareIsa.PTX_14);
                reg(CudaVersion.CUDA_23, 190, SoftwareIsa.PTX_15);
                reg(CudaVersion.CUDA_30, null, SoftwareIsa.PTX_20);
                reg(CudaVersion.CUDA_31, null, SoftwareIsa.PTX_21);

                for (var i = -1; i < map.Count(); ++i)
                {
                    var prev = map.NthOrDefault(i);
                    var next = map.NthOrDefault(i + 1);
                    if (gte(prev.Key) && lte(next.Key))
                    {
                        return prev.Value;
                    }
                }

                throw AssertionHelper.Fail();
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