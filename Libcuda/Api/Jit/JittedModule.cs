using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using Libcuda.Api.Native;
using Libcuda.Api.Native.DataTypes;
using XenoGears.Functional;
using XenoGears.Traits.Disposable;
using XenoGears.Assertions;

namespace Libcuda.Api.Jit
{
    [DebuggerNonUserCode]
    public class JittedModule : Disposable
    {
        public JitResult JitResult { get; private set; }
        public String Ptx { get; private set; }

        public CUmodule Handle { get; private set; }
        public ReadOnlyCollection<JittedFunction> Functions { get; private set; }
        public static implicit operator CUmodule(JittedModule mod) { return mod == null ? CUmodule.Null : mod.Handle; }
        public override String ToString() { return Handle.ToString(); }

        public JittedModule(JitResult jitResult, CUmodule handle)
            : this(jitResult.Ptx, handle)
        {
            JitResult = jitResult.AssertNotNull();
        }

        public JittedModule(String ptx, CUmodule handle)
        {
            CudaDriver.Ensure();
            Ptx = ptx.AssertNotNull();
            Handle = handle.AssertThat(h => h.IsNotNull);

            var match = Regex.Match(Ptx, @"\.entry\s*(?<entrypoint>\w*?)\s*\(");
            Functions = match.Unfoldi(m => m.NextMatch(), m => m.Success).Select(m =>
            {
                var name = match.Result("${entrypoint}");
                var hfunc = nvcuda.cuModuleGetFunction(this, name);
                return new JittedFunction(hfunc, name);
            }).ToReadOnly();
        }

        protected override void DisposeUnmanagedResources()
        {
            if (Handle.IsNotNull)
            {
                nvcuda.cuModuleUnload(Handle);
            }
        }
    }
}