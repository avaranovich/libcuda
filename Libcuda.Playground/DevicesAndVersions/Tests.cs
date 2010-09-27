using System;
using Libcuda.Api.Devices;
using Libcuda.Versions;
using XenoGears.Assertions;
using XenoGears.Playground.Framework;
using XenoGears.Traits.Dumpable;
using NUnit.Framework;

namespace Libcuda.Playground.DevicesAndVersions
{
    [TestFixture]
    public class Tests : BaseTests
    {
        [Test, Category("Hot")]
        public void Win7x64_Cuda32RC_Gtx260()
        {
            CudaDriver.Ensure();

            (CudaVersions.Driver == new Version("8.17.12.6061")).AssertTrue();
            (CudaVersions.Cuda == CudaVersion.CUDA_32).AssertTrue();
            (CudaVersions.SoftwareIsa == SoftwareIsa.PTX_21).AssertTrue();
            (CudaVersions.HardwareIsa == HardwareIsa.SM_13).AssertTrue();

            var gtx260 = CudaDevice.Current.AssertNotNull();
            VerifyResult(gtx260.DumpAsText());
        }
    }
}