using NUnit.Framework;

namespace Libcuda.Playground.JitAndRun
{
    [TestFixture]
    public class MatMul : Tests
    {
        protected override string PtxResourceName { get { return "matmul.ptx"; } }
    }
}