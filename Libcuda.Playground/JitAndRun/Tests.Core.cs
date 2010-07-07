using Libcuda.Tracing;
using NUnit.Framework;
using XenoGears.Playground.Framework;

namespace Libcuda.Playground.JitAndRun
{
    public abstract partial class Tests : BaseTests
    {
        [Test, Category("Hot")]
        public void SmallTest()
        {
            var a = RandMatrix(6, 15);
            var b = RandMatrix(15, 9);
            var rc = ReferenceMul(a, b);

            Traces.Init.Disable();
            var kc = KernelMul(a, b);
            AssertAreTheSame(rc, kc);
            VerifyResult();
        }

        [Test]
        public void MediumTest()
        {
            var a = RandMatrix(17, 19);
            var b = RandMatrix(19, 18);
            var rc = ReferenceMul(a, b);

            Traces.Init.Disable();
            var kc = KernelMul(a, b);
            AssertAreTheSame(rc, kc);
            VerifyResult();
        }

        [Test]
        public void BigTest()
        {
            var a = RandMatrix(302, 434);
            var b = RandMatrix(434, 408);
            var rc = ReferenceMul(a, b);

            Traces.Init.Disable();
            var kc = KernelMul(a, b);
            AssertAreTheSame(rc, kc);
            VerifyResult();
        }
    }
}