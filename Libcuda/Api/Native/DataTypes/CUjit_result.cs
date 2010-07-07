using System;
using System.Diagnostics;

namespace Libcuda.Api.Native.DataTypes
{
    [DebuggerNonUserCode]
    public class CUjit_result
    {
        public CUresult ErrorCode { get; set; }
        public CUmodule Module { get; set; }
        public TimeSpan WallTime { get; set; }

        private String _infoLog = String.Empty;
        public String InfoLog
        {
            get { return _infoLog; }
            set { _infoLog = (value ?? String.Empty).TrimEnd('\0'); }
        }

        private String _errorLog = String.Empty;
        public String ErrorLog
        {
            get { return _errorLog; }
            set { _errorLog = (value ?? String.Empty).TrimEnd('\0'); }
        }
    }
}