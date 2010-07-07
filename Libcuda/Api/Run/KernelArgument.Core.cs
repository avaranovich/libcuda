using System;
using System.Diagnostics;
using Libcuda.Api.Native;
using Libcuda.Api.Native.DataTypes;
using Libcuda.Exceptions;
using XenoGears.Assertions;
using XenoGears.Traits.Disposable;
using XenoGears.Unsafe;

namespace Libcuda.Api.Run
{
    [DebuggerNonUserCode]
    public partial class KernelArgument : Disposable
    {
        public ParameterDirection Direction { get; private set; }
        private Type _type;
        private Object _value; // will be reused when getting the result back from VRAM
        private CUdeviceptr _devicePtr;

        public int SizeInRAM { get { return _value.SizeOfUsefulData(); } }
        public int SizeInVRAM { get { return _devicePtr.IsNull ? 0 : SizeInRAM; } }
        public int SizeInArgList { get { return _devicePtr.IsNull ? SizeInRAM : typeof(CUdeviceptr).SizeOfUsefulData(); } }

        public KernelArgument(ParameterDirection direction, Object value)
        {
            Direction = direction;
            _type = value == null ? null : value.GetType();
            _value = value.AssertNotNull();

            Initialize();
        }

        public Object Value
        {
            get
            {
                switch (Direction)
                {
                    case ParameterDirection.In:
                        return _value;

                    case ParameterDirection.InOut:
                    case ParameterDirection.Out:
                        // if the result has already been acquired
                        // the original value is destroyed and cannot be read anymore
                        _alreadyAcquiredResult.AssertFalse();
                        return _value;

                    default:
                        throw AssertionHelper.Fail();
                }
            }
        }

        private bool _alreadyAcquiredResult = false;
        private readonly Object _resultAcquisitionLock = new Object();
        public Object Result
        {
            get
            {
                switch (Direction)
                {
                    case ParameterDirection.In:
                        throw AssertionHelper.Fail();

                    case ParameterDirection.InOut:
                    case ParameterDirection.Out:
                        lock (_resultAcquisitionLock)
                        {
                            if (!_alreadyAcquiredResult)
                            {
                                _value.GetType().IsArray.AssertTrue();
                                using (var rawMem = _value.Pin())
                                {
                                    nvcuda.cuMemcpyDtoH(rawMem, _devicePtr, (uint)SizeInVRAM);
                                    _alreadyAcquiredResult = true;
                                }
                            }

                            return _value;
                        }

                    default:
                        throw AssertionHelper.Fail();
                }
            }
        }

        protected override void DisposeUnmanagedResources()
        {
            if (_devicePtr.IsNotNull)
            {
                nvcuda.cuMemFree(_devicePtr);
            }
        }
    }
}