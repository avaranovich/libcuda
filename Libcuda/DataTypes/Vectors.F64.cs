using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Libcuda.DataTypes
{
    [DebuggerNonUserCode]
    public struct double1 : IEnumerable<double>, IEquatable<double1>
    {
        public double X;
        public double1(double x) { X = x; }

        IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }
        public IEnumerator<double> GetEnumerator() { return new[] { X }.Cast<double>().GetEnumerator(); }
        public override String ToString() { return String.Format("{0}{1}", typeof(double).Name, String.Format("({0})", X)); }

        public bool Equals(double1 other)
        {
            return Equals(other.X, X);
        }

        public override bool Equals(Object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (obj.GetType() != typeof(double1)) return false;
            return Equals((double1)obj);
        }

        public override int GetHashCode()
        {
            return X.GetHashCode();
        }

        public static bool operator ==(double1 left, double1 right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(double1 left, double1 right)
        {
            return !left.Equals(right);
        }
    }

    [DebuggerNonUserCode]
    public struct double2 : IEnumerable<double>, IEquatable<double2>
    {
        public double X;
        public double Y;

        public double2(double x) : this(x, default(double)) { }
        public double2(double x, double y) { X = x; Y = y; }

        IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }
        public IEnumerator<double> GetEnumerator() { return new[] { X, Y }.Cast<double>().GetEnumerator(); }
        public override String ToString() { return String.Format("{0}{1}", typeof(double).Name, String.Format("({0}, {1})", X, Y)); }

        public bool Equals(double2 other)
        {
            return Equals(other.X, X) && Equals(other.Y, Y);
        }

        public override bool Equals(Object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (obj.GetType() != typeof(double2)) return false;
            return Equals((double2)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (X.GetHashCode() * 397) ^ Y.GetHashCode();
            }
        }

        public static bool operator ==(double2 left, double2 right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(double2 left, double2 right)
        {
            return !left.Equals(right);
        }
    }
}