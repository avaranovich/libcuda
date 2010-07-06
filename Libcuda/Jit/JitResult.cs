using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using Libcuda.Native.DataTypes;
using Libcuda.Native.Exceptions;
using Libcuda.Versions;
using XenoGears.Logging;
using XenoGears.Functional;

namespace Libcuda.Jit
{
    [DebuggerNonUserCode]
    public class JitResult
    {
        public HardwareIsa CompilationTarget { get; set; }
        public int CompilationOptimizationLevel { get; set; }
        public int CompilationMaxRegistersPerThread { get; set; }
        public int CompilationPlannedThreadsPerBlock { get; set; }
        public TimeSpan CompilationWallTime { get; private set; }
        public String CompilationInfoLog { get; private set; }
        public String CompilationErrorLog { get; private set; }

        public String Ptx { get; private set; }
        public CUresult ErrorCode { get; private set; }
        public JittedModule Module { get; private set; }
        public ReadOnlyCollection<JittedFunction> Functions { get { return Module == null ? null : Module.Functions; } }

        internal JitResult(JitCompiler compiler, String ptx, TimeSpan wallTime, String infoLog, String errorLog, CUmodule module)
        {
            CompilationTarget = compiler.Target;
            CompilationOptimizationLevel = compiler.OptimizationLevel;
            CompilationMaxRegistersPerThread = compiler.MaxRegistersPerThread;
            CompilationPlannedThreadsPerBlock = compiler.PlannedThreadsPerBlock;
            CompilationWallTime = wallTime;
            CompilationInfoLog = infoLog;
            CompilationErrorLog = errorLog;

            Ptx = ptx;
            ErrorCode = CUresult.Success;

#if TRACE
            Log.TraceLine("JIT compilation succeeded in {0} and produced 0x{1}.", CompilationWallTime, module);
            Log.TraceLine();
#endif

#if TRACE
            Log.TraceLine("Loading entry points of {0}...", module);
            Module = new JittedModule(this, module);
            Module.Functions.ForEach(function =>
            {
                Log.TraceLine("Found entry point {0}...", function.Name);
                Log.TraceLine("Successfully resolved it as {0}.", function.Handle);
                Log.TraceLine("    Max threads per block        : {0}", function.MaxThreadsPerBlock);
                Log.TraceLine("    Shared memory requirements   : {0} bytes", function.SharedSizeBytes);
                Log.TraceLine("    Constant memory requirements : {0} bytes", function.ConstSizeBytes);
                Log.TraceLine("    Local memory requirements    : {0} bytes", function.LocalSizeBytes);
                Log.TraceLine("    Register memory requirements : {0} registers", function.NumRegs);
                Log.TraceLine("    PTX version                  : {0}.{1}", function.PtxVersion / 10, function.PtxVersion % 10);
                Log.TraceLine("    Binary version               : {0}.{1}", function.BinaryVersion / 10, function.BinaryVersion % 10);
            });
            Log.TraceLine();
#else
            Module = new JittedModule(this, Module);
#endif
        }

        public JitResult(JitCompiler compiler, String ptx, TimeSpan wallTime, String infoLog, String errorLog, CUresult errorCode)
        {
            CompilationTarget = compiler.Target;
            CompilationOptimizationLevel = compiler.OptimizationLevel;
            CompilationMaxRegistersPerThread = compiler.MaxRegistersPerThread;
            CompilationPlannedThreadsPerBlock = compiler.PlannedThreadsPerBlock;
            CompilationWallTime = wallTime;
            CompilationInfoLog = infoLog;
            CompilationErrorLog = errorLog;

            Ptx = ptx;
            ErrorCode = errorCode;
        }
    }
}