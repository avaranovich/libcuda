using NUnit.Framework;

namespace Libcuda.Playground.JitAndRun
{
    [TestFixture]
    public class MatMul_Fast : Tests
    {
        protected override string PtxResourceName { get { return "matmul_fast.ptx"; } }
    }
}