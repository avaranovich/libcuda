﻿using System;
using System.Diagnostics;
using System.Linq;
using System.Collections.Generic;
using Libcuda.DataTypes;
using Libcuda.Api.Jit;
using Libcuda.Api.Native;
using Libcuda.Api.Native.DataTypes;
using XenoGears.Assertions;
using XenoGears.Logging;
using XenoGears.Functional;
using XenoGears.Reflection.Simple;
using XenoGears.Strings;
using XenoGears.Traits.Disposable;

namespace Libcuda.Api.Run
{
    [DebuggerNonUserCode]
    public class KernelInvocation : Disposable
    {
        public JittedFunction Function { get; private set; }
        public KernelArguments Args { get; set; }

        private readonly Object _launchLock = new Object();
        internal bool HasCompletedExecution = false;

        public KernelInvocation(JittedFunction function, IEnumerable<KernelArgument> args)
        {
            Function = function;
            Args = new KernelArguments(args);
        }

        public KernelResult Launch(dim3 gridDim, dim3 blockDim)
        {
            lock (_launchLock)
            {
                IsDisposed.AssertFalse();
                Args.AssertNone(p => p.IsDisposed);
                HasCompletedExecution.AssertFalse();

                var offsets = Args.Scanbe(0, (offset, arg, _) => offset + arg.SizeInArgList);
                Args.Zip(offsets, (arg, offset) => arg.Fill(this, offset));

                try
                {
                    nvcuda.cuFuncSetBlockShape(Function, blockDim);
                    nvcuda.cuFuncSetSharedSize(Function, (uint)Function.SharedSizeBytes);
                    nvcuda.cuFuncSetCacheConfig(Function, CUfunc_cache.PreferNone);
                    nvcuda.cuParamSetSize(Function, (uint)Args.Select(p => p.SizeInArgList).Sum());

                    TraceBeforeLaunch(gridDim, blockDim);
                    var wall_time = CudaProfiler.Benchmark(() => nvcuda.cuLaunchGrid(Function, gridDim));
                    Log.TraceLine("Function execution succeeded in {0}." + Environment.NewLine, wall_time);
                    return new KernelResult(this, wall_time);
                }
                finally 
                {
                    HasCompletedExecution = true;
                }
            }
        }

        private void TraceBeforeLaunch(dim3 gridDim, dim3 blockDim)
        {
            Log.TraceLine("Launching function {0} ({1})...", Function.Name, Function.Handle);
            Log.TraceLine("Grid is configured as {2}: blockdim is {0}, griddim is {1}",
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
                    Log.TraceLine("    " + "*".Repeat(maxLength - 4));
                }

                var line = String.Format("    {0} {1} {2} ({3} bytes in VRAM)", entry.SkipLast(1).ToArray());
                Log.TraceLine(line);
                maxLength = Math.Max(line.Length, maxLength);
            });
        }
    }
}
