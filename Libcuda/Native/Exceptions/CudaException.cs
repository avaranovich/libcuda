using System;
using System.Diagnostics;
using XenoGears.Assertions;
using XenoGears.Exceptions;
using XenoGears.Strings;

namespace Libcuda.Native.Exceptions
{
    [DebuggerNonUserCode]
    public class CudaException : BaseException
    {
        private readonly CUresult _errorCode;
        private readonly String _customMessage;

        public CudaException(CUresult errorCode)
            : this(errorCode, null)
        {
        }

        public CudaException(CUresult errorCode, String customMessage)
        {
            _errorCode = errorCode.AssertThat(ec => ec != CUresult.Success);
            _customMessage = customMessage;
        }

        [IncludeInMessage]
        public override bool IsUnexpected { get { return _errorCode == CUresult.ErrorUnknown; } }

        [IncludeInMessage]
        public int ErrorCode { get { return (int)_errorCode; } }

        [IncludeInMessage]
        public String ErrorDescription
        {
            get
            {
                var desc = _errorCode.ToString();
                if (desc.IndexOf("Error") != -1) 
                    desc = desc.Substring(desc.IndexOf("Error") + "Error".Length);
                return desc.CSharpNameToHumanReadableString();
            }
        }

        [IncludeInMessage]
        public String CustomMessage { get { return _customMessage; } }
    }
}