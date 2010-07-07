using System;
using System.Diagnostics;
using Libcuda.Api.Native.DataTypes;
using XenoGears.Assertions;
using XenoGears.Exceptions;
using XenoGears.Strings;

namespace Libcuda.Exceptions
{
    [DebuggerNonUserCode]
    public class CudaException : BaseException
    {
        public CudaError Error { get; private set; }
        private readonly String _message;

        public CudaException(CUresult error)
            : this(error, null, null)
        {
        }

        public CudaException(CudaError error)
            : this(error, null, null)
        {
        }

        public CudaException(CUresult error, Exception innerException)
            : this(error, null, innerException)
        {
        }

        public CudaException(CudaError error, Exception innerException)
            : this(error, null, innerException)
        {
        }

        public CudaException(CUresult error, String message)
            : this(error, message, null)
        {
        }

        public CudaException(CudaError error, String message)
            : this(error, message, null)
        {
        }

        public CudaException(CUresult error, String message, Exception innerException)
            : this((CudaError)error, message, innerException)
        {
        }

        public CudaException(CudaError error, String message, Exception innerException)
            : base(innerException)
        {
            Error = error.AssertThat(ec => ec != CudaError.Success);
            _message = message;
        }

        [IncludeInMessage]
        public override bool IsUnexpected { get { return Error == CudaError.Unknown; } }

        [IncludeInMessage]
        public int ErrorCode { get { return (int)Error; } }

        [IncludeInMessage]
        public String ErrorDescription
        {
            get
            {
                var desc = Error.ToString();
                return desc.CSharpNameToHumanReadableString();
            }
        }

        [IncludeInMessage]
        public new String Message { get { return _message; } }
    }
}