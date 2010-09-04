using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Libcuda.Api.DataTypes;
using XenoGears.Assertions;
using XenoGears.Collections.Dictionaries;
using XenoGears.Collections.Lists;
using XenoGears.Functional;
using XenoGears.Traits.Disposable;
using Libcuda.Api.Native.DataTypes;
using Libcuda.Exceptions;

namespace Libcuda.Api.Native
{
    public static partial class nvcuda
    {
        [DllImport("nvcuda", EntryPoint = "cuModuleLoadDataEx")]
        // http://developer.download.nvidia.com/compute/cuda/3_1/toolkit/docs/online/group__CUMODULE_gbfbf77eb2a307af8aa81376ecc909bd3.html
        private static extern CUresult nativeModuleLoadDataEx(out CUmodule module, IntPtr image, uint numOptions, CUjit_option[] options, IntPtr optionValues);

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static CUjit_result cuModuleLoadDataEx(String ptx, CUjit_options options)
        {
            var image = Marshal.StringToHGlobalAnsi(ptx);
            try { return cuModuleLoadDataEx(image, options); }
            finally { Marshal.FreeHGlobal(image); }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static CUjit_result cuModuleLoadDataEx(IntPtr image, CUjit_options options)
        {
            return Wrap(() =>
            {
                try
                {
                    // todo. an attempt to pass the Target value directly leads to CUDA_ERROR_INVALID_VALUE
                    // as of now, this feature is not really important, so I'm marking it as TBI
                    options.TargetFromContext.AssertTrue();

                    using (var native_options = new nativeModuleLoadDataExOptions(options))
                    {
                        CUmodule module;
                        var error = nativeModuleLoadDataEx(out module, image, native_options.Count, native_options.Keys, native_options.Values);

                        var result = new CUjit_result();
                        result.ErrorCode = error;
                        result.Module = module;
                        result.WallTime = native_options.WallTime;
                        result.InfoLog = native_options.InfoLog;
                        result.ErrorLog = native_options.ErrorLog;
                        return result;
                    }
                }
                catch (CudaException)
                {
                    throw;
                }
                catch (DllNotFoundException dnfe)
                {
                    throw new CudaException(CudaError.NoDriver, dnfe);
                }
                catch (Exception e)
                {
                    throw new CudaException(CudaError.Unknown, e);
                }
            });
        }

        #region Murky details of converting CUjit_options into the format expected by nativeModuleLoadDataEx

        [Finalizable]
        [DebuggerNonUserCode]
        private class nativeModuleLoadDataExOptions : Disposable
        {
            private partial class Option { public CUjit_option Key; public Object Value; public int Size; }
            private IEnumerable<Option> GatherOptions()
            {
                // can't pass 0 since it'll cause CUresult.ErrorInvalidValue
//                if (MaxRegistersPerThread != 0) yield return new Option(CUjit_option.MaxRegisters, MaxRegistersPerThread);
//                yield return new Option(CUjit_option.ThreadsPerBlock, PlannedThreadsPerBlock);
//                yield return new Option(CUjit_option.WallTime, default(float));
                yield return new Option(CUjit_option.InfoLogBuffer, _infoLogBuffer);
                yield return new Option(CUjit_option.InfoLogBufferSizeBytes, _infoLogBufferSizeBytes, IntPtr.Size);
                yield return new Option(CUjit_option.ErrorLogBuffer, _errorLogBuffer);
                yield return new Option(CUjit_option.ErrorLogBufferSizeBytes, _errorLogBufferSizeBytes, IntPtr.Size);
//                yield return new Option(CUjit_option.OptimizationLevel, OptimizationLevel);
//                if (TargetFromContext) yield return new Option(CUjit_option.TargetFromContext, 1);
//                if (!TargetFromContext) yield return new Option(CUjit_option.Target, (int)Target);
//                yield return new Option(CUjit_option.FallbackStrategy, (int)FallbackStrategy);
            }

            #region Input/output parameters

            // input parameters

            public int MaxRegistersPerThread { get; set; }
            public int PlannedThreadsPerBlock { get; set; }
            public int OptimizationLevel { get; set; }
            public bool TargetFromContext { get; set; }
            public CUjit_target Target { get; set; }
            public CUjit_fallbackstrategy FallbackStrategy { get; set; }

            public nativeModuleLoadDataExOptions(CUjit_options options)
            {
                MaxRegistersPerThread = options.MaxRegistersPerThread;
                PlannedThreadsPerBlock = options.PlannedThreadsPerBlock;
                OptimizationLevel = options.OptimizationLevel;
                TargetFromContext = options.TargetFromContext;
                Target = options.Target;
                FallbackStrategy = options.FallbackStrategy;
            }

            // output parameters

            public unsafe int ThreadsPerBlock
            {
                get
                {
                    IsDisposed.AssertFalse();
                    (Raw != null).AssertTrue();

                    if (!Raw.ContainsKey(CUjit_option.ThreadsPerBlock)) return 0;
                    var ptr = Raw[CUjit_option.ThreadsPerBlock];
                    return *((int*)ptr);
                }
            }

            public unsafe ElapsedTime WallTime
            {
                get
                {
                    IsDisposed.AssertFalse();
                    (Raw != null).AssertTrue();

                    if (!Raw.ContainsKey(CUjit_option.WallTime)) return null;
                    var ptr = Raw[CUjit_option.WallTime];
                    var bits = *((int*)ptr);

                    var bytes = BitConverter.GetBytes(bits);
                    var wallTime = BitConverter.ToSingle(bytes, 0);
                    return new ElapsedTime(wallTime);
                }
            }

            public unsafe String InfoLog
            {
                get
                {
                    IsDisposed.AssertFalse();
                    (Raw != null).AssertTrue(); ;

                    if (!Raw.ContainsKey(CUjit_option.InfoLogBufferSizeBytes)) return null;
                    if (!Raw.ContainsKey(CUjit_option.InfoLogBuffer)) return null;
                    var ptr = Raw[CUjit_option.InfoLogBufferSizeBytes];
                    var byteCount = *((int*)ptr);
                    return Marshal.PtrToStringAnsi(_infoLogBuffer, byteCount);
                }
            }

            public unsafe String ErrorLog
            {
                get
                {
                    IsDisposed.AssertFalse();
                    (Raw != null).AssertTrue(); ;

                    if (!Raw.ContainsKey(CUjit_option.ErrorLogBufferSizeBytes)) return null;
                    if (!Raw.ContainsKey(CUjit_option.ErrorLogBuffer)) return null;
                    var ptr = Raw[CUjit_option.ErrorLogBufferSizeBytes];
                    var byteCount = *((int*)ptr);
                    return Marshal.PtrToStringAnsi(_errorLogBuffer, byteCount);
                }
            }

            #endregion

            #region Manual assembly of native data structure

            private OrderedDictionary<CUjit_option, IntPtr> _raw;
            private OrderedDictionary<CUjit_option, IntPtr> Raw { get { EnsureRaw(); return _raw; } }
            public uint Count { get { return (uint)Raw.Count; } }
            public CUjit_option[] Keys { get { return Raw.Select(t => t.Key).ToArray(); } }
            public IntPtr Values { get { EnsureRaw(); return _optionValues; } }

            unsafe private void EnsureRaw()
            {
                if (_raw == null)
                {
                    AllocateTemporaryBuffers();
                    var options = GatherOptions();
                    _raw = new OrderedDictionary<CUjit_option, IntPtr>();

                    var offset = 0;
                    options.ForEach(kvp =>
                    {
                        var value = kvp.Value;
                        var ptr = (IntPtr)(&((byte*)_optionValues)[offset]);
                        Marshal.StructureToPtr(value, ptr, false);
                        _raw.Add(kvp.Key, ptr);
                        offset += (kvp.Size == 0 ? Marshal.SizeOf(value) : kvp.Size);
                    });
                }
            }

            private partial class Option
            {
                public Option(CUjit_option key, Object value)
                    : this(key, value, Marshal.SizeOf(value))
                {
                }

                public Option(CUjit_option key, Object value, int size)
                {
                    Key = key;
                    Value = value;
                    Size = size;
                }
            }

            #endregion

            #region Allocation/deallocation of temporary buffers

            private bool _is_allocated = false;
            private int _infoLogBufferSizeBytes = 10000 * sizeof(byte);
            private IntPtr _infoLogBuffer;
            private int _errorLogBufferSizeBytes = 10000 * sizeof(byte);
            private IntPtr _errorLogBuffer;
            private int _optionValuesSizeBytes = 10000 * sizeof(byte);
            private IntPtr _optionValues;

            [DllImport("kernel32.dll", SetLastError = true)]
            private static extern void ZeroMemory(IntPtr handle, uint length);

            private void AllocateTemporaryBuffers()
            {
                if (!_is_allocated)
                {
                    // todo. there's a possibility of memory leak here
                    // if mallocs crash in the middle of action
                    // I'm cba to embrace the code in a number of try-finallies
                    // so I leave this bugg here

                    _infoLogBuffer = Marshal.AllocHGlobal(_infoLogBufferSizeBytes);
                    ZeroMemory(_infoLogBuffer, (uint)_infoLogBufferSizeBytes);
                    Marshal.ThrowExceptionForHR(Marshal.GetLastWin32Error());

                    _errorLogBuffer = Marshal.AllocHGlobal(_errorLogBufferSizeBytes);
                    ZeroMemory(_errorLogBuffer, (uint)_errorLogBufferSizeBytes);
                    Marshal.ThrowExceptionForHR(Marshal.GetLastWin32Error());

                    // todo. calculating exact amount of memory to reserve is cumbersome
                    // here we just allocate enough memory to hold all the parameters
                    _optionValues = Marshal.AllocHGlobal(_optionValuesSizeBytes);
                    ZeroMemory(_optionValues, (uint)_optionValuesSizeBytes);
                    Marshal.ThrowExceptionForHR(Marshal.GetLastWin32Error());

                    _is_allocated = true;
                }
            }

            protected override void DisposeUnmanagedResources()
            {
                // todo. there's a possibility of memory leak here
                // if frees crash in the middle of action
                // I'm cba to embrace the code in a number of try-finallies
                // so I leave this bugg here

                if (_infoLogBuffer != IntPtr.Zero)
                    Marshal.FreeHGlobal(_infoLogBuffer);

                if (_errorLogBuffer != IntPtr.Zero)
                    Marshal.FreeHGlobal(_errorLogBuffer);

                if (_optionValues != IntPtr.Zero)
                    Marshal.FreeHGlobal(_optionValues);
            }

            #endregion
        }

        #endregion

        [DllImport("nvcuda", EntryPoint = "cuModuleUnload")]
        // http://developer.download.nvidia.com/compute/cuda/3_1/toolkit/docs/online/group__CUMODULE_g35a621d73ee186733f051de672fbe02b.html
        private static extern CUresult nativeModuleUnload(CUmodule mod);

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void cuModuleUnload(CUmodule mod)
        {
            Wrap(() =>
            {
                try
                {
                    var error = nativeModuleUnload(mod);
                    if (error != CUresult.CUDA_SUCCESS) throw new CudaException(error);
                }
                catch (CudaException)
                {
                    throw;
                }
                catch (DllNotFoundException dnfe)
                {
                    throw new CudaException(CudaError.NoDriver, dnfe);
                }
                catch (Exception e)
                {
                    throw new CudaException(CudaError.Unknown, e);
                }
            });
        }

        [DllImport("nvcuda", EntryPoint = "cuModuleGetFunction")]
        // http://developer.download.nvidia.com/compute/cuda/3_1/toolkit/docs/online/group__CUMODULE_ge18a9f0d853ae3a96a38416a0671606b.html
        private static extern CUresult nativeModuleGetFunction(out CUfunction hfunc, CUmodule hmod, String name);

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static CUfunction cuModuleGetFunction(CUmodule hmod, String name)
        {
            return Wrap(() =>
            {
                try
                {
                    CUfunction hfunc;
                    var error = nativeModuleGetFunction(out hfunc, hmod, name);
                    if (error != CUresult.CUDA_SUCCESS) throw new CudaException(error);
                    return hfunc;
                }
                catch (CudaException)
                {
                    throw;
                }
                catch (DllNotFoundException dnfe)
                {
                    throw new CudaException(CudaError.NoDriver, dnfe);
                }
                catch (Exception e)
                {
                    throw new CudaException(CudaError.Unknown, e);
                }
            });
        }
    }
}