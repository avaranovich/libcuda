using System;
using System.Diagnostics;
using System.Reflection;
using Libcuda.Api.Native;
using Libcuda.Api.Native.DataTypes;
using XenoGears.Assertions;
using XenoGears.Exceptions;
using XenoGears.Strings;

namespace Libcuda.Exceptions
{
    [DebuggerNonUserCode]
    public class CudaException : BaseException
    {
        private readonly CudaError _errorCode;
        private readonly MethodBase _source;
        private readonly String _customMessage;

        public CudaException(CUresult errorCode)
            : this(errorCode, null, null)
        {
        }

        public CudaException(CudaError errorCode)
            : this(errorCode, null, null)
        {
        }

        public CudaException(CUresult errorCode, Exception innerException)
            : this(errorCode, null, innerException)
        {
        }

        public CudaException(CudaError errorCode, Exception innerException)
            : this(errorCode, null, innerException)
        {
        }

        public CudaException(CUresult errorCode, String customMessage)
            : this(errorCode, customMessage, null)
        {
        }

        public CudaException(CudaError errorCode, String customMessage)
            : this(errorCode, customMessage, null)
        {
        }

        public CudaException(CUresult errorCode, String customMessage, Exception innerException)
            : this((CudaError)errorCode, customMessage, innerException)
        {
        }

        public CudaException(CudaError errorCode, String customMessage, Exception innerException)
            : base(innerException)
        {
            _errorCode = errorCode.AssertThat(ec => ec != CudaError.Success);
            var m_caller = new StackTrace().GetFrame(1).GetMethod();
            var t_caller = m_caller == null ? null : m_caller.DeclaringType;
            _source = t_caller == typeof(nvcuda) ? m_caller : null;
            _customMessage = customMessage;
        }

        [IncludeInMessage]
        public override bool IsUnexpected { get { return _errorCode == CudaError.Unknown; } }

        [IncludeInMessage]
        public CudaError Error { get { return _errorCode; } }

        [IncludeInMessage]
        public int ErrorCode { get { return (int)_errorCode; } }

        [IncludeInMessage]
        public String ErrorDescription
        {
            get
            {
                var desc = _errorCode.ToString();
                return desc.CSharpNameToHumanReadableString();
            }
        }

        [IncludeInMessage]
        public override String Source
        {
            get { return _source == null ? null : _source.GetCSharpRef(ToCSharpOptions.Informative); }
        }

        [IncludeInMessage]
        public String CustomMessage { get { return _customMessage; } }
    }
}