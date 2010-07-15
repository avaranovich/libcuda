using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Libcuda.Api.DataTypes;
using XenoGears.Assertions;
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

        [DebuggerNonUserCode]
        private class nativeModuleLoadDataExOptions : Disposable
        {
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
                    (_allocated_threadsPerBlock != IntPtr.Zero).AssertTrue();

                    var indexOfThreadsPerBlock = Raw.IndexOf(t => t.Item1 == CUjit_option.ThreadsPerBlock);
                    (indexOfThreadsPerBlock != -1).AssertTrue();
                    return (int)((IntPtr*)_allocated_arrayOfOptionValues)[indexOfThreadsPerBlock];
                }
            }

            public unsafe ElapsedTime WallTime
            {
                get
                {
                    IsDisposed.AssertFalse();
                    (_allocated_arrayOfOptionValues != IntPtr.Zero).AssertTrue();

                    var indexOfWallTime = Raw.IndexOf(t => t.Item1 == CUjit_option.WallTime);
                    (indexOfWallTime != -1).AssertTrue();
                    var raw = (int)((IntPtr*)_allocated_arrayOfOptionValues)[indexOfWallTime];

                    var rawBytes = BitConverter.GetBytes(raw);
                    var wallTime = BitConverter.ToSingle(rawBytes, 0);
                    return new ElapsedTime(wallTime);
                }
            }

            public unsafe String InfoLog
            {
                get
                {
                    IsDisposed.AssertFalse();
                    (_allocated_infoLogBuffer != IntPtr.Zero).AssertTrue();
                    (_allocated_arrayOfOptionValues != IntPtr.Zero).AssertTrue();

                    var indexOfInfoLogBufferSizeBytes = Raw.IndexOf(t => t.Item1 == CUjit_option.InfoLogBufferSizeBytes);
                    (indexOfInfoLogBufferSizeBytes != -1).AssertTrue();
                    var byteCount = (int)((IntPtr*)_allocated_arrayOfOptionValues)[indexOfInfoLogBufferSizeBytes];

                    return Marshal.PtrToStringAnsi(_allocated_infoLogBuffer, byteCount);
                }
            }

            public unsafe String ErrorLog
            {
                get
                {
                    IsDisposed.AssertFalse();
                    (_allocated_errorLogBuffer != IntPtr.Zero).AssertTrue();
                    (_allocated_arrayOfOptionValues != IntPtr.Zero).AssertTrue();

                    var indexOfErrorLogBufferSizeBytes = Raw.IndexOf(t => t.Item1 == CUjit_option.ErrorLogBufferSizeBytes);
                    (indexOfErrorLogBufferSizeBytes != -1).AssertTrue();
                    var byteCount = (int)((IntPtr*)_allocated_arrayOfOptionValues)[indexOfErrorLogBufferSizeBytes];

                    return Marshal.PtrToStringAnsi(_allocated_errorLogBuffer, byteCount);
                }
            }

            public uint Count { get { return (uint)Raw.Count; } }
            public CUjit_option[] Keys { get { return Raw.Select(t => t.Item1).ToArray(); } }
            public IntPtr Values { get { EnsureRaw(); return _allocated_arrayOfOptionValues; } }

            private ReadOnlyCollection<Tuple<CUjit_option, IntPtr>> _raw;
            private ReadOnlyCollection<Tuple<CUjit_option, IntPtr>> Raw { get { EnsureRaw(); return _raw; } }
            private void EnsureRaw()
            {
                if (_raw == null)
                {
                    LowLevelAllocate();
                    _raw = GatherOptions().ToReadOnly();

                    var optionValues = _raw.Select(t => t.Item2).ToArray();
                    _allocated_arrayOfOptionValues = Marshal.AllocHGlobal(IntPtr.Size * optionValues.Count());
                    Marshal.Copy(optionValues, 0, _allocated_arrayOfOptionValues, optionValues.Count());
                }
            }

            private IEnumerable<Tuple<CUjit_option, IntPtr>> GatherOptions()
            {
                // can't pass 0 since it'll cause CUresult.ErrorInvalidValue
                if (MaxRegistersPerThread != 0) yield return Tuple.New(CUjit_option.MaxRegisters, _star_allocated_maxRegistersPerThread);
                yield return Tuple.New(CUjit_option.ThreadsPerBlock, _star_allocated_threadsPerBlock);
                yield return Tuple.New(CUjit_option.WallTime, _star_allocated_wallTime);
                yield return Tuple.New(CUjit_option.InfoLogBuffer, _allocated_infoLogBuffer);
                yield return Tuple.New(CUjit_option.InfoLogBufferSizeBytes, _star_allocated_infoLogBufferSizeBytes);
                yield return Tuple.New(CUjit_option.ErrorLogBuffer, _allocated_errorLogBuffer);
                yield return Tuple.New(CUjit_option.ErrorLogBufferSizeBytes, _star_allocated_errorLogBufferSizeBytes);
                yield return Tuple.New(CUjit_option.OptimizationLevel, _star_allocated_optimizationLevel);
                if (TargetFromContext) yield return Tuple.New(CUjit_option.TargetFromContext, _star_allocated_targetFromContext);
                if (!TargetFromContext) yield return Tuple.New(CUjit_option.Target, _star_allocated_target);
                yield return Tuple.New(CUjit_option.FallbackStrategy, _star_allocated_fallbackStrategy);
            }

            #region Very ugly allocation/deallocation code

            private const int maxCharsInLogBuffer = 1000;
            private const int maxCharsInErrorBuffer = 1000;

            private bool _is_allocated = false;

            private IntPtr _allocated_maxRegistersPerThread;
            private unsafe IntPtr _star_allocated_maxRegistersPerThread { get { return *((IntPtr*)(void*)_allocated_maxRegistersPerThread); } }
            private IntPtr _allocated_threadsPerBlock;
            private unsafe IntPtr _star_allocated_threadsPerBlock { get { return *((IntPtr*)(void*)_allocated_threadsPerBlock); } }
            private IntPtr _allocated_wallTime;
            private unsafe IntPtr _star_allocated_wallTime { get { return *((IntPtr*)(void*)_allocated_wallTime); } }
            private IntPtr _allocated_infoLogBuffer;
            private IntPtr _allocated_infoLogBufferSizeBytes;
            private unsafe IntPtr _star_allocated_infoLogBufferSizeBytes { get { return *((IntPtr*)(void*)_allocated_infoLogBufferSizeBytes); } }
            private IntPtr _allocated_errorLogBuffer;
            private IntPtr _allocated_errorLogBufferSizeBytes;
            private unsafe IntPtr _star_allocated_errorLogBufferSizeBytes { get { return *((IntPtr*)(void*)_allocated_errorLogBufferSizeBytes); } }
            private IntPtr _allocated_optimizationLevel;
            private unsafe IntPtr _star_allocated_optimizationLevel { get { return *((IntPtr*)(void*)_allocated_optimizationLevel); } }
            private IntPtr _allocated_targetFromContext;
            private unsafe IntPtr _star_allocated_targetFromContext { get { return _allocated_targetFromContext == IntPtr.Zero ? IntPtr.Zero : *((IntPtr*)(void*)_allocated_targetFromContext); } }
            private IntPtr _allocated_target;
            private unsafe IntPtr _star_allocated_target { get { return _allocated_target == IntPtr.Zero ? IntPtr.Zero : *((IntPtr*)(void*)_allocated_target); } }
            private IntPtr _allocated_fallbackStrategy;
            private unsafe IntPtr _star_allocated_fallbackStrategy { get { return *((IntPtr*)(void*)_allocated_fallbackStrategy); } }
            private IntPtr _allocated_arrayOfOptionValues;
            private unsafe IntPtr _star_allocated_arrayOfOptionValues { get { return *((IntPtr*)(void*)_allocated_arrayOfOptionValues); } }

            private void LowLevelAllocate()
            {
                if (!_is_allocated)
                {
                    // todo. there's a possibility of memory leak here
                    // if mallocs crash in the middle of action
                    // I'm cba to embrace the code in a number of try-finallies
                    // so I leave this bugg here

                    _allocated_maxRegistersPerThread = Marshal.AllocHGlobal(Marshal.SizeOf(MaxRegistersPerThread));
                    Marshal.WriteInt32(_allocated_maxRegistersPerThread, MaxRegistersPerThread);

                    _allocated_threadsPerBlock = Marshal.AllocHGlobal(Marshal.SizeOf(PlannedThreadsPerBlock));
                    Marshal.WriteInt32(_allocated_threadsPerBlock, PlannedThreadsPerBlock);

                    _allocated_wallTime = Marshal.AllocHGlobal(sizeof(float));

                    var sizeofInfoLogBuffer = maxCharsInLogBuffer * sizeof(byte);
                    _allocated_infoLogBuffer = Marshal.AllocHGlobal(sizeofInfoLogBuffer);
                    ZeroMemory(_allocated_infoLogBuffer, (uint)sizeofInfoLogBuffer);
                    Marshal.ThrowExceptionForHR(Marshal.GetLastWin32Error());
                    _allocated_infoLogBufferSizeBytes = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(IntPtr)));
                    Marshal.WriteIntPtr(_allocated_infoLogBufferSizeBytes, (IntPtr)sizeofInfoLogBuffer);

                    var sizeofErrorLogBuffer = maxCharsInErrorBuffer * sizeof(byte);
                    _allocated_errorLogBuffer = Marshal.AllocHGlobal(sizeofErrorLogBuffer);
                    ZeroMemory(_allocated_errorLogBuffer, (uint)sizeofErrorLogBuffer);
                    Marshal.ThrowExceptionForHR(Marshal.GetLastWin32Error());
                    _allocated_errorLogBufferSizeBytes = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(IntPtr)));
                    Marshal.WriteIntPtr(_allocated_errorLogBufferSizeBytes, (IntPtr)sizeofErrorLogBuffer);

                    _allocated_optimizationLevel = Marshal.AllocHGlobal(Marshal.SizeOf(OptimizationLevel));
                    Marshal.WriteInt32(_allocated_optimizationLevel, OptimizationLevel);

                    if (TargetFromContext)
                    {
                        _allocated_targetFromContext = Marshal.AllocHGlobal(sizeof(uint));
                        Marshal.WriteInt32(_allocated_targetFromContext, 1);
                    }
                    else
                    {
                        _allocated_target = Marshal.AllocHGlobal(sizeof(uint));
                        Marshal.WriteInt32(_allocated_target, (int)Target);
                    }

                    _allocated_fallbackStrategy = Marshal.AllocHGlobal(sizeof(uint));
                    Marshal.WriteInt32(_allocated_fallbackStrategy, (int)CUjit_fallbackstrategy.PreferPtx);

                    _is_allocated = true;
                }
            }

            [DllImport("kernel32.dll", SetLastError = true)]
            private static extern void ZeroMemory(IntPtr handle, uint length);

            protected override void DisposeUnmanagedResources()
            {
                // todo. there's a possibility of memory leak here
                // if frees crash in the middle of action
                // I'm cba to embrace the code in a number of try-finallies
                // so I leave this bugg here

                if (_allocated_maxRegistersPerThread != IntPtr.Zero)
                    Marshal.FreeHGlobal(_allocated_maxRegistersPerThread);

                if (_allocated_threadsPerBlock != IntPtr.Zero)
                    Marshal.FreeHGlobal(_allocated_threadsPerBlock);

                if (_allocated_wallTime != IntPtr.Zero)
                    Marshal.FreeHGlobal(_allocated_wallTime);

                if (_allocated_infoLogBuffer != IntPtr.Zero)
                    Marshal.FreeHGlobal(_allocated_infoLogBuffer);

                if (_allocated_infoLogBufferSizeBytes != IntPtr.Zero)
                    Marshal.FreeHGlobal(_allocated_infoLogBufferSizeBytes);

                if (_allocated_errorLogBuffer != IntPtr.Zero)
                    Marshal.FreeHGlobal(_allocated_errorLogBuffer);

                if (_allocated_errorLogBufferSizeBytes != IntPtr.Zero)
                    Marshal.FreeHGlobal(_allocated_errorLogBufferSizeBytes);

                if (_allocated_optimizationLevel != IntPtr.Zero)
                    Marshal.FreeHGlobal(_allocated_optimizationLevel);

                if (_allocated_targetFromContext != IntPtr.Zero)
                    Marshal.FreeHGlobal(_allocated_targetFromContext);

                if (_allocated_target != IntPtr.Zero)
                    Marshal.FreeHGlobal(_allocated_target);

                if (_allocated_fallbackStrategy != IntPtr.Zero)
                    Marshal.FreeHGlobal(_allocated_fallbackStrategy);

                if (_allocated_arrayOfOptionValues != IntPtr.Zero)
                    Marshal.FreeHGlobal(_allocated_arrayOfOptionValues);
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