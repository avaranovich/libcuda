using System;
using System.Runtime.InteropServices;
using Libcuda.DataTypes;
using Libcuda.Native;
using Libcuda.Native.Exceptions;
using XenoGears.Assertions;
using XenoGears.Functional;
using XenoGears.Reflection;
using XenoGears.Unsafe;

namespace Libcuda.Run
{
    public partial class KernelArgument
    {
        private void Initialize()
        {
            if (_type.IsCudaPrimitive())
            {
                if (_type.IsInteger())
                {
                    (_type == typeof(int) || _type == typeof(uint)).AssertTrue();
                    (Direction == ParameterDirection.In).AssertTrue();
                }
                else if (_type.IsFloatingPoint())
                {
                    (_type == typeof(float)).AssertTrue();
                    (Direction == ParameterDirection.In).AssertTrue();
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
                        var array = _value.AssertCast<Array>();
                        var t_el = _type.GetElementType();
                        t_el.IsValueType.AssertTrue();

                        // allocate memory in VRAM
                        var sizeOfAlloc = (uint)(array.Dims().Product() * Marshal.SizeOf(t_el));
                        var error1 = nvcuda.cuMemAlloc(out _devicePtr, sizeOfAlloc);
                        if (error1 != CUresult.Success) throw new CudaException(error1);

                        // copy data if necessary
                        if (Direction == ParameterDirection.In ||
                            Direction == ParameterDirection.InOut)
                        {
                            using (var rawmem = _value.Pin())
                            {
                                var error2 = nvcuda.cuMemcpyHtoD(_devicePtr, rawmem, sizeOfAlloc);
                                if (error2 != CUresult.Success) throw new CudaException(error2);
                            }
                        }
                        else if (Direction == ParameterDirection.Out)
                        {
                            // do nothing
                        }
                        else
                        {
                            throw AssertionHelper.Fail();
                        }
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
