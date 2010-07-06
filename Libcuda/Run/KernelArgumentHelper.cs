using System;
using System.Diagnostics;

namespace Libcuda.Run
{
    [DebuggerNonUserCode]
    public static class KernelArgumentHelper
    {
        public static KernelArgument In(this Object value)
        {
            return new KernelArgument(ParameterDirection.In, value);
        }

        public static KernelArgument Out(this Object value)
        {
            return new KernelArgument(ParameterDirection.Out, value);
        }

        public static KernelArgument InOut(this Object value)
        {
            return new KernelArgument(ParameterDirection.InOut, value);
        }
    }
}