using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Libcuda.Native.DataTypes
{
    [StructLayout(LayoutKind.Sequential)]
    [DebuggerNonUserCode]
    internal struct CUfunction : IEquatable<CUfunction>
    {
        public IntPtr Handle;

        public static CUfunction Null { get { return (CUfunction)IntPtr.Zero; } }
        public bool IsNull { get { return Handle == Null; } }
        public bool IsNotNull { get { return !IsNull; } }
        public static implicit operator IntPtr(CUfunction @this) { return @this.Handle; }
        public static explicit operator CUfunction(IntPtr handle) { return new CUfunction { Handle = handle }; }

        public bool Equals(CUfunction other) { return other.Handle.Equals(Handle); }
        public override bool Equals(Object obj) { if (ReferenceEquals(null, obj)) return false; if (obj.GetType() != typeof(CUfunction)) return false; return Equals((CUfunction)obj); }
        public override int GetHashCode() { return Handle.GetHashCode(); }
        public static bool operator ==(CUfunction left, CUfunction right) { return left.Equals(right); }
        public static bool operator !=(CUfunction left, CUfunction right) { return !left.Equals(right); }

        public override String ToString()
        {
            if (this == Null) return "CUfunction null";
            var format = "x" + Marshal.SizeOf(typeof(IntPtr));
            return String.Format("CUfunction 0x{0}", Handle.ToString(format));
        }
    }
}