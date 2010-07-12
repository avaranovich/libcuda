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
    public struct ushort1 : IEnumerable<ushort>, IEquatable<ushort1>
    {
        public ushort X;
        public ushort1(ushort x) { X = x; }

        IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }
        public IEnumerator<ushort> GetEnumerator() { return new[] { X }.Cast<ushort>().GetEnumerator(); }
        public override String ToString() { return String.Format("{0}{1}", typeof(ushort).Name, String.Format("({0})", X)); }

        public bool Equals(ushort1 other)
        {
            return Equals(other.X, X);
        }

        public override bool Equals(Object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (obj.GetType() != typeof(ushort1)) return false;
            return Equals((ushort1)obj);
        }

        public override int GetHashCode()
        {
            return X.GetHashCode();
        }

        public static bool operator ==(ushort1 left, ushort1 right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(ushort1 left, ushort1 right)
        {
            return !left.Equals(right);
        }
    }

    [DebuggerNonUserCode]
    [StructLayout(LayoutKind.Sequential)]
    public struct ushort2 : IEnumerable<ushort>, IEquatable<ushort2>
    {
        public ushort X;
        public ushort Y;

        public ushort2(ushort x) : this(x, default(ushort)) { }
        public ushort2(ushort x, ushort y) { X = x; Y = y; }

        IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }
        public IEnumerator<ushort> GetEnumerator() { return new[] { X, Y }.Cast<ushort>().GetEnumerator(); }
        public override String ToString() { return String.Format("{0}{1}", typeof(ushort).Name, String.Format("({0}, {1})", X, Y)); }

        public bool Equals(ushort2 other)
        {
            return Equals(other.X, X) && Equals(other.Y, Y);
        }

        public override bool Equals(Object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (obj.GetType() != typeof(ushort2)) return false;
            return Equals((ushort2)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (X.GetHashCode() * 397) ^ Y.GetHashCode();
            }
        }

        public static bool operator ==(ushort2 left, ushort2 right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(ushort2 left, ushort2 right)
        {
            return !left.Equals(right);
        }
    }

    [DebuggerNonUserCode]
    [StructLayout(LayoutKind.Sequential)]
    public struct ushort3 : IEnumerable<ushort>, IEquatable<ushort3>
    {
        public ushort X;
        public ushort Y;
        public ushort Z;

        public ushort3(ushort x) : this(x, default(ushort), default(ushort)) { }
        public ushort3(ushort x, ushort y) : this(x, y, default(ushort)) { }
        public ushort3(ushort x, ushort y, ushort z) { X = x; Y = y; Z = z; }

        IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }
        public IEnumerator<ushort> GetEnumerator() { return new[] { X, Y, Z }.Cast<ushort>().GetEnumerator(); }
        public override String ToString() { return String.Format("{0}{1}", typeof(ushort).Name, String.Format("({0}, {1}, {2})", X, Y, Z)); }

        public bool Equals(ushort3 other)
        {
            return Equals(other.X, X) && Equals(other.Y, Y) && Equals(other.Z, Z);
        }

        public override bool Equals(Object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (obj.GetType() != typeof(ushort3)) return false;
            return Equals((ushort3)obj);
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

        public static bool operator ==(ushort3 left, ushort3 right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(ushort3 left, ushort3 right)
        {
            return !left.Equals(right);
        }
    }

    [DebuggerNonUserCode]
    [StructLayout(LayoutKind.Sequential)]
    public struct ushort4 : IEnumerable<ushort>, IEquatable<ushort4>
    {
        public ushort X;
        public ushort Y;
        public ushort Z;
        public ushort W;

        public ushort4(ushort x) : this(x, default(ushort), default(ushort), default(ushort)) { }
        public ushort4(ushort x, ushort y) : this(x, y, default(ushort), default(ushort)) { }
        public ushort4(ushort x, ushort y, ushort z) : this(x, y, z, default(ushort)) { }
        public ushort4(ushort x, ushort y, ushort z, ushort w) { X = x; Y = y; Z = z; W = w; }

        IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }
        public IEnumerator<ushort> GetEnumerator() { return new[] { X, Y, Z, W }.Cast<ushort>().GetEnumerator(); }
        public override String ToString() { return String.Format("{0}{1}", typeof(ushort).Name, String.Format("({0}, {1}, {2}, {3})", X, Y, Z, W)); }

        public bool Equals(ushort4 other)
        {
            return Equals(other.X, X) && Equals(other.Y, Y) && Equals(other.Z, Z) && Equals(other.W, W);
        }

        public override bool Equals(Object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (obj.GetType() != typeof(ushort4)) return false;
            return Equals((ushort4)obj);
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

        public static bool operator ==(ushort4 left, ushort4 right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(ushort4 left, ushort4 right)
        {
            return !left.Equals(right);
        }
    }
}