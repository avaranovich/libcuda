using Libcuda.DataTypes;
using Libcuda.Api.Native;
using XenoGears.Assertions;
using XenoGears.Reflection;

namespace Libcuda.Api.Run
{
    public partial class KernelArgument
    {
        internal void PassInto(KernelInvocation invocation, int offset)
        {
            if (_type.IsCudaPrimitive())
            {
                if (_type.IsInteger())
                {
                    (_type == typeof(int) || _type == typeof(uint)).AssertTrue();
                    (Direction == ParameterDirection.In).AssertTrue();
                    var value = _value.AssertCoerce<uint>();
                    nvcuda.cuParamSeti(invocation.Function.Handle, offset, value);
                }
                else if (_type.IsFloatingPoint())
                {
                    (_type == typeof(float)).AssertTrue();
                    (Direction == ParameterDirection.In).AssertTrue();
                    var value = _value.AssertCoerce<float>();
                    nvcuda.cuParamSetf(invocation.Function.Handle, offset, value);
                }
                else
                {
                    throw AssertionHelper.Fail();
                }
            }
            else if (_type.IsCudaVector())
            {
                (Direction == ParameterDirection.In).AssertTrue();
                nvcuda.cuParamSetv(invocation.Function.Handle, offset, _value);
            }
            else
            {
                if (_type.IsValueType)
                {
                    (Direction == ParameterDirection.In).AssertTrue();
                    nvcuda.cuParamSetv(invocation.Function.Handle, offset, _value);
                }
                else if (_type.IsClass)
                {
                    if (_value.IsArray())
                    {
                        nvcuda.cuParamSeti(invocation.Function.Handle, offset, _devicePtr);
                    }
                    else
                    {
                        throw AssertionHelper.Fail();
                    }
                }
            }
        }
    }
}
