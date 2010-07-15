using System;
using System.Diagnostics;
using Libcuda.Api.DataTypes;
using XenoGears.Functional;

namespace Libcuda.Api.Native.DataTypes
{
    [DebuggerNonUserCode]
    public class CUjit_result
    {
        public CUresult ErrorCode { get; set; }
        public CUmodule Module { get; set; }
        public ElapsedTime WallTime { get; set; }

        private String _infoLog = String.Empty;
        public String InfoLog
        {
            get { return _infoLog.IsNullOrEmpty() ? null : _infoLog; }
            set { _infoLog = (value ?? String.Empty).TrimEnd('\0'); }
        }

        private String _errorLog = String.Empty;
        public String ErrorLog
        {
            get { return _errorLog.IsNullOrEmpty() ? null : _errorLog; }
            set { _errorLog = (value ?? String.Empty).TrimEnd('\0'); }
        }
    }
}