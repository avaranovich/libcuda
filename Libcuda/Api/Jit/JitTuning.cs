using Libcuda.DataTypes;

namespace Libcuda.Api.Jit
{
    public class JitTuning
    {
        public int Maxnreg { get; set; }
        public dim3 Maxntid { get; set; }
        public dim3 Reqntid { get; set; }
        public int Minnctapersm { get; set; }
        public int Maxnctapersm { get; set; }
    }
}