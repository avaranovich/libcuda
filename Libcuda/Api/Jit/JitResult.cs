using System;
using System.Collections.ObjectModel;
using Libcuda.Api.DataTypes;
using Libcuda.Api.Native.DataTypes;
using Libcuda.Exceptions;
using Libcuda.Tracing;
using Libcuda.Versions;
using XenoGears.Functional;

namespace Libcuda.Api.Jit
{
    public class JitResult
    {
        public HardwareIsa CompilationTarget { get; set; }
        public int CompilationOptimizationLevel { get; set; }
        public int CompilationMaxRegistersPerThread { get; set; }
        public int CompilationPlannedThreadsPerBlock { get; set; }
        public ElapsedTime CompilationWallTime { get; private set; }
        public String CompilationInfoLog { get; private set; }
        public String CompilationErrorLog { get; private set; }

        public String Ptx { get; private set; }
        public CudaError ErrorCode { get; private set; }
        public JittedModule Module { get; private set; }
        public ReadOnlyCollection<JittedFunction> Functions { get { return Module == null ? null : Module.Functions; } }

        internal JitResult(JitCompiler compiler, String ptx, CUjit_result result)
        {
            var log = Traces.Jit.Info;
            log.EnsureBlankLine();

            CompilationTarget = compiler.Target;
            CompilationOptimizationLevel = compiler.OptimizationLevel;
            CompilationMaxRegistersPerThread = compiler.MaxRegistersPerThread;
            CompilationPlannedThreadsPerBlock = compiler.PlannedThreadsPerBlock;
            CompilationWallTime = result.WallTime;
            CompilationInfoLog = result.InfoLog == String.Empty ? null : result.InfoLog;
            if (CompilationInfoLog != null) log.WriteLine(CompilationInfoLog);
            CompilationErrorLog = result.ErrorLog == String.Empty ? null : result.ErrorLog;

            Ptx = ptx;
            ErrorCode = (CudaError)result.ErrorCode;

            if (ErrorCode == CudaError.Success)
            {
                Module = new JittedModule(this, result.Module);
                log.WriteLine("JIT compilation succeeded in {0} and produced {1}." , CompilationWallTime, Module);

                log.EnsureBlankLine();
                log.WriteLine("Loading entry points of {0}...", Module);
                Module.Functions.ForEach(function =>
                {
                    log.WriteLine("Found entry point {0}...", function.Name);
                    log.WriteLine("Successfully resolved it as {0}.", function.Handle);
                    log.WriteLine("    Max threads per block        : {0}", function.MaxThreadsPerBlock);
                    log.WriteLine("    Shared memory requirements   : {0} bytes", function.SharedSizeBytes);
                    log.WriteLine("    Constant memory requirements : {0} bytes", function.ConstSizeBytes);
                    log.WriteLine("    Local memory requirements    : {0} bytes", function.LocalSizeBytes);
                    log.WriteLine("    Register memory requirements : {0} registers", function.NumRegs);
                    log.WriteLine("    PTX version                  : {0}", function.PtxVersion);
                    log.WriteLine("    Binary version               : {0}", function.BinaryVersion);
                });
            }
            else
            {
                log.WriteLine("JIT compilation failed in {0}." + Environment.NewLine, CompilationWallTime);
                throw new JitException(this);
            }
        }
    }
}