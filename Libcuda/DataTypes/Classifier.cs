using System;
using System.Collections.Generic;
using System.Diagnostics;
using Half = Libcuda.DataTypes.half;

namespace Libcuda.DataTypes
{
    [DebuggerNonUserCode]
    public static class Classifier
    {
        private readonly static HashSet<Type> _primitives = new HashSet<Type>();
        private readonly static HashSet<Type> _vectors = new HashSet<Type>();

        static Classifier()
        {
            _primitives.Add(typeof(bool));
            _primitives.Add(typeof(SByte));
            _primitives.Add(typeof(Byte));
            _primitives.Add(typeof(Int16));
            _primitives.Add(typeof(UInt16));
            _primitives.Add(typeof(Int32));
            _primitives.Add(typeof(UInt32));
            _primitives.Add(typeof(Int64));
            _primitives.Add(typeof(UInt64));
            _primitives.Add(typeof(Half));
            _primitives.Add(typeof(Single));
            _primitives.Add(typeof(Double));

            _vectors.Add(typeof(dim3));
            _vectors.Add(typeof(sbyte1));
            _vectors.Add(typeof(sbyte2));
            _vectors.Add(typeof(sbyte3));
            _vectors.Add(typeof(sbyte4));
            _vectors.Add(typeof(byte1));
            _vectors.Add(typeof(byte2));
            _vectors.Add(typeof(byte3));
            _vectors.Add(typeof(byte4));
            _vectors.Add(typeof(short1));
            _vectors.Add(typeof(short2));
            _vectors.Add(typeof(short3));
            _vectors.Add(typeof(short4));
            _vectors.Add(typeof(ushort1));
            _vectors.Add(typeof(ushort2));
            _vectors.Add(typeof(ushort3));
            _vectors.Add(typeof(ushort4));
            _vectors.Add(typeof(int1));
            _vectors.Add(typeof(int2));
            _vectors.Add(typeof(int3));
            _vectors.Add(typeof(int4));
            _vectors.Add(typeof(uint1));
            _vectors.Add(typeof(uint2));
            _vectors.Add(typeof(uint3));
            _vectors.Add(typeof(uint4));
            _vectors.Add(typeof(long1));
            _vectors.Add(typeof(long2));
            _vectors.Add(typeof(ulong1));
            _vectors.Add(typeof(ulong2));
            _vectors.Add(typeof(half1));
            _vectors.Add(typeof(half2));
            _vectors.Add(typeof(half3));
            _vectors.Add(typeof(half4));
            _vectors.Add(typeof(float1));
            _vectors.Add(typeof(float2));
            _vectors.Add(typeof(float3));
            _vectors.Add(typeof(float4));
            _vectors.Add(typeof(double1));
            _vectors.Add(typeof(double2));
        }

        public static bool IsCudaPrimitive(this Type t)
        {
            return _primitives.Contains(t);
        }

        public static bool IsCudaPrimitive(this Object o)
        {
            return o != null && o.GetType().IsCudaPrimitive();
        }

        public static bool IsCudaVector(this Type t)
        {
            return _vectors.Contains(t);
        }

        public static bool IsCudaVector(this Object o)
        {
            return o != null && o.GetType().IsCudaVector();
        }
    }
}