using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Libcuda.Api.Native.DataTypes
{
    [StructLayout(LayoutKind.Sequential)]
    [DebuggerNonUserCode]
    public struct CUmodule
    {
        public IntPtr Handle;

        public static CUmodule Null { get { return (CUmodule)IntPtr.Zero; } }
        public bool IsNull { get { return Handle == Null; } }
        public bool IsNotNull { get { return !IsNull; } }
        public static implicit operator IntPtr(CUmodule @this) { return @this.Handle; }
        public static explicit operator CUmodule(IntPtr handle) { return new CUmodule { Handle = handle }; }

        public bool Equals(CUmodule other) { return other.Handle.Equals(Handle); }
        public override bool Equals(Object obj) { if (ReferenceEquals(null, obj)) return false; if (obj.GetType() != typeof(CUmodule)) return false; return Equals((CUmodule)obj); }
        public override int GetHashCode() { return Handle.GetHashCode(); }
        public static bool operator ==(CUmodule left, CUmodule right) { return left.Equals(right); }
        public static bool operator !=(CUmodule left, CUmodule right) { return !left.Equals(right); }

        public override String ToString()
        {
            if (this == Null) return "CUmodule null";
            var format = "x" + Marshal.SizeOf(typeof(IntPtr));
            return String.Format("CUmodule 0x{0}", Handle.ToString(format));
        }
    }
}