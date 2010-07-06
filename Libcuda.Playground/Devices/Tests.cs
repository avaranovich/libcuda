using Libcuda.Devices;
using XenoGears.Assertions;
using XenoGears.Playground.Framework;
using XenoGears.Traits.Dumpable;
using NUnit.Framework;

namespace Libcuda.Playground.Devices
{
    [TestFixture]
    public class Tests : BaseTests
    {
        [Test, Category("Hot")]
        public void Gtx260()
        {
            var gtx260 = CudaDevice.Current.AssertNotNull();
            VerifyResult(gtx260.DumpAsText());
        }
    }
}
