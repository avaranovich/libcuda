using System;
using Libcuda.Api.Native;
using Libcuda.DataTypes;
using XenoGears.Assertions;
using XenoGears.Reflection;
using XenoGears.Unsafe;

namespace Libcuda.Api.Run
{
    public partial class KernelArgument
    {
        private void CopyDtoH()
        {
            Direction.IsOut().AssertTrue();

            if (_type.IsCudaPrimitive())
            {
                throw AssertionHelper.Fail();
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
                    if (_type.IsArray())
                    {
                        var t_el = _type.GetElementType();
                        t_el.IsValueType.AssertTrue();

                        using (var rawMem = _value.Pin())
                        {
                            nvcuda.cuMemcpyDtoH(rawMem, _devicePtr, (uint)SizeInVRAM);
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
