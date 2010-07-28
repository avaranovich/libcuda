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
    public struct half1 : IEnumerable<half>, IEquatable<half1>
    {
        public half X;
        public half1(half x) { X = x; }

        IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }
        public IEnumerator<half> GetEnumerator() { return new[] { X }.Cast<half>().GetEnumerator(); }
        public override String ToString() { return String.Format("{0}{1}", typeof(half).Name, String.Format("({0})", X)); }

        public bool Equals(half1 other)
        {
            return Equals(other.X, X);
        }

        public override bool Equals(Object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (obj.GetType() != typeof(half1)) return false;
            return Equals((half1)obj);
        }

        public override int GetHashCode()
        {
            return X.GetHashCode();
        }

        public static bool operator ==(half1 left, half1 right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(half1 left, half1 right)
        {
            return !left.Equals(right);
        }
    }

    [DebuggerNonUserCode]
    [StructLayout(LayoutKind.Sequential)]
    public struct half2 : IEnumerable<half>, IEquatable<half2>
    {
        public half X;
        public half Y;

        public half2(half x) : this(x, default(half)) { }
        public half2(half x, half y) { X = x; Y = y; }

        IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }
        public IEnumerator<half> GetEnumerator() { return new[] { X, Y }.Cast<half>().GetEnumerator(); }
        public override String ToString() { return String.Format("{0}{1}", typeof(half).Name, String.Format("({0}, {1})", X, Y)); }

        public bool Equals(half2 other)
        {
            return Equals(other.X, X) && Equals(other.Y, Y);
        }

        public override bool Equals(Object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (obj.GetType() != typeof(half2)) return false;
            return Equals((half2)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (X.GetHashCode() * 397) ^ Y.GetHashCode();
            }
        }

        public static bool operator ==(half2 left, half2 right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(half2 left, half2 right)
        {
            return !left.Equals(right);
        }
    }

    [DebuggerNonUserCode]
    [StructLayout(LayoutKind.Sequential)]
    public struct half3 : IEnumerable<half>, IEquatable<half3>
    {
        public half X;
        public half Y;
        public half Z;

        public half3(half x) : this(x, default(half), default(half)) { }
        public half3(half x, half y) : this(x, y, default(half)) { }
        public half3(half x, half y, half z) { X = x; Y = y; Z = z; }

        IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }
        public IEnumerator<half> GetEnumerator() { return new[] { X, Y, Z }.Cast<half>().GetEnumerator(); }
        public override String ToString() { return String.Format("{0}{1}", typeof(half).Name, String.Format("({0}, {1}, {2})", X, Y, Z)); }

        public bool Equals(half3 other)
        {
            return Equals(other.X, X) && Equals(other.Y, Y) && Equals(other.Z, Z);
        }

        public override bool Equals(Object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (obj.GetType() != typeof(half3)) return false;
            return Equals((half3)obj);
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

        public static bool operator ==(half3 left, half3 right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(half3 left, half3 right)
        {
            return !left.Equals(right);
        }
    }

    [DebuggerNonUserCode]
    [StructLayout(LayoutKind.Sequential)]
    public struct half4 : IEnumerable<half>, IEquatable<half4>
    {
        public half X;
        public half Y;
        public half Z;
        public half W;

        public half4(half x) : this(x, default(half), default(half), default(half)) { }
        public half4(half x, half y) : this(x, y, default(half), default(half)) { }
        public half4(half x, half y, half z) : this(x, y, z, default(half)) { }
        public half4(half x, half y, half z, half w) { X = x; Y = y; Z = z; W = w; }

        IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }
        public IEnumerator<half> GetEnumerator() { return new[] { X, Y, Z, W }.Cast<half>().GetEnumerator(); }
        public override String ToString() { return String.Format("{0}{1}", typeof(half).Name, String.Format("({0}, {1}, {2}, {3})", X, Y, Z, W)); }

        public bool Equals(half4 other)
        {
            return Equals(other.X, X) && Equals(other.Y, Y) && Equals(other.Z, Z) && Equals(other.W, W);
        }

        public override bool Equals(Object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (obj.GetType() != typeof(half4)) return false;
            return Equals((half4)obj);
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

        public static bool operator ==(half4 left, half4 right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(half4 left, half4 right)
        {
            return !left.Equals(right);
        }
    }
}