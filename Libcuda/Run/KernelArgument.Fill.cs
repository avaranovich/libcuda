using Libcuda.DataTypes;
using Libcuda.Native;
using Libcuda.Native.Exceptions;
using XenoGears.Assertions;
using XenoGears.Reflection;

namespace Libcuda.Run
{
    public partial class KernelArgument
    {
        internal void Fill(KernelInvocation invocation, int offset)
        {
            if (_type.IsCudaPrimitive())
            {
                if (_type.IsInteger())
                {
                    (_type == typeof(int) || _type == typeof(uint)).AssertTrue();
                    (Direction == ParameterDirection.In).AssertTrue();
                    var value = _value.AssertCoerce<uint>();
                    var error = nvcuda.cuParamSeti(invocation.Function.Handle, offset, value);
                    if (error != CUresult.Success) throw new CudaException(error);
                }
                else if (_type.IsFloatingPoint())
                {
                    (_type == typeof(float)).AssertTrue();
                    (Direction == ParameterDirection.In).AssertTrue();
                    var value = _value.AssertCoerce<float>();
                    var error = nvcuda.cuParamSetf(invocation.Function.Handle, offset, value);
                    if (error != CUresult.Success) throw new CudaException(error);
                }
                else
                {
                    throw AssertionHelper.Fail();
                }
            }
            else if (_type.IsCudaVector())
            {
                throw AssertionHelper.Fail();
            }
            else
            {
                if (_type.IsValueType)
                {
                    throw AssertionHelper.Fail();
                }
                else if (_type.IsClass)
                {
                    if (_value.IsArray())
                    {
                        var error = nvcuda.cuParamSeti(invocation.Function.Handle, offset, _devicePtr);
                        if (error != CUresult.Success) throw new CudaException(error);
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
