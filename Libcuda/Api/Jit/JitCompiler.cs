using System;
using System.Collections.Generic;
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

            // here we attempt to rewrite PTX by injecting performance tuning directives directly into source codes
            if (Maxnreg != 0 || Maxntid != new dim3() || Reqntid != new dim3() || Minnctapersm != 0 || Maxnctapersm != 0)
            {
                (Maxnreg >= 0).AssertTrue();
                (Maxntid.X >= 1 && Maxntid.Y >= 1 && Maxntid.Z >= 1).AssertTrue();
                (Reqntid.X >= 1 && Reqntid.Y >= 1 && Reqntid.Z >= 1).AssertTrue();
                (Minnctapersm >= 0).AssertTrue();
                (Maxnctapersm >= 0).AssertTrue();

                log.EnsureBlankLine();
                log.WriteLine("Detected non-trivial performance tuning parameters...");
                log.WriteLine("    Max registers per thread                     : {0}", Maxnreg);
                log.WriteLine("    Max threads in thread block                  : {0} x {1} x {2}", Maxntid.X, Maxntid.Y, Maxntid.Z);
                log.WriteLine("    Required threads in thread block             : {0} x {1} x {2}", Reqntid.X, Reqntid.Y, Reqntid.Z);
                log.WriteLine("    Min thread blocks per SM                     : {0}", Minnctapersm);
                log.WriteLine("    Max thread blocks per SM                     : {0}", Maxnctapersm);
                log.WriteLine("    Optimization level (0 - 4, higher is better) : {0}", OptimizationLevel);

                Func<int, dim3, dim3, int, int, String> render = (maxnreg, maxntid, reqntid, minnctapersm, maxnctapersm) =>
                {
                    var directives = new List<String>();
                    if (maxnreg != 0) directives.Add(String.Format(".maxnreg {0}", maxnreg));
                    if (maxntid != new dim3()) directives.Add(String.Format(".maxntid {0}, {1}, {2}", maxntid.X, maxntid.Y, maxntid.Z));
                    if (reqntid != new dim3()) directives.Add(String.Format(".reqntid {0}, {1}, {2}", reqntid.X, reqntid.Y, reqntid.Z));
                    if (minnctapersm != 0) directives.Add(String.Format(".minnctapersm {0}", minnctapersm));
                    if (maxnctapersm != 0) directives.Add(String.Format(".maxnctapersm {0}", maxnctapersm));
                    return directives.StringJoin(Environment.NewLine);
                };

                log.EnsureBlankLine();
                log.WriteLine("To apply them it is necessary to perform PTX rewriting and inject corresponding directives directly into source codes.");
                log.WriteLine("Analyzing entries in PTX module...");
                var rx_entry = @"(?<header>\.entry\s+(?<name>([a-zA-Z][a-zA-Z0-9_$]*)|([_$%][a-zA-Z0-9_$]*))\s*(?<params>\(.*?\))?)\s*(?<directives>\..*?)?\s*\{";
                ptx = ptx.Replace(rx_entry, RegexOptions.Singleline, m =>
                {
                    var name = m["name"];
                    var directives = m["directives"].Split('.').Trim().Select(s => s.Parse(@"$(?<name>\w+)\s+(?<value>.*?)^")).ToDictionary(m1 => m1["name"].Trim(), m1 => m1["value"].Trim()).ToReadOnly();
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
                        if (Maxnreg != 0 && Maxnreg > maxnreg)
                        {
                            log.WriteLine("Conflict! New max threads in thread block ({0}) is incompatible with original value ({1}).", Maxnreg, maxnreg);
                            throw AssertionHelper.Fail();
                        }
                        else
                        {
                            maxnreg = Maxnreg;
                        }

                        var maxntid = parse_dim3(directives.GetOrDefault("maxntid", "1, 1, 1"));
                        if (Maxntid != new dim3() && Maxntid > maxntid)
                        {
                            log.WriteLine("Conflict! New max threads in thread block ({0}, {1}, {2}) is incompatible with original value ({3}, {4}, {5}).", Maxntid.X, Maxntid.Y, Maxntid.Z, maxntid.X, maxntid.Y, maxntid.Z);
                            throw AssertionHelper.Fail();
                        }
                        else
                        {
                            maxntid = Maxntid;
                        }

                        var reqntid = parse_dim3(directives.GetOrDefault("reqntid", "1, 1, 1"));
                        if (Reqntid != new dim3() && Reqntid != reqntid)
                        {
                            log.WriteLine("Conflict! New required threads in thread block ({0}, {1}, {2}) is incompatible with original value ({3}, {4}, {5}).", Reqntid.X, Reqntid.Y, Reqntid.Z, reqntid.X, reqntid.Y, reqntid.Z);
                            throw AssertionHelper.Fail();
                        }
                        else
                        {
                            reqntid = Reqntid;
                        }

                        var minnctapersm = int.Parse(directives.GetOrDefault("minnctapersm", "0"));
                        if (Minnctapersm != 0 && Minnctapersm < minnctapersm)
                        {
                            log.WriteLine("Conflict! New min thread blocks per SM ({0}) is incompatible with original value ({1}).", Minnctapersm, minnctapersm);
                            throw AssertionHelper.Fail();
                        }
                        else
                        {
                            minnctapersm = Minnctapersm;
                        }

                        var maxnctapersm = int.Parse(directives.GetOrDefault("maxnctapersm", "0"));
                        if (Maxnctapersm != 0 && Maxnctapersm > maxnctapersm)
                        {
                            log.WriteLine("Conflict! New max thread blocks per SM ({0}) is incompatible with original value ({1}).", Maxnctapersm, maxnctapersm);
                            throw AssertionHelper.Fail();
                        }
                        else
                        {
                            maxnctapersm = Maxnctapersm;
                        }

                        var replacement = m["header"] + Environment.NewLine + render(maxnreg, maxntid, reqntid, minnctapersm, maxnctapersm) + Environment.NewLine + "{";
                        return replacement;
                    }
                    else
                    {
                        log.WriteLine("Found entry \"{0}\" without performance tuning directives.", name);
                        log.Write("Applying compilation parameters... Success.");
                        var replacement = m["header"] + Environment.NewLine + render(Maxnreg, Maxntid, Reqntid, Minnctapersm, Maxnctapersm) + Environment.NewLine + "{";
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