using System;
using System.Linq;
using System.Text.RegularExpressions;
using Libcuda.Api.Native;
using Libcuda.Api.Native.DataTypes;
using Libcuda.DataTypes;
using Libcuda.Tracing;
using Libcuda.Versions;
using XenoGears.Functional;
using XenoGears.Assertions;
using XenoGears.Strings;
using XenoGears.Traits.Dumpable;

namespace Libcuda.Api.Jit
{
    public class JitCompiler
    {
        public bool TargetFromContext { get; set; }
        public HardwareIsa Target { get; set; }
        private int _optimizationLevel = 4; // 0..4, higher is better
        public int OptimizationLevel { get { return _optimizationLevel; } set { _optimizationLevel = value; } }

        private JitTuning _tuning = new JitTuning();
        public JitTuning Tuning { get { return _tuning; } set { _tuning = value ?? new JitTuning(); } }
        public int Maxnreg { get { return Tuning.Maxnreg; } set { Tuning.Maxnreg = value; } }
        public dim3 Maxntid { get { return Tuning.Maxntid; } set { Tuning.Maxntid = value; } }
        public dim3 Reqntid { get { return Tuning.Reqntid; } set { Tuning.Reqntid = value; } }
        public int Minnctapersm { get { return Tuning.Minnctapersm; } set { Tuning.Minnctapersm = value; } }
        public int Maxnctapersm { get { return Tuning.Maxnctapersm; } set { Tuning.Maxnctapersm = value; } }

