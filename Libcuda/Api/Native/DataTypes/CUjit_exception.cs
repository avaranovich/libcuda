using System;
using System.Diagnostics;
using Libcuda.Exceptions;
using XenoGears.Assertions;
using XenoGears.Exceptions;

namespace Libcuda.Api.Native.DataTypes
{
    [DebuggerNonUserCode]
    public class CUjit_exception : CudaException
    {
        public CUjit_result JitResult { get; private set; }

        public CUjit_exception(CUjit_result jitResult)
            : base(jitResult.ErrorCode)
        {
            JitResult = jitResult.AssertNotNull();
        }

        [IncludeInMessage]
        public TimeSpan WallTime { get { return JitResult.WallTime; } }

        [IncludeInMessage]
        public String InfoLog { get { return JitResult.InfoLog; } }

        [IncludeInMessage]
        public String ErrorLog { get { return JitResult.ErrorLog; } }
    }
}