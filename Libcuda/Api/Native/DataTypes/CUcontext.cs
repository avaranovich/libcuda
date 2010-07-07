using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Libcuda.Api.Native.DataTypes
{
    [StructLayout(LayoutKind.Sequential)]
    [DebuggerNonUserCode]
    public struct CUcontext
    {
        public IntPtr Handle;

        public static CUcontext Null { get { return (CUcontext)IntPtr.Zero; } }
        public bool IsNull { get { return Handle == Null; } }
        public bool IsNotNull { get { return !IsNull; } }
        public static implicit operator IntPtr(CUcontext @this) { return @this.Handle; }
        public static explicit operator CUcontext(IntPtr handle) { return new CUcontext { Handle = handle }; }

        public bool Equals(CUcontext other) { return other.Handle.Equals(Handle); }
        public override bool Equals(Object obj) { if (ReferenceEquals(null, obj)) return false; if (obj.GetType() != typeof(CUcontext)) return false; return Equals((CUcontext)obj); }
        public override int GetHashCode() { return Handle.GetHashCode(); }
        public static bool operator ==(CUcontext left, CUcontext right) { return left.Equals(right); }
        public static bool operator !=(CUcontext left, CUcontext right) { return !left.Equals(right); }

        public override String ToString()
        {
            if (this == Null) return "CUcontext null";
            var format = "x" + Marshal.SizeOf(typeof(IntPtr));
            return String.Format("CUcontext 0x{0}", Handle.ToString(format));
        }
    }
}