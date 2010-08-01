using System;
using System.Linq;
using System.Collections.Generic;
using Libcuda.DataTypes;
using Libcuda.Api.Jit;
using Libcuda.Api.Native;
using Libcuda.Api.Native.DataTypes;
using Libcuda.Tracing;
using XenoGears.Assertions;
using XenoGears.Functional;
using XenoGears.Logging;
using XenoGears.Reflection.Simple;
using XenoGears.Strings;

namespace Libcuda.Api.Run
{
    public class KernelInvocation
    {
        private LevelLogger Log { get { return Traces.Run.Info; } }

        public JittedFunction Function { get; private set; }
        public KernelArguments Args { get; private set; }
        private bool _hasCompletedExecution = false;

        public KernelInvocation(JittedFunction function, params KernelArgument[] args)
            : this(function, (IEnumerable<KernelArgument>)args)
        {
        }

        public KernelInvocation(JittedFunction function, IEnumerable<KernelArgument> args)
        {
            CudaDriver.Ensure();
            Function = function.AssertNotNull();
            Args = new KernelArguments(args);
            GC.SuppressFinalize(Args);
        }

        public KernelResult Launch(dim3 gridDim, dim3 blockDim)
        {
            CudaDriver.Ensure();
            Args.AssertNone(p => p.IsDisposed);
            _hasCompletedExecution.AssertFalse();

            var offsets = Args.Scanbe(0, (offset, arg, _) => offset + arg.SizeInArgList);
            Args.Zip(offsets, (arg, offset) => arg.PassInto(this, offset));

            try
            {
                nvcuda.cuFuncSetBlockShape(Function, blockDim);
                nvcuda.cuFuncSetSharedSize(Function, (uint)Function.SharedSizeBytes);
                nvcuda.cuFuncSetCacheConfig(Function, CUfunc_cache.PreferNone);
                nvcuda.cuParamSetSize(Function, (uint)Args.Select(p => p.SizeInArgList).Sum());

                TraceBeforeLaunch(gridDim, blockDim);
                var wall_time = CudaProfiler.Benchmark(() => nvcuda.cuLaunchGrid(Function, gridDim));
                Log.WriteLine("Function execution succeeded in {0} ({1} = 0.5 {2}s).", wall_time, Syms.Epsilon, Syms.Mu);
                return new KernelResult(this, wall_time);
            }
            finally 
            {
                _hasCompletedExecution = true;
            }
        }

        private void TraceBeforeLaunch(dim3 gridDim, dim3 blockDim)
        {
            Log.EnsureBlankLine();
            Log.WriteLine("Launching function {0}...", Function);
            Log.WriteLine("Grid is configured as {2}: blockdim is {0}, griddim is {1}",
                blockDim.ToString().Slice(4),
                gridDim.ToString().Slice(4),
                new dim3(blockDim.X * gridDim.X, blockDim.Y * gridDim.Y, blockDim.Z * gridDim.Z).ToString().Slice(4));

            var log = new List<ITuple>();

            var offset = 0;
            var sizeInVRAM = 0;
            foreach (var args in Args)
            {
                var value = args.Get("_value");
                log.Add(Tuple.New("+" + offset.ToString("0000"), args.Direction, value, args.SizeInVRAM, args.SizeInArgList));
                offset += args.SizeInArgList;
                sizeInVRAM += args.SizeInVRAM;
            }
            log.Add(Tuple.New(offset.ToString("0000"), "", "", sizeInVRAM, offset));

            var formatted = log.Select(t => ((ITuple)t).Items.Select(v => v.ToString()).ToArray()).ToArray();
            Func<int, String, int, String> pad = (i, text, max) => i < 2 ? text.PadRight(max) : i == 2 ? text.PadRight((int)(max + 1)) : text;
            var padded = formatted.Select(entry => entry.Select((part, i) => pad(i, part, formatted.Max(entry1 => entry1[i].Length)))).ToArray();

            var maxLength = 0;
            padded.ForEach((entry, i) =>
            {
                if (i == log.Count() - 1 && maxLength != 0)
                {
                    Log.WriteLine("    " + "*".Repeat(maxLength - 4));
                }

                var line = String.Format("    {0} {1} {2} ({3} bytes in VRAM)", entry.SkipLast(1).ToArray());
                Log.WriteLine(line);
                maxLength = Math.Max(line.Length, maxLength);
            });
        }
    }
}
