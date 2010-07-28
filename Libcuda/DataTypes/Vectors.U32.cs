using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;

namespace Libcuda.DataTypes
{
    // todo. implement implicit/explicit casts, arithmetic operations, mirror math apis
    // for some starter see OpenCL spec, section 6.4 "Vector Operations"

    [DebuggerNonUserCode]
    [StructLayout(LayoutKind.Sequential)]
    public struct uint1 : IEnumerable<uint>, IEquatable<uint1>
    {
        public uint X;
        public uint1(uint x) { X = x; }

        IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }
        public IEnumerator<uint> GetEnumerator() { return new[] { X }.Cast<uint>().GetEnumerator(); }
        public override String ToString() { return String.Format("{0}{1}", typeof(uint).Name, String.Format("({0})", X)); }

        public bool Equals(uint1 other)
        {
            return Equals(other.X, X);
        }

        public override bool Equals(Object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (obj.GetType() != typeof(uint1)) return false;
            return Equals((uint1)obj);
        }

        public override int GetHashCode()
        {
            return X.GetHashCode();
        }

        public static bool operator ==(uint1 left, uint1 right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(uint1 left, uint1 right)
        {
            return !left.Equals(right);
        }
    }

    [DebuggerNonUserCode]
    [StructLayout(LayoutKind.Sequential)]
    public struct uint2 : IEnumerable<uint>, IEquatable<uint2>
    {
        public uint X;
        public uint Y;

        public uint2(uint x) : this(x, default(uint)) { }
        public uint2(uint x, uint y) { X = x; Y = y; }

        IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }
        public IEnumerator<uint> GetEnumerator() { return new[] { X, Y }.Cast<uint>().GetEnumerator(); }
        public override String ToString() { return String.Format("{0}{1}", typeof(uint).Name, String.Format("({0}, {1})", X, Y)); }

        public bool Equals(uint2 other)
        {
            return Equals(other.X, X) && Equals(other.Y, Y);
        }

        public override bool Equals(Object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (obj.GetType() != typeof(uint2)) return false;
            return Equals((uint2)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (X.GetHashCode() * 397) ^ Y.GetHashCode();
            }
        }

        public static bool operator ==(uint2 left, uint2 right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(uint2 left, uint2 right)
        {
            return !left.Equals(right);
        }
    }

    [DebuggerNonUserCode]
    [StructLayout(LayoutKind.Sequential)]
    public struct uint3 : IEnumerable<uint>, IEquatable<uint3>
    {
        public uint X;
        public uint Y;
        public uint Z;

        public uint3(uint x) : this(x, default(uint), default(uint)) { }
        public uint3(uint x, uint y) : this(x, y, default(uint)) { }
        public uint3(uint x, uint y, uint z) { X = x; Y = y; Z = z; }

        IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }
        public IEnumerator<uint> GetEnumerator() { return new[] { X, Y, Z }.Cast<uint>().GetEnumerator(); }
        public override String ToString() { return String.Format("{0}{1}", typeof(uint).Name, String.Format("({0}, {1}, {2})", X, Y, Z)); }

        public bool Equals(uint3 other)
        {
            return Equals(other.X, X) && Equals(other.Y, Y) && Equals(other.Z, Z);
        }

        public override bool Equals(Object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (obj.GetType() != typeof(uint3)) return false;
            return Equals((uint3)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int result = X.GetHashCode();
                result = (result * 397) ^ Y.GetHashCode();
                result = (result * 397) ^ Z.GetHashCode();
                return result;
            }
        }

        public static bool operator ==(uint3 left, uint3 right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(uint3 left, uint3 right)
        {
            return !left.Equals(right);
        }
    }

    [DebuggerNonUserCode]
    [StructLayout(LayoutKind.Sequential)]
    public struct uint4 : IEnumerable<uint>, IEquatable<uint4>
    {
        public uint X;
        public uint Y;
        public uint Z;
        public uint W;

        public uint4(uint x) : this(x, default(uint), default(uint), default(uint)) { }
        public uint4(uint x, uint y) : this(x, y, default(uint), default(uint)) { }
        public uint4(uint x, uint y, uint z) : this(x, y, z, default(uint)) { }
        public uint4(uint x, uint y, uint z, uint w) { X = x; Y = y; Z = z; W = w; }

        IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }
        public IEnumerator<uint> GetEnumerator() { return new[] { X, Y, Z, W }.Cast<uint>().GetEnumerator(); }
        public override String ToString() { return String.Format("{0}{1}", typeof(uint).Name, String.Format("({0}, {1}, {2}, {3})", X, Y, Z, W)); }

        public bool Equals(uint4 other)
        {
            return Equals(other.X, X) && Equals(other.Y, Y) && Equals(other.Z, Z) && Equals(other.W, W);
        }

        public override bool Equals(Object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (obj.GetType() != typeof(uint4)) return false;
            return Equals((uint4)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int result = X.GetHashCode();
                result = (result * 397) ^ Y.GetHashCode();
                result = (result * 397) ^ Z.GetHashCode();
                result = (result * 397) ^ W.GetHashCode();
                return result;
            }
        }

        public static bool operator ==(uint4 left, uint4 right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(uint4 left, uint4 right)
        {
            return !left.Equals(right);
        }
    }
}