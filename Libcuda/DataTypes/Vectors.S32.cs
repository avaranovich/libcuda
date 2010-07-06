using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Libcuda.DataTypes
{
    [DebuggerNonUserCode]
    public struct int1 : IEnumerable<int>, IEquatable<int1>
    {
        public int X;
        public int1(int x) { X = x; }

        IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }
        public IEnumerator<int> GetEnumerator() { return new[] { X }.Cast<int>().GetEnumerator(); }
        public override String ToString() { return String.Format("{0}{1}", typeof(int).Name, String.Format("({0})", X)); }

        public bool Equals(int1 other)
        {
            return Equals(other.X, X);
        }

        public override bool Equals(Object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (obj.GetType() != typeof(int1)) return false;
            return Equals((int1)obj);
        }

        public override int GetHashCode()
        {
            return X.GetHashCode();
        }

        public static bool operator ==(int1 left, int1 right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(int1 left, int1 right)
        {
            return !left.Equals(right);
        }
    }

    [DebuggerNonUserCode]
    public struct int2 : IEnumerable<int>, IEquatable<int2>
    {
        public int X;
        public int Y;

        public int2(int x) : this(x, default(int)) { }
        public int2(int x, int y) { X = x; Y = y; }

        IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }
        public IEnumerator<int> GetEnumerator() { return new[] { X, Y }.Cast<int>().GetEnumerator(); }
        public override String ToString() { return String.Format("{0}{1}", typeof(int).Name, String.Format("({0}, {1})", X, Y)); }

        public bool Equals(int2 other)
        {
            return Equals(other.X, X) && Equals(other.Y, Y);
        }

        public override bool Equals(Object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (obj.GetType() != typeof(int2)) return false;
            return Equals((int2)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (X.GetHashCode() * 397) ^ Y.GetHashCode();
            }
        }

        public static bool operator ==(int2 left, int2 right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(int2 left, int2 right)
        {
            return !left.Equals(right);
        }
    }

    [DebuggerNonUserCode]
    public struct int3 : IEnumerable<int>, IEquatable<int3>
    {
        public int X;
        public int Y;
        public int Z;

        public int3(int x) : this(x, default(int), default(int)) { }
        public int3(int x, int y) : this(x, y, default(int)) { }
        public int3(int x, int y, int z) { X = x; Y = y; Z = z; }

        IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }
        public IEnumerator<int> GetEnumerator() { return new[] { X, Y, Z }.Cast<int>().GetEnumerator(); }
        public override String ToString() { return String.Format("{0}{1}", typeof(int).Name, String.Format("({0}, {1}, {2})", X, Y, Z)); }

        public bool Equals(int3 other)
        {
            return Equals(other.X, X) && Equals(other.Y, Y) && Equals(other.Z, Z);
        }

        public override bool Equals(Object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (obj.GetType() != typeof(int3)) return false;
            return Equals((int3)obj);
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

        public static bool operator ==(int3 left, int3 right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(int3 left, int3 right)
        {
            return !left.Equals(right);
        }
    }

    [DebuggerNonUserCode]
    public struct int4 : IEnumerable<int>, IEquatable<int4>
    {
        public int X;
        public int Y;
        public int Z;
        public int W;

        public int4(int x) : this(x, default(int), default(int), default(int)) { }
        public int4(int x, int y) : this(x, y, default(int), default(int)) { }
        public int4(int x, int y, int z) : this(x, y, z, default(int)) { }
        public int4(int x, int y, int z, int w) { X = x; Y = y; Z = z; W = w; }

        IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }
        public IEnumerator<int> GetEnumerator() { return new[] { X, Y, Z, W }.Cast<int>().GetEnumerator(); }
        public override String ToString() { return String.Format("{0}{1}", typeof(int).Name, String.Format("({0}, {1}, {2}, {3})", X, Y, Z, W)); }

        public bool Equals(int4 other)
        {
            return Equals(other.X, X) && Equals(other.Y, Y) && Equals(other.Z, Z) && Equals(other.W, W);
        }

        public override bool Equals(Object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (obj.GetType() != typeof(int4)) return false;
            return Equals((int4)obj);
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

        public static bool operator ==(int4 left, int4 right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(int4 left, int4 right)
        {
            return !left.Equals(right);
        }
    }
}