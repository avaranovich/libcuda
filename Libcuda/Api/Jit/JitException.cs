using System;
using System.Diagnostics;
using Libcuda.Exceptions;
using XenoGears.Exceptions;
using XenoGears.Assertions;
using XenoGears.Strings;

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
        public TimeSpan WallTime { get { return JitResult.CompilationWallTime; } }

        [IncludeInMessage]
        public String InfoLog { get { return Environment.NewLine + JitResult.CompilationInfoLog.Indent(); } }

        [IncludeInMessage]
        public String ErrorLog { get { return Environment.NewLine + JitResult.CompilationErrorLog.Indent(); } }
    }
}