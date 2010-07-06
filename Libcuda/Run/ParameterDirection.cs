using System;

namespace Libcuda.Run
{
    [Flags]
    public enum ParameterDirection
    {
        In = 1,
        Out = 2,
        InOut = 3,
    }
}
