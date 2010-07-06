using System;
using System.IO;
using Libcuda.DataTypes;
using Libcuda.Run;
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
                var c = new float[a.Height(), b.Width()];
                using (var result = jitted.Run(gridDim, blockDim,
                    a.Width().In(), a.Height().In(), a.In(),
                    b.Width().In(), b.Height().In(), b.In(),
                    c.Width().In(), c.Height().In(), c.Out()))
                {
                    var c_result = result.Result;
                    return (float[,])c_result;
                }
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
