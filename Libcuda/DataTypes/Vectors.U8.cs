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
    public struct byte1 : IEnumerable<byte>, IEquatable<byte1>
    {
        public byte X;
        public byte1(byte x) { X = x; }

        IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }
        public IEnumerator<byte> GetEnumerator() { return new[] { X }.Cast<byte>().GetEnumerator(); }
        public override String ToString() { return String.Format("{0}{1}", typeof(byte).Name, String.Format("({0})", X)); }

        public bool Equals(byte1 other)
        {
            return Equals(other.X, X);
        }

        public override bool Equals(Object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (obj.GetType() != typeof(byte1)) return false;
            return Equals((byte1)obj);
        }

        public override int GetHashCode()
        {
            return X.GetHashCode();
        }

        public static bool operator ==(byte1 left, byte1 right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(byte1 left, byte1 right)
        {
            return !left.Equals(right);
        }
    }

    [DebuggerNonUserCode]
    [StructLayout(LayoutKind.Sequential)]
    public struct byte2 : IEnumerable<byte>, IEquatable<byte2>
    {
        public byte X;
        public byte Y;

        public byte2(byte x) : this(x, default(byte)) { }
        public byte2(byte x, byte y) { X = x; Y = y; }

        IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }
        public IEnumerator<byte> GetEnumerator() { return new[] { X, Y }.Cast<byte>().GetEnumerator(); }
        public override String ToString() { return String.Format("{0}{1}", typeof(byte).Name, String.Format("({0}, {1})", X, Y)); }

        public bool Equals(byte2 other)
        {
            return Equals(other.X, X) && Equals(other.Y, Y);
        }

        public override bool Equals(Object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (obj.GetType() != typeof(byte2)) return false;
            return Equals((byte2)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (X.GetHashCode() * 397) ^ Y.GetHashCode();
            }
        }

        public static bool operator ==(byte2 left, byte2 right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(byte2 left, byte2 right)
        {
            return !left.Equals(right);
        }
    }

    [DebuggerNonUserCode]
    [StructLayout(LayoutKind.Sequential)]
    public struct byte3 : IEnumerable<byte>, IEquatable<byte3>
    {
        public byte X;
        public byte Y;
        public byte Z;

        public byte3(byte x) : this(x, default(byte), default(byte)) { }
        public byte3(byte x, byte y) : this(x, y, default(byte)) { }
        public byte3(byte x, byte y, byte z) { X = x; Y = y; Z = z; }

        IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }
        public IEnumerator<byte> GetEnumerator() { return new[] { X, Y, Z }.Cast<byte>().GetEnumerator(); }
        public override String ToString() { return String.Format("{0}{1}", typeof(byte).Name, String.Format("({0}, {1}, {2})", X, Y, Z)); }

        public bool Equals(byte3 other)
        {
            return Equals(other.X, X) && Equals(other.Y, Y) && Equals(other.Z, Z);
        }

        public override bool Equals(Object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (obj.GetType() != typeof(byte3)) return false;
            return Equals((byte3)obj);
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

        public static bool operator ==(byte3 left, byte3 right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(byte3 left, byte3 right)
        {
            return !left.Equals(right);
        }
    }

    [DebuggerNonUserCode]
    [StructLayout(LayoutKind.Sequential)]
    public struct byte4 : IEnumerable<byte>, IEquatable<byte4>
    {
        public byte X;
        public byte Y;
        public byte Z;
        public byte W;

        public byte4(byte x) : this(x, default(byte), default(byte), default(byte)) { }
        public byte4(byte x, byte y) : this(x, y, default(byte), default(byte)) { }
        public byte4(byte x, byte y, byte z) : this(x, y, z, default(byte)) { }
        public byte4(byte x, byte y, byte z, byte w) { X = x; Y = y; Z = z; W = w; }

        IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }
        public IEnumerator<byte> GetEnumerator() { return new[] { X, Y, Z, W }.Cast<byte>().GetEnumerator(); }
        public override String ToString() { return String.Format("{0}{1}", typeof(byte).Name, String.Format("({0}, {1}, {2}, {3})", X, Y, Z, W)); }

        public bool Equals(byte4 other)
        {
            return Equals(other.X, X) && Equals(other.Y, Y) && Equals(other.Z, Z) && Equals(other.W, W);
        }

        public override bool Equals(Object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (obj.GetType() != typeof(byte4)) return false;
            return Equals((byte4)obj);
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

        public static bool operator ==(byte4 left, byte4 right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(byte4 left, byte4 right)
        {
            return !left.Equals(right);
        }
    }
}