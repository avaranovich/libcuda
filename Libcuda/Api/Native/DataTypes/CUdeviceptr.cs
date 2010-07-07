using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Libcuda.Api.Native.DataTypes
{
    [StructLayout(LayoutKind.Sequential)]
    [DebuggerNonUserCode]
    public struct CUdeviceptr
    {
        public uint Handle;
        public int Size { get { return 4; } }

        public static CUdeviceptr operator +(CUdeviceptr src, uint value) { return (CUdeviceptr)(src.Handle + value); }
        public static CUdeviceptr operator +(CUdeviceptr src, int value) { return value >= 0 ? src + (uint)Math.Abs(value) : src - (uint)Math.Abs(value); }
        public static CUdeviceptr operator -(CUdeviceptr src, uint value) { return (CUdeviceptr)(src.Handle - value); }
        public static CUdeviceptr operator -(CUdeviceptr src, int value) { return value >= 0 ? src - (uint)Math.Abs(value) : src + (uint)Math.Abs(value); }

        public static CUdeviceptr Null { get { return (CUdeviceptr)0u; } }
        public bool IsNull { get { return Handle == Null; } }
        public bool IsNotNull { get { return !IsNull; } }
        public static implicit operator uint(CUdeviceptr src) { return src.Handle; }
        public static explicit operator CUdeviceptr(uint src) { return new CUdeviceptr{Handle = src}; }

        public bool Equals(CUdeviceptr other) { return other.Handle.Equals(Handle); }
        public override bool Equals(Object obj) { if (ReferenceEquals(null, obj)) return false; if (obj.GetType() != typeof(CUdeviceptr)) return false; return Equals((CUdeviceptr)obj); }
        public override int GetHashCode() { return Handle.GetHashCode(); }
        public static bool operator ==(CUdeviceptr left, CUdeviceptr right) { return left.Equals(right); }
        public static bool operator !=(CUdeviceptr left, CUdeviceptr right) { return !left.Equals(right); }

        public override String ToString()
        {
            if (this == Null) return "CUdeviceptr null";
            return String.Format("CUdeviceptr 0x{0}", Handle.ToString("x8"));
        }
    }
}