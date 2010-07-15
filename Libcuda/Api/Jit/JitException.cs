using System;
using System.Diagnostics;
using Libcuda.Api.DataTypes;
using Libcuda.Exceptions;
using XenoGears.Exceptions;
using XenoGears.Assertions;

namespace Libcuda.Api.Jit
{
    [DebuggerNonUserCode]
    public class JitException : CudaException
    {
        public JitResult JitResult { get; private set; }

        public JitException(JitResult jitResult)
            : this(jitResult, null)
        {
        }

        public JitException(JitResult jitResult, Exception innerException)
            : base(jitResult.ErrorCode, innerException)
        {
            JitResult = jitResult.AssertNotNull();
        }

        [IncludeInMessage]
        public ElapsedTime WallTime { get { return JitResult.CompilationWallTime; } }

        [IncludeInMessage]
        public String InfoLog { get { return JitResult.CompilationInfoLog; } }

        [IncludeInMessage]
        public String ErrorLog { get { return JitResult.CompilationErrorLog; } }
    }
}