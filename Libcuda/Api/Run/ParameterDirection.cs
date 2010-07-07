using System;
using XenoGears.Assertions;

namespace Libcuda.Api.Run
{
    [Flags]
    public enum ParameterDirection
    {
        In = 1,
        Out = 2,
        InOut = 3,
    }

    public static class ParameterDirectionHelper
    {
        public static bool IsIn(this ParameterDirection direction)
        {
            switch (direction)
            {
                case ParameterDirection.In:
                    return true;
                case ParameterDirection.Out:
                    return false;
                case ParameterDirection.InOut:
                    return true;
                default:
                    throw AssertionHelper.Fail();
            }
        }

        public static bool IsOut(this ParameterDirection direction)
        {
            switch (direction)
            {
                case ParameterDirection.In:
                    return false;
                case ParameterDirection.Out:
                    return true;
                case ParameterDirection.InOut:
                    return true;
                default:
                    throw AssertionHelper.Fail();
            }
        }
    }
}
