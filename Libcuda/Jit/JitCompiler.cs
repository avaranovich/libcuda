using System;
using System.Diagnostics;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices;
using Libcuda.Native;
using Libcuda.Native.DataTypes;
using Libcuda.Native.Exceptions;
using Libcuda.Versions;
using XenoGears.Functional;
using XenoGears.Assertions;
using XenoGears.Logging;
using XenoGears.Strings;
using XenoGears.Traits.Disposable;

namespace Libcuda.Jit
{
    [DebuggerNonUserCode]
    public class JitCompiler
    {
        public HardwareIsa Target { get; set; }
        private int _optimizationLevel = 4; // 0..4, higher is better
        public int OptimizationLevel { get { return _optimizationLevel; } set { _optimizationLevel = value; } }
        public int MaxRegistersPerThread { get; set; }
        public int PlannedThreadsPerBlock { get; set; }

        public JitResult Compile(String ptx)
        {
            var image = Marshal.StringToHGlobalAnsi(ptx);

            try
            {
                using (var options = new cuModuleLoadDataExOptions(
                    Target, OptimizationLevel, MaxRegistersPerThread, PlannedThreadsPerBlock))
                {
#if TRACE
                    typeof(nvcuda).TypeInitializer.Invoke(null, null);
                    Log.TraceLine("Peforming JIT compilation...");
                    Log.TraceLine("    PTX source text                              : {0}", "(see below)");
                    Log.TraceLine("    Target ISA                                   : {0}", Target);
                    Log.TraceLine("    Max registers per thread                     : {0}", MaxRegistersPerThread);
                    Log.TraceLine("    Planned threads per block                    : {0}", PlannedThreadsPerBlock);
                    Log.TraceLine("    Optimization level (0 - 4, higher is better) : {0}", OptimizationLevel);
                    Log.TraceLine(Environment.NewLine + "*".Repeat(120));
                    Log.TraceLine(ptx.QuoteBraces().TrimEnd());
                    Log.TraceLine(120.Times("*"));
                    Log.TraceLine();
#endif

                    CUmodule module;
                    var error = nvcuda.cuModuleLoadDataEx(out module, image, options.Count, options.Keys, options.Values);
                    var wallTime = options.WallTime;
                    var infoLog = options.InfoLog.TrimEnd('\0');
                    var errorLog = options.ErrorLog.TrimEnd('\0');

#if TRACE
                    Log.TraceLine(infoLog);
                    Log.TraceLine();
#endif

                    var result = error == CUresult.Success ?
                        new JitResult(this, ptx, wallTime, infoLog, errorLog, module) :
                        new JitResult(this, ptx, wallTime, infoLog, errorLog, error);

                    if (error == CUresult.Success)
                    {
                        return result;
                    }
                    else
                    {
                        throw new JitException(result);
                    }
                }
            }
            finally 
            {
                Marshal.FreeHGlobal(image);
            }
        }

        [DebuggerNonUserCode]
        private class cuModuleLoadDataExOptions : Disposable
        {
            // input parameters

            private readonly HardwareIsa _target;
            private readonly int _optimizationLevel;
            private readonly int _maxRegistersPerThread;
            private readonly int _plannedThreadsPerBlock;
            private const int maxCharsInLogBuffer = 1000;
            private const int maxCharsInErrorBuffer = 1000;

            // output parameters

            public unsafe int ThreadsPerBlock
            {
                get
                {
                    IsDisposed.AssertFalse();
                    (_allocated_threadsPerBlock != IntPtr.Zero).AssertTrue();

                    var indexOfThreadsPerBlock = Tuples.IndexOf(t => t.Item1 == CUjit_option.ThreadsPerBlock);
                    (indexOfThreadsPerBlock != -1).AssertTrue();
                    return (int)((IntPtr*)_allocated_arrayOfOptionValues)[indexOfThreadsPerBlock];
                }
            }

            public unsafe TimeSpan WallTime
            {
                get
                {
                    IsDisposed.AssertFalse();
                    (_allocated_arrayOfOptionValues != IntPtr.Zero).AssertTrue();

                    var indexOfWallTime = Tuples.IndexOf(t => t.Item1 == CUjit_option.WallTime);
                    (indexOfWallTime != -1).AssertTrue();
                    var raw = (int)((IntPtr*)_allocated_arrayOfOptionValues)[indexOfWallTime];

                    var rawBytes = BitConverter.GetBytes(raw);
                    var wallTime = BitConverter.ToSingle(rawBytes, 0);
                    return TimeSpan.FromMilliseconds(wallTime);
                }
            }

            public unsafe String InfoLog
            {
                get
                {
                    IsDisposed.AssertFalse();
                    (_allocated_infoLogBuffer != IntPtr.Zero).AssertTrue();
                    (_allocated_arrayOfOptionValues != IntPtr.Zero).AssertTrue();

                    var indexOfInfoLogBufferSizeBytes = Tuples.IndexOf(t => t.Item1 == CUjit_option.InfoLogBufferSizeBytes);
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

                    var indexOfErrorLogBufferSizeBytes = Tuples.IndexOf(t => t.Item1 == CUjit_option.ErrorLogBufferSizeBytes);
                    (indexOfErrorLogBufferSizeBytes != -1).AssertTrue();
                    var byteCount = (int)((IntPtr*)_allocated_arrayOfOptionValues)[indexOfErrorLogBufferSizeBytes];

                    return Marshal.PtrToStringAnsi(_allocated_errorLogBuffer, byteCount);
                }
            }

