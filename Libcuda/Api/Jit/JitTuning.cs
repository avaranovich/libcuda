using System;
using System.Collections.Generic;
using System.IO;
using Libcuda.DataTypes;
using XenoGears.Functional;
using XenoGears.Assertions;
using XenoGears.Traits.Dumpable;

namespace Libcuda.Api.Jit
{
    public class JitTuning : IDumpableAsText
    {
        public int Maxnreg { get; set; }
        public dim3 Maxntid { get; set; }
        public dim3 Reqntid { get; set; }
        public int Minnctapersm { get; set; }
        public int Maxnctapersm { get; set; }

        public JitTuning()
        {
            Maxnreg = 0;
            Maxntid = new dim3(0, 0, 0);
            Reqntid = new dim3(0, 0, 0);
            Minnctapersm = 0;
            Maxnctapersm = 0;
        }

        public bool IsTrivial
        {
            get
            {
                var os_trivial = true;
                os_trivial &= Maxnreg == 0;
                os_trivial &= Maxntid == new dim3(0, 0, 0);
                os_trivial &= Reqntid == new dim3(0, 0, 0);
                os_trivial &= Minnctapersm == 0;
                os_trivial &= Maxnctapersm == 0;
                return os_trivial;
            }
        }

        public bool IsNotTrivial
        {
            get { return !IsTrivial; }
        }

        public void Validate()
        {
            (Maxnreg >= 0).AssertTrue();
            (Maxntid == new dim3(0, 0, 0) || Maxntid >= new dim3(1, 1, 1)).AssertTrue();
            (Reqntid == new dim3(0, 0, 0) || Reqntid >= new dim3(1, 1, 1)).AssertTrue();
            (Minnctapersm >= 0).AssertTrue();
            (Maxnctapersm >= 0).AssertTrue();

            (Maxntid != new dim3(0, 0, 0) && Reqntid != new dim3(0, 0, 0)).AssertFalse();
            (Minnctapersm != 0 && Maxnctapersm != 0).AssertImplies(Minnctapersm <= Maxnctapersm);
        }

        public String RenderPtx()
        {
            var @params = new List<String>();
            if (Maxnreg != 0) @params.Add(String.Format(".maxnreg {0}", Maxnreg));
            if (Maxntid != new dim3()) @params.Add(String.Format(".maxntid {0}, {1}, {2}", Maxntid.X, Maxntid.Y, Maxntid.Z));
            if (Reqntid != new dim3()) @params.Add(String.Format(".reqntid {0}, {1}, {2}", Reqntid.X, Reqntid.Y, Reqntid.Z));
            if (Minnctapersm != 0) @params.Add(String.Format(".minnctapersm {0}", Minnctapersm));
            if (Maxnctapersm != 0) @params.Add(String.Format(".maxnctapersm {0}", Maxnctapersm));
            return @params.StringJoin(Environment.NewLine);
        }

        public override String ToString() { return ((IDumpableAsText)this).DumpAsText(); }
        void IDumpableAsText.DumpAsText(TextWriter writer)
        {
            writer.WriteLine("Performance tuning parameters:");
            writer.WriteLine("    Max registers per thread                     : {0}", Maxnreg);
            writer.WriteLine("    Max threads in thread block                  : {0} x {1} x {2}", Maxntid.X, Maxntid.Y, Maxntid.Z);
            writer.WriteLine("    Required threads in thread block             : {0} x {1} x {2}", Reqntid.X, Reqntid.Y, Reqntid.Z);
            writer.WriteLine("    Min thread blocks per SM                     : {0}", Minnctapersm);
            writer.WriteLine("    Max thread blocks per SM                     : {0}", Maxnctapersm);
        }
    }
}