using System;
using System.Diagnostics;
using System.Linq;
using System.Collections.Generic;
using Libcuda.DataTypes;
using Libcuda.Jit;
using Libcuda.Native;
using Libcuda.Native.DataTypes;
using Libcuda.Native.Exceptions;
using XenoGears.Assertions;
using XenoGears.Logging;
using XenoGears.Functional;
using XenoGears.Reflection.Simple;
using XenoGears.Strings;
using XenoGears.Traits.Disposable;

namespace Libcuda.Run
{
    [DebuggerNonUserCode]
    public class KernelInvocation : Disposable
    {
        public JittedFunction Function { get; private set; }
        public KernelArguments Args { get; set; }

        private readonly CUevent _beforeLaunch;
        private readonly CUevent _afterLaunch;
        private readonly Object _launchLock = new Object();
        internal bool HasCompletedExecution = false;

        public KernelInvocation(JittedFunction function, IEnumerable<KernelArgument> args)
        {
            Function = function;
            Args = new KernelArguments(args);

            try
            {
                var error2 = nvcuda.cuEventCreate(out _beforeLaunch, CUevent_flags.Default);
                if (error2 != CUresult.Success) throw new CudaException(error2);

                var error3 = nvcuda.cuEventCreate(out _afterLaunch, CUevent_flags.Default);
                if (error3 != CUresult.Success) throw new CudaException(error3);
            }
            catch (CudaException)
            {
                Dispose();
                throw;
            }
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
#if TRACE
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
#endif

                    var function = Function.Handle;
                    var error1 = nvcuda.cuFuncSetBlockShape(function, (int)blockDim.X, (int)blockDim.Y, (int)blockDim.Z);
                    if (error1 != CUresult.Success) throw new CudaException(error1);

//                    var error2 = nvcuda.cuFuncSetSharedSize(function, /* todo */);
//                    if (error2 != CUresult.Success) throw new CudaException(error2);

                    var error3 = nvcuda.cuFuncSetCacheConfig(function, CUfunc_cache.PreferNone);
                    if (error3 != CUresult.Success) throw new CudaException(error3);

                    var argListSize = Args.Select(p => p.SizeInArgList).Sum();
                    var error4 = nvcuda.cuParamSetSize(function, (uint)argListSize);
                    if (error4 != CUresult.Success) throw new CudaException(error4);

                    var error5 = nvcuda.cuEventRecord(_beforeLaunch, CUstream.Null);
                    if (error5 != CUresult.Success) throw new CudaException(error5);

                    // note. wow, here we ain't able to specify the Z dimension
                    (gridDim.Z == 1).AssertTrue();
                    var error6 = nvcuda.cuLaunchGrid(function, (int)gridDim.X, (int)gridDim.Y);
                    if (error6 != CUresult.Success) throw new CudaException(error6);

                    var error7 = nvcuda.cuEventRecord(_afterLaunch, CUstream.Null);
                    if (error7 != CUresult.Success) throw new CudaException(error7);

                    var error8 = nvcuda.cuEventSynchronize(_afterLaunch);
                    if (error8 != CUresult.Success) throw new CudaException(error8);

                    float wallTime;
                    var error9 = nvcuda.cuEventElapsedTime(out wallTime, _beforeLaunch, _afterLaunch);
                    if (error9 != CUresult.Success) throw new CudaException(error9);

#if TRACE
                    Log.TraceLine("Function execution succeeded in {0}.", CUDAElapsedTimeToString(wallTime));
                    Log.TraceLine();
#endif

                    // note. cannot use TimeSpan here because it ain't work with fractions of milliseconds
                    return new KernelResult(this, wallTime);
                }
                catch (CudaException)
                {
                    Dispose();
                    throw;
                }
                finally 
                {
                    HasCompletedExecution = true;
                }
            }
        }

        private static String CUDAElapsedTimeToString(float msecs)
        {
            var totalDays = (int)Math.Floor(msecs / (1000 * 60 * 60 * 24));
            var totalHours = (int)Math.Floor(msecs / (1000 * 60 * 60));
            var totalMinutes = (int)Math.Floor(msecs / (1000 * 60));
            var totalSeconds = (int)Math.Floor(msecs / 1000);

            var days = totalDays;
            var hours = totalHours - 24 * days;
            var minutes = totalMinutes - 60 * totalMinutes;
            var seconds = totalSeconds - 60 * totalMinutes;
            var theRest = (msecs / 1000) - totalSeconds;

            var fmt = String.Format("{0:00}:{1:00}:{2:00}.{3:000000} (ε = 0.5 μs)",
                hours, minutes, seconds, (int)(theRest * 1e6));
            if (days != 0) fmt = days.ToString("00") + fmt;

            return fmt;
        }

        protected override void DisposeUnmanagedResources()
        {
            lock (_launchLock)
            {
                if (_beforeLaunch.IsNotNull)
                {
                    var error = nvcuda.cuEventDestroy(_beforeLaunch);
                    if (error != CUresult.Success) throw new CudaException(error);
                }

                if (_afterLaunch.IsNotNull)
                {
                    var error = nvcuda.cuEventDestroy(_afterLaunch);
                    if (error != CUresult.Success) throw new CudaException(error);
                }
            }
        }
    }
}
