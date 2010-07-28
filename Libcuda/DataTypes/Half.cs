using System.Diagnostics;

namespace Libcuda.DataTypes
{
    // todo. implement implicit/explicit casts, arithmetic operations, mirror math apis
    // for some starter for some starter see OpenCL spec, section 9.10 "Half Floating-Point"

    [DebuggerNonUserCode]
    public struct half
    {
        public ushort Raw { get; set; }
    }
}
