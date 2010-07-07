using System;
using System.IO;
using Libcuda.DataTypes;
using Libcuda.Api.Run;
using XenoGears.Assertions;
using XenoGears.Functional;

namespace Libcuda.Playground.JitAndRun
{
    public abstract partial class Tests
    {
        private float[,] KernelMul(float[,] a, float[,] b)
        {
            const int defaultBlockSize = 16;
            var blockDim = new dim3(Math.Min(b.Width(), defaultBlockSize), Math.Min(a.Height(), defaultBlockSize));
            var gridDim = new dim3((int)Math.Ceiling(1.0 * b.Width() / blockDim.X), (int)Math.Ceiling(1.0 * a.Height() / blockDim.Y));

            var ptx = LoadPtxFromResources();
            using (var jitted = ptx.JitKernel(blockDim))
            {
                // todo. that's an untidy way to invoke a kernel
                // since it doesn't dispose of invocation parameters
                // neither it is shielded against partial initialization issues

                var c = new float[a.Height(), b.Width()];
                var c_result = jitted.Invoke(gridDim, blockDim,
                    a.Width().In(), a.Height().In(), a.In(),
                    b.Width().In(), b.Height().In(), b.In(),
                    c.Width().In(), c.Height().In(), c.Out());
                return (float[,])c_result;
            }
        }

        protected abstract String PtxResourceName { get; }
        private String LoadPtxFromResources()
        {
            var fullName = this.GetType().Namespace + ".Ptx." + PtxResourceName;
            using (var resourceStream = this.GetType().Assembly.GetManifestResourceStream(fullName))
            {
                resourceStream.AssertNotNull();
                return new StreamReader(resourceStream).ReadToEnd();
            }
        }
    }
}
