using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Libcuda.Native.DataTypes
{
    [StructLayout(LayoutKind.Sequential)]
    [DebuggerNonUserCode]
    internal struct CUdevice
    {
        public int Handle;

        public static CUdevice Null { get { return (CUdevice)(-1); } }
        public bool IsNull { get { return Handle == Null; } }
        public bool IsNotNull { get { return !IsNull; } }
        public static implicit operator int(CUdevice @this) { return @this.Handle; }
        public static explicit operator CUdevice(int handle) { return new CUdevice{Handle = handle}; }

        public bool Equals(CUdevice other) { return other.Handle.Equals(Handle); }
        public override bool Equals(Object obj) { if (ReferenceEquals(null, obj)) return false; if (obj.GetType() != typeof(CUdevice)) return false; return Equals((CUdevice)obj); }
        public override int GetHashCode() { return Handle.GetHashCode(); }
        public static bool operator ==(CUdevice left, CUdevice right) { return left.Equals(right); }
        public static bool operator !=(CUdevice left, CUdevice right) { return !left.Equals(right); }

        public override String ToString()
        {
            if (this == Null) return "CUdevice null";
            return String.Format("CUdevice #{0}", Handle);
        }
    }
}