        // todo. cache jitted kernels
        // this is of little priority though, since driver caches kernels as well
        public JitResult Compile(String ptx)
        {
            var log = Traces.Jit.Info;
            log.EnsureBlankLine();

            log.WriteLine("Peforming JIT compilation...");
            log.WriteLine("    PTX source text                              : {0}", "(see below)");
            log.WriteLine("    Target hardware ISA                          : {0}", TargetFromContext ? "(determined from context)" : Target.ToString());
            log.WriteLine("    Actual hardware ISA                          : {0}", CudaVersions.HardwareIsa);
            log.WriteLine("    Optimization level (0 - 4, higher is better) : {0}", OptimizationLevel);

            // here we attempt to rewrite PTX by injecting performance tuning directives directly into source codes
            if (Tuning.IsNotTrivial)
            {
                Tuning.Validate();

                log.EnsureBlankLine();
                log.WriteLine("Detected non-trivial performance tuning parameters...");
                Tuning.DumpAsText(log.Writer.Medium);

                log.EnsureBlankLine();
                log.WriteLine("To apply them it is necessary to perform PTX rewriting and inject corresponding directives directly into source codes.");
                log.WriteLine("Analyzing entries in PTX module...");
                var rx_entry = @"(?<header>\.entry\s+(?<name>([a-zA-Z][a-zA-Z0-9_$]*)|([_$%][a-zA-Z0-9_$]*))\s*(?<params>\(.*?\))?)\s*(?<directives>\..*?)?\s*\{";
                ptx = ptx.Replace(rx_entry, RegexOptions.Singleline, m =>
                {
                    var name = m["name"];
                    var s_directives = m["directives"].Split(".".MkArray(), StringSplitOptions.None).Trim().Where(s => s.IsNotEmpty()).ToReadOnly();
                    var directives = s_directives.Select(s => s.Parse(@"$(?<name>\w+)\s+(?<value>.*?)^")).ToDictionary(m1 => m1["name"].Trim(), m1 => m1["value"].Trim()).ToReadOnly();
                    if (directives.IsNotEmpty())
                    {
                        Func<String, dim3> parse_dim3 = s =>
                        {
                            var m1 = s.AssertParse(@"^(?<x>\d+)?(\s*,\s*(?<y>\d+))?(\s*,\s*(?<z>\d+))?$").ToDictionary();
                            m1 = m1.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.IsNullOrEmpty() ? null : kvp.Value);
                            return new dim3(int.Parse(m1["x"]), int.Parse(m1["y"] ?? "1"), int.Parse(m1["z"] ?? "1"));
                        };

                        log.WriteLine("Found entry \"{0}\" tuned as follows: {1}.", name, directives.Select(kvp => String.Format("{0} = {1}", kvp.Key, kvp.Value)).StringJoin(", "));

                        var maxnreg = int.Parse(directives.GetOrDefault("maxnreg", "0"));
                        if (Maxnreg != 0)
                        {
                            if (maxnreg != 0 && !(Maxnreg <= maxnreg))
                            {
                                log.WriteLine("Conflict! New max registers per thread ({0}) is incompatible with original value ({1}).", Maxnreg, maxnreg);
                                throw AssertionHelper.Fail();
                            }
                            else
                            {
                                maxnreg = Maxnreg;
                            }
                        }

                        var maxntid = parse_dim3(directives.GetOrDefault("maxntid", "0, 0, 0"));
                        if (Maxntid != new dim3())
                        {
                            if (maxntid != new dim3() && !(Maxntid <= maxntid))
                            {
                                log.WriteLine("Conflict! New max threads in thread block ({0}, {1}, {2}) is incompatible with original value ({3}, {4}, {5}).", Maxntid.X, Maxntid.Y, Maxntid.Z, maxntid.X, maxntid.Y, maxntid.Z);
                                throw AssertionHelper.Fail();
                            }
                            else
                            {
                                maxntid = Maxntid;
                            }
                        }

                        var reqntid = parse_dim3(directives.GetOrDefault("reqntid", "0, 0, 0"));
                        if (Reqntid != new dim3())
                        {
                            if (reqntid != new dim3() && Reqntid != reqntid)
                            {
                                log.WriteLine("Conflict! New required threads in thread block ({0}, {1}, {2}) is incompatible with original value ({3}, {4}, {5}).", Reqntid.X, Reqntid.Y, Reqntid.Z, reqntid.X, reqntid.Y, reqntid.Z);
                                throw AssertionHelper.Fail();
                            }
                            else
                            {
                                reqntid = Reqntid;
                            }
                        }

                        if (maxntid != new dim3() && reqntid != new dim3())
                        {
                            if (!(reqntid <= maxntid))
                            {
                                log.WriteLine("Conflict! Required threads in thread block ({0}, {1}, {2}) is incompatible with max threads in thread block ({3}, {4}, {5}).", reqntid.X, reqntid.Y, reqntid.Z, maxntid.X, maxntid.Y, maxntid.Z);
                                throw AssertionHelper.Fail();
                            }
                            else
                            {
                                maxntid = new dim3(0, 0, 0);
                            }
                        }

                        var minnctapersm = int.Parse(directives.GetOrDefault("minnctapersm", "0"));
                        if (Minnctapersm != 0)
                        {
                            if (Minnctapersm < minnctapersm)
                            {
                                log.WriteLine("Conflict! New min thread blocks per SM ({0}) is incompatible with original value ({1}).", Minnctapersm, minnctapersm);
                                throw AssertionHelper.Fail();
                            }
                            else
                            {
                                minnctapersm = Minnctapersm;
                            }
                        }

                        var maxnctapersm = int.Parse(directives.GetOrDefault("maxnctapersm", "0"));
                        if (Maxnctapersm != 0) 
                        {
                            if (Maxnctapersm > maxnctapersm)
                            {
                                log.WriteLine("Conflict! New max thread blocks per SM ({0}) is incompatible with original value ({1}).", Maxnctapersm, maxnctapersm);
                                throw AssertionHelper.Fail();
                            }
                            else
                            {
                                maxnctapersm = Maxnctapersm;
                            }
                        }

                        if (minnctapersm != 0 && maxnctapersm != 0)
                        {
                            if (minnctapersm > maxnctapersm)
                            {
                                log.WriteLine("Conflict! Min thread blocks per SM ({0}) and max thread blocks per SM ({1}) are incompatible.", minnctapersm, maxnctapersm);
                                throw AssertionHelper.Fail();
                            }
                        }

                        log.Write("Applying compilation parameters... ");
                        var tuning = new JitTuning{Maxnreg = maxnreg, Maxntid = maxntid, Reqntid = reqntid, Minnctapersm = minnctapersm, Maxnctapersm = maxnctapersm};
                        tuning.Validate();
                        var replacement = m["header"] + Environment.NewLine + tuning.RenderPtx() + Environment.NewLine + "{";

                        log.WriteLine("Success.");
                        return replacement;
                    }
                    else
                    {
                        log.WriteLine("Found entry \"{0}\" without performance tuning directives.", name);

                        log.Write("Applying compilation parameters... ");
                        var replacement = m["header"] + Environment.NewLine + Tuning.RenderPtx() + Environment.NewLine + "{";

                        log.WriteLine("Success.");
                        return replacement;
                    }
                });
            }

            log.EnsureBlankLine();
            log.WriteLine("*".Repeat(120));
            log.WriteLine(ptx.TrimEnd());
            log.WriteLine(120.Times("*"));

            var options = new CUjit_options();
            options.OptimizationLevel = OptimizationLevel;
            options.PlannedThreadsPerBlock = Reqntid.Product();
            // todo. an attempt to pass the Target value directly leads to CUDA_ERROR_INVALID_VALUE
            // as of now, this feature is not really important, so I'm marking it as TBI
            options.TargetFromContext = TargetFromContext.AssertTrue();
            options.Target = Target.ToCUjit_target();
            options.FallbackStrategy = CUjit_fallbackstrategy.PreferPtx;

            var native_result = nvcuda.cuModuleLoadDataEx(ptx, options);
            return new JitResult(this, ptx, native_result);
        }
    }
}