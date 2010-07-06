using System;
using System.Diagnostics;
using Libcuda.Native.Exceptions;
using XenoGears.Exceptions;
using XenoGears.Strings;

namespace Libcuda.Jit
{
    [DebuggerNonUserCode]
    public class JitException : CudaException
    {
        private readonly JitResult _result;

        public JitException(JitResult result)
            : base(result.ErrorCode)
        {
            _result = result;
        }

        [IncludeInMessage]
        public TimeSpan WallTime { get { return _result.CompilationWallTime; } }

        [IncludeInMessage]
        public String InfoLog { get { return Environment.NewLine + _result.CompilationInfoLog.Indent(); } }

        [IncludeInMessage]
        public String ErrorLog { get { return Environment.NewLine + _result.CompilationErrorLog.Indent(); } }
    }
}