            public cuModuleLoadDataExOptions(HardwareIsa target, int optimizationLevel, int maxRegistersPerThread, int plannedThreadsPerBlock)
            {
                _target = target;
                _optimizationLevel = optimizationLevel;
                _maxRegistersPerThread = maxRegistersPerThread;
                _plannedThreadsPerBlock = plannedThreadsPerBlock;
            }

            public uint Count { get { return (uint)Tuples.Count; } }
            public CUjit_option[] Keys { get { return Tuples.Select(t => t.Item1).ToArray(); } }
            public IntPtr Values { get { return _allocated_arrayOfOptionValues; } }

            private ReadOnlyCollection<Tuple<CUjit_option, IntPtr>> _cache;
            private ReadOnlyCollection<Tuple<CUjit_option, IntPtr>> Tuples
            {
                get
                {
                    if (_cache == null)
                    {
                        LowLevelAllocate();
                        _cache = GatherOptions().ToReadOnly();

                        var optionValues = _cache.Select(t => t.Item2).ToArray();
                        _allocated_arrayOfOptionValues = Marshal.AllocHGlobal(IntPtr.Size * optionValues.Count());
                        Marshal.Copy(optionValues, 0, _allocated_arrayOfOptionValues, optionValues.Count());
                    }

                    return _cache;
                }
            }

            private IEnumerable<Tuple<CUjit_option, IntPtr>> GatherOptions()
            {
                // can't pass 0 since it'll cause CUresult.ErrorInvalidValue
                if (_maxRegistersPerThread != 0) yield return Tuple.New(CUjit_option.MaxRegisters, _star_allocated_maxRegistersPerThread);
                yield return Tuple.New(CUjit_option.ThreadsPerBlock, _star_allocated_threadsPerBlock);
                yield return Tuple.New(CUjit_option.WallTime, _star_allocated_wallTime);
                yield return Tuple.New(CUjit_option.InfoLogBuffer, _allocated_infoLogBuffer);
                yield return Tuple.New(CUjit_option.InfoLogBufferSizeBytes, _star_allocated_infoLogBufferSizeBytes);
                yield return Tuple.New(CUjit_option.ErrorLogBuffer, _allocated_errorLogBuffer);
                yield return Tuple.New(CUjit_option.ErrorLogBufferSizeBytes, _star_allocated_errorLogBufferSizeBytes);
                yield return Tuple.New(CUjit_option.OptimizationLevel, _star_allocated_optimizationLevel);
                yield return Tuple.New(CUjit_option.Target, _star_allocated_target);
                yield return Tuple.New(CUjit_option.FallbackStrategy, _star_allocated_fallbackStrategy);
            }

            #region Very ugly allocation/deallocation code

            private bool _allocated = false;
            private Object _allocationLock = new Object();

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
            private IntPtr _allocated_target;
            private unsafe IntPtr _star_allocated_target { get { return _allocated_target == IntPtr.Zero ? IntPtr.Zero : *((IntPtr*)(void*)_allocated_target); } }
            private IntPtr _allocated_fallbackStrategy;
            private unsafe IntPtr _star_allocated_fallbackStrategy { get { return *((IntPtr*)(void*)_allocated_fallbackStrategy); } }
            private IntPtr _allocated_arrayOfOptionValues;
            private unsafe IntPtr _star_allocated_arrayOfOptionValues { get { return *((IntPtr*)(void*)_allocated_arrayOfOptionValues); } }

            private void LowLevelAllocate()
            {
                lock (_allocationLock)
                {
                    if (!_allocated)
                    {
                        // todo. there's a possibility of memory leak here
                        // if mallocs crash in the middle of action
                        // I'm cba to embrace the code in a number of try-finallies
                        // so I leave this bugg here

                        _allocated_maxRegistersPerThread = Marshal.AllocHGlobal(Marshal.SizeOf(_maxRegistersPerThread));
                        Marshal.WriteInt32(_allocated_maxRegistersPerThread, _maxRegistersPerThread);

                        _allocated_threadsPerBlock = Marshal.AllocHGlobal(Marshal.SizeOf(_plannedThreadsPerBlock));
                        Marshal.WriteInt32(_allocated_threadsPerBlock, _plannedThreadsPerBlock);

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

                        _allocated_optimizationLevel = Marshal.AllocHGlobal(Marshal.SizeOf(_optimizationLevel));
                        Marshal.WriteInt32(_allocated_optimizationLevel, _optimizationLevel);

                        _allocated_target = Marshal.AllocHGlobal(sizeof(uint));
                        Marshal.WriteInt32(_allocated_target, (int)_target.ToCUjit_target());

                        _allocated_fallbackStrategy = Marshal.AllocHGlobal(sizeof(uint));
                        Marshal.WriteInt32(_allocated_fallbackStrategy, (int)CUjit_fallbackstrategy.PreferPtx);

                        _allocated = true;
                    }
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

                if (_allocated_target != IntPtr.Zero) 
                    Marshal.FreeHGlobal(_allocated_target);

                if (_allocated_arrayOfOptionValues != IntPtr.Zero) 
                    Marshal.FreeHGlobal(_allocated_arrayOfOptionValues);
            }

            #endregion
        }
    }
}