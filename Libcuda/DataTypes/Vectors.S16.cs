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
    public struct short1 : IEnumerable<short>, IEquatable<short1>
    {
        public short X;
        public short1(short x) { X = x; }

        IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }
        public IEnumerator<short> GetEnumerator() { return new[] { X }.Cast<short>().GetEnumerator(); }
        public override String ToString() { return String.Format("{0}{1}", typeof(short).Name, String.Format("({0})", X)); }

        public bool Equals(short1 other)
        {
            return Equals(other.X, X);
        }

        public override bool Equals(Object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (obj.GetType() != typeof(short1)) return false;
            return Equals((short1)obj);
        }

        public override int GetHashCode()
        {
            return X.GetHashCode();
        }

        public static bool operator ==(short1 left, short1 right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(short1 left, short1 right)
        {
            return !left.Equals(right);
        }
    }

    [DebuggerNonUserCode]
    [StructLayout(LayoutKind.Sequential)]
    public struct short2 : IEnumerable<short>, IEquatable<short2>
    {
        public short X;
        public short Y;

        public short2(short x) : this(x, default(short)) { }
        public short2(short x, short y) { X = x; Y = y; }

        IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }
        public IEnumerator<short> GetEnumerator() { return new[] { X, Y }.Cast<short>().GetEnumerator(); }
        public override String ToString() { return String.Format("{0}{1}", typeof(short).Name, String.Format("({0}, {1})", X, Y)); }

        public bool Equals(short2 other)
        {
            return Equals(other.X, X) && Equals(other.Y, Y);
        }

        public override bool Equals(Object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (obj.GetType() != typeof(short2)) return false;
            return Equals((short2)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (X.GetHashCode() * 397) ^ Y.GetHashCode();
            }
        }

        public static bool operator ==(short2 left, short2 right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(short2 left, short2 right)
        {
            return !left.Equals(right);
        }
    }

    [DebuggerNonUserCode]
    [StructLayout(LayoutKind.Sequential)]
    public struct short3 : IEnumerable<short>, IEquatable<short3>
    {
        public short X;
        public short Y;
        public short Z;

        public short3(short x) : this(x, default(short), default(short)) { }
        public short3(short x, short y) : this(x, y, default(short)) { }
        public short3(short x, short y, short z) { X = x; Y = y; Z = z; }

        IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }
        public IEnumerator<short> GetEnumerator() { return new[] { X, Y, Z }.Cast<short>().GetEnumerator(); }
        public override String ToString() { return String.Format("{0}{1}", typeof(short).Name, String.Format("({0}, {1}, {2})", X, Y, Z)); }

        public bool Equals(short3 other)
        {
            return Equals(other.X, X) && Equals(other.Y, Y) && Equals(other.Z, Z);
        }

        public override bool Equals(Object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (obj.GetType() != typeof(short3)) return false;
            return Equals((short3)obj);
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

        public static bool operator ==(short3 left, short3 right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(short3 left, short3 right)
        {
            return !left.Equals(right);
        }
    }

    [DebuggerNonUserCode]
    [StructLayout(LayoutKind.Sequential)]
    public struct short4 : IEnumerable<short>, IEquatable<short4>
    {
        public short X;
        public short Y;
        public short Z;
        public short W;

        public short4(short x) : this(x, default(short), default(short), default(short)) { }
        public short4(short x, short y) : this(x, y, default(short), default(short)) { }
        public short4(short x, short y, short z) : this(x, y, z, default(short)) { }
        public short4(short x, short y, short z, short w) { X = x; Y = y; Z = z; W = w; }

        IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }
        public IEnumerator<short> GetEnumerator() { return new[] { X, Y, Z, W }.Cast<short>().GetEnumerator(); }
        public override String ToString() { return String.Format("{0}{1}", typeof(short).Name, String.Format("({0}, {1}, {2}, {3})", X, Y, Z, W)); }

        public bool Equals(short4 other)
        {
            return Equals(other.X, X) && Equals(other.Y, Y) && Equals(other.Z, Z) && Equals(other.W, W);
        }

        public override bool Equals(Object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (obj.GetType() != typeof(short4)) return false;
            return Equals((short4)obj);
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

        public static bool operator ==(short4 left, short4 right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(short4 left, short4 right)
        {
            return !left.Equals(right);
        }
    }
}