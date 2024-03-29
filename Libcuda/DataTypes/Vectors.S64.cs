﻿using System;
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
    public struct long1 : IEnumerable<long>, IEquatable<long1>
    {
        public long X;
        public long1(long x) { X = x; }

        IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }
        public IEnumerator<long> GetEnumerator() { return new[] { X }.Cast<long>().GetEnumerator(); }
        public override String ToString() { return String.Format("{0}{1}", typeof(long).Name, String.Format("({0})", X)); }

        public bool Equals(long1 other)
        {
            return Equals(other.X, X);
        }

        public override bool Equals(Object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (obj.GetType() != typeof(long1)) return false;
            return Equals((long1)obj);
        }

        public override int GetHashCode()
        {
            return X.GetHashCode();
        }

        public static bool operator ==(long1 left, long1 right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(long1 left, long1 right)
        {
            return !left.Equals(right);
        }
    }

    [DebuggerNonUserCode]
    [StructLayout(LayoutKind.Sequential)]
    public struct long2 : IEnumerable<long>, IEquatable<long2>
    {
        public long X;
        public long Y;

        public long2(long x) : this(x, default(long)) { }
        public long2(long x, long y) { X = x; Y = y; }

        IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }
        public IEnumerator<long> GetEnumerator() { return new[] { X, Y }.Cast<long>().GetEnumerator(); }
        public override String ToString() { return String.Format("{0}{1}", typeof(long).Name, String.Format("({0}, {1})", X, Y)); }

        public bool Equals(long2 other)
        {
            return Equals(other.X, X) && Equals(other.Y, Y);
        }

        public override bool Equals(Object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (obj.GetType() != typeof(long2)) return false;
            return Equals((long2)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (X.GetHashCode() * 397) ^ Y.GetHashCode();
            }
        }

        public static bool operator ==(long2 left, long2 right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(long2 left, long2 right)
        {
            return !left.Equals(right);
        }
    }
}