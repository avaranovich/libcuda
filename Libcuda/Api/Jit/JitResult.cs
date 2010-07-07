using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using Libcuda.Api.Native.DataTypes;
using Libcuda.Exceptions;
using Libcuda.Versions;
using XenoGears.Logging;
using XenoGears.Functional;

namespace Libcuda.Api.Jit
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
        public CudaError ErrorCode { get; private set; }
        public JittedModule Module { get; private set; }
        public ReadOnlyCollection<JittedFunction> Functions { get { return Module == null ? null : Module.Functions; } }

        internal JitResult(JitCompiler compiler, String ptx, CUjit_result result)
        {
            CompilationTarget = compiler.Target;
            CompilationOptimizationLevel = compiler.OptimizationLevel;
            CompilationMaxRegistersPerThread = compiler.MaxRegistersPerThread;
            CompilationPlannedThreadsPerBlock = compiler.PlannedThreadsPerBlock;
            CompilationWallTime = result.WallTime;
            CompilationInfoLog = result.InfoLog;
            CompilationErrorLog = result.ErrorLog;

            Ptx = ptx;
            ErrorCode = (CudaError)result.ErrorCode;
            Module = ErrorCode == CudaError.Success ? new JittedModule(this, result.Module) : null;

            if (Module != null)
            {
                Log.TraceLine("JIT compilation succeeded in {0} and produced 0x{1}.", CompilationWallTime, Module);
                Log.TraceLine();

                Log.TraceLine("Loading entry points of {0}...", Module);
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
            }
            else
            {
                Log.TraceLine("JIT compilation failed in {0}.", CompilationWallTime);
                Log.TraceLine();
            }
        }
    }
}