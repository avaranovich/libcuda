using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Libcuda.Native.DataTypes
{
    [StructLayout(LayoutKind.Sequential)]
    [DebuggerNonUserCode]
    internal struct CUevent
    {
        public IntPtr Handle;

        public static CUevent Null { get { return (CUevent)IntPtr.Zero; } }
        public bool IsNull { get { return Handle == Null; } }
        public bool IsNotNull { get { return !IsNull; } }
        public static implicit operator IntPtr(CUevent @this) { return @this.Handle; }
        public static explicit operator CUevent(IntPtr handle) { return new CUevent{Handle = handle}; }

        public bool Equals(CUevent other) { return other.Handle.Equals(Handle); }
        public override bool Equals(Object obj) { if (ReferenceEquals(null, obj)) return false; if (obj.GetType() != typeof(CUevent)) return false; return Equals((CUevent)obj); }
        public override int GetHashCode() { return Handle.GetHashCode(); }
        public static bool operator ==(CUevent left, CUevent right) { return left.Equals(right); }
        public static bool operator !=(CUevent left, CUevent right) { return !left.Equals(right); }

        public override String ToString()
        {
            if (this == Null) return "CUevent null";
            var format = "x" + Marshal.SizeOf(typeof(IntPtr));
            return String.Format("CUevent 0x{0}", Handle.ToString(format));
        }
    }
}