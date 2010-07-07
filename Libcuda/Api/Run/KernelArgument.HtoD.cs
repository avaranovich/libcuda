using System;
using System.Runtime.InteropServices;
using Libcuda.DataTypes;
using Libcuda.Api.Native;
using XenoGears.Assertions;
using XenoGears.Functional;
using XenoGears.Reflection;
using XenoGears.Unsafe;

namespace Libcuda.Api.Run
{
    public partial class KernelArgument
    {
        private void CopyHtoD()
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
                (Direction == ParameterDirection.In).AssertTrue();
            }
            else
            {
                if (_type.IsValueType)
                {
                    (Direction == ParameterDirection.In).AssertTrue();
                }
                else if (_type.IsClass)
                {
                    if (_type.IsArray())
                    {
                        var array = _value.AssertCast<Array>();
                        var t_el = _type.GetElementType();
                        t_el.IsValueType.AssertTrue();

                        // allocate memory in VRAM
                        var sizeOfAlloc = (uint)(array.Dims().Product() * Marshal.SizeOf(t_el));
                        _devicePtr = nvcuda.cuMemAlloc(sizeOfAlloc);

                        // copy data if necessary
                        if (Direction == ParameterDirection.In ||
                            Direction == ParameterDirection.InOut)
                        {
                            using (var rawmem = _value.Pin())
                            {
                                nvcuda.cuMemcpyHtoD(_devicePtr, rawmem, sizeOfAlloc);
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
