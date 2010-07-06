using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Libcuda.DataTypes
{
    [DebuggerNonUserCode]
    public struct ulong1 : IEnumerable<ulong>, IEquatable<ulong1>
    {
        public ulong X;
        public ulong1(ulong x) { X = x; }

        IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }
        public IEnumerator<ulong> GetEnumerator() { return new[] { X }.Cast<ulong>().GetEnumerator(); }
        public override String ToString() { return String.Format("{0}{1}", typeof(ulong).Name, String.Format("({0})", X)); }

        public bool Equals(ulong1 other)
        {
            return Equals(other.X, X);
        }

        public override bool Equals(Object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (obj.GetType() != typeof(ulong1)) return false;
            return Equals((ulong1)obj);
        }

        public override int GetHashCode()
        {
            return X.GetHashCode();
        }

        public static bool operator ==(ulong1 left, ulong1 right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(ulong1 left, ulong1 right)
        {
            return !left.Equals(right);
        }
    }

    [DebuggerNonUserCode]
    public struct ulong2 : IEnumerable<ulong>, IEquatable<ulong2>
    {
        public ulong X;
        public ulong Y;

        public ulong2(ulong x) : this(x, default(ulong)) { }
        public ulong2(ulong x, ulong y) { X = x; Y = y; }

        IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }
        public IEnumerator<ulong> GetEnumerator() { return new[] { X, Y }.Cast<ulong>().GetEnumerator(); }
        public override String ToString() { return String.Format("{0}{1}", typeof(ulong).Name, String.Format("({0}, {1})", X, Y)); }

        public bool Equals(ulong2 other)
        {
            return Equals(other.X, X) && Equals(other.Y, Y);
        }

        public override bool Equals(Object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (obj.GetType() != typeof(ulong2)) return false;
            return Equals((ulong2)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (X.GetHashCode() * 397) ^ Y.GetHashCode();
            }
        }

        public static bool operator ==(ulong2 left, ulong2 right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(ulong2 left, ulong2 right)
        {
            return !left.Equals(right);
        }
    }
}