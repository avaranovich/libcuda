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
    public struct float1 : IEnumerable<float>, IEquatable<float1>
    {
        public float X;
        public float1(float x) { X = x; }

        IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }
        public IEnumerator<float> GetEnumerator() { return new[] { X }.Cast<float>().GetEnumerator(); }
        public override String ToString() { return String.Format("{0}{1}", typeof(float).Name, String.Format("({0})", X)); }

        public bool Equals(float1 other)
        {
            return Equals(other.X, X);
        }

        public override bool Equals(Object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (obj.GetType() != typeof(float1)) return false;
            return Equals((float1)obj);
        }

        public override int GetHashCode()
        {
            return X.GetHashCode();
        }

        public static bool operator ==(float1 left, float1 right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(float1 left, float1 right)
        {
            return !left.Equals(right);
        }
    }

    [DebuggerNonUserCode]
    [StructLayout(LayoutKind.Sequential)]
    public struct float2 : IEnumerable<float>, IEquatable<float2>
    {
        public float X;
        public float Y;

        public float2(float x) : this(x, default(float)) { }
        public float2(float x, float y) { X = x; Y = y; }

        IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }
        public IEnumerator<float> GetEnumerator() { return new[] { X, Y }.Cast<float>().GetEnumerator(); }
        public override String ToString() { return String.Format("{0}{1}", typeof(float).Name, String.Format("({0}, {1})", X, Y)); }

        public bool Equals(float2 other)
        {
            return Equals(other.X, X) && Equals(other.Y, Y);
        }

        public override bool Equals(Object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (obj.GetType() != typeof(float2)) return false;
            return Equals((float2)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (X.GetHashCode() * 397) ^ Y.GetHashCode();
            }
        }

        public static bool operator ==(float2 left, float2 right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(float2 left, float2 right)
        {
            return !left.Equals(right);
        }
    }

    [DebuggerNonUserCode]
    [StructLayout(LayoutKind.Sequential)]
    public struct float3 : IEnumerable<float>, IEquatable<float3>
    {
        public float X;
        public float Y;
        public float Z;

        public float3(float x) : this(x, default(float), default(float)) { }
        public float3(float x, float y) : this(x, y, default(float)) { }
        public float3(float x, float y, float z) { X = x; Y = y; Z = z; }

        IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }
        public IEnumerator<float> GetEnumerator() { return new[] { X, Y, Z }.Cast<float>().GetEnumerator(); }
        public override String ToString() { return String.Format("{0}{1}", typeof(float).Name, String.Format("({0}, {1}, {2})", X, Y, Z)); }

        public bool Equals(float3 other)
        {
            return Equals(other.X, X) && Equals(other.Y, Y) && Equals(other.Z, Z);
        }

        public override bool Equals(Object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (obj.GetType() != typeof(float3)) return false;
            return Equals((float3)obj);
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

        public static bool operator ==(float3 left, float3 right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(float3 left, float3 right)
        {
            return !left.Equals(right);
        }
    }

    [DebuggerNonUserCode]
    [StructLayout(LayoutKind.Sequential)]
    public struct float4 : IEnumerable<float>, IEquatable<float4>
    {
        public float X;
        public float Y;
        public float Z;
        public float W;

        public float4(float x) : this(x, default(float), default(float), default(float)) { }
        public float4(float x, float y) : this(x, y, default(float), default(float)) { }
        public float4(float x, float y, float z) : this(x, y, z, default(float)) { }
        public float4(float x, float y, float z, float w) { X = x; Y = y; Z = z; W = w; }

        IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }
        public IEnumerator<float> GetEnumerator() { return new[] { X, Y, Z, W }.Cast<float>().GetEnumerator(); }
        public override String ToString() { return String.Format("{0}{1}", typeof(float).Name, String.Format("({0}, {1}, {2}, {3})", X, Y, Z, W)); }

        public bool Equals(float4 other)
        {
            return Equals(other.X, X) && Equals(other.Y, Y) && Equals(other.Z, Z) && Equals(other.W, W);
        }

        public override bool Equals(Object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (obj.GetType() != typeof(float4)) return false;
            return Equals((float4)obj);
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

        public static bool operator ==(float4 left, float4 right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(float4 left, float4 right)
        {
            return !left.Equals(right);
        }
    }
}