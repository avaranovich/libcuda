using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;

namespace Libcuda.DataTypes
{
    [DebuggerNonUserCode]
    [StructLayout(LayoutKind.Sequential)]
    public struct sbyte1 : IEnumerable<sbyte>, IEquatable<sbyte1>
    {
        public sbyte X;
        public sbyte1(sbyte x) { X = x; }

        IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }
        public IEnumerator<sbyte> GetEnumerator() { return new[] { X }.Cast<sbyte>().GetEnumerator(); }
        public override String ToString() { return String.Format("{0}{1}", typeof(sbyte).Name, String.Format("({0})", X)); }

        public bool Equals(sbyte1 other)
        {
            return Equals(other.X, X);
        }

        public override bool Equals(Object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (obj.GetType() != typeof(sbyte1)) return false;
            return Equals((sbyte1)obj);
        }

        public override int GetHashCode()
        {
            return X.GetHashCode();
        }

        public static bool operator ==(sbyte1 left, sbyte1 right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(sbyte1 left, sbyte1 right)
        {
            return !left.Equals(right);
        }
    }

    [DebuggerNonUserCode]
    [StructLayout(LayoutKind.Sequential)]
    public struct sbyte2 : IEnumerable<sbyte>, IEquatable<sbyte2>
    {
        public sbyte X;
        public sbyte Y;

        public sbyte2(sbyte x) : this(x, default(sbyte)) { }
        public sbyte2(sbyte x, sbyte y) { X = x; Y = y; }

        IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }
        public IEnumerator<sbyte> GetEnumerator() { return new[] { X, Y }.Cast<sbyte>().GetEnumerator(); }
        public override String ToString() { return String.Format("{0}{1}", typeof(sbyte).Name, String.Format("({0}, {1})", X, Y)); }

        public bool Equals(sbyte2 other)
        {
            return Equals(other.X, X) && Equals(other.Y, Y);
        }

        public override bool Equals(Object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (obj.GetType() != typeof(sbyte2)) return false;
            return Equals((sbyte2)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (X.GetHashCode() * 397) ^ Y.GetHashCode();
            }
        }

        public static bool operator ==(sbyte2 left, sbyte2 right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(sbyte2 left, sbyte2 right)
        {
            return !left.Equals(right);
        }
    }

    [DebuggerNonUserCode]
    [StructLayout(LayoutKind.Sequential)]
    public struct sbyte3 : IEnumerable<sbyte>, IEquatable<sbyte3>
    {
        public sbyte X;
        public sbyte Y;
        public sbyte Z;

        public sbyte3(sbyte x) : this(x, default(sbyte), default(sbyte)) { }
        public sbyte3(sbyte x, sbyte y) : this(x, y, default(sbyte)) { }
        public sbyte3(sbyte x, sbyte y, sbyte z) { X = x; Y = y; Z = z; }

        IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }
        public IEnumerator<sbyte> GetEnumerator() { return new[] { X, Y, Z }.Cast<sbyte>().GetEnumerator(); }
        public override String ToString() { return String.Format("{0}{1}", typeof(sbyte).Name, String.Format("({0}, {1}, {2})", X, Y, Z)); }

        public bool Equals(sbyte3 other)
        {
            return Equals(other.X, X) && Equals(other.Y, Y) && Equals(other.Z, Z);
        }

        public override bool Equals(Object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (obj.GetType() != typeof(sbyte3)) return false;
            return Equals((sbyte3)obj);
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

        public static bool operator ==(sbyte3 left, sbyte3 right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(sbyte3 left, sbyte3 right)
        {
            return !left.Equals(right);
        }
    }

    [DebuggerNonUserCode]
    [StructLayout(LayoutKind.Sequential)]
    public struct sbyte4 : IEnumerable<sbyte>, IEquatable<sbyte4>
    {
        public sbyte X;
        public sbyte Y;
        public sbyte Z;
        public sbyte W;

        public sbyte4(sbyte x) : this(x, default(sbyte), default(sbyte), default(sbyte)) { }
        public sbyte4(sbyte x, sbyte y) : this(x, y, default(sbyte), default(sbyte)) { }
        public sbyte4(sbyte x, sbyte y, sbyte z) : this(x, y, z, default(sbyte)) { }
        public sbyte4(sbyte x, sbyte y, sbyte z, sbyte w) { X = x; Y = y; Z = z; W = w; }

        IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }
        public IEnumerator<sbyte> GetEnumerator() { return new[] { X, Y, Z, W }.Cast<sbyte>().GetEnumerator(); }
        public override String ToString() { return String.Format("{0}{1}", typeof(sbyte).Name, String.Format("({0}, {1}, {2}, {3})", X, Y, Z, W)); }

        public bool Equals(sbyte4 other)
        {
            return Equals(other.X, X) && Equals(other.Y, Y) && Equals(other.Z, Z) && Equals(other.W, W);
        }

        public override bool Equals(Object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (obj.GetType() != typeof(sbyte4)) return false;
            return Equals((sbyte4)obj);
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

        public static bool operator ==(sbyte4 left, sbyte4 right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(sbyte4 left, sbyte4 right)
        {
            return !left.Equals(right);
        }
    }
}