using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using Libcuda.Native;
using Libcuda.Native.DataTypes;
using Libcuda.Native.Exceptions;
using XenoGears.Functional;
using XenoGears.Traits.Disposable;

namespace Libcuda.Jit
{
    [DebuggerNonUserCode]
    public class JittedModule : Disposable
    {
        public JitResult JitResult { get; private set; }
        public String Ptx { get { return JitResult.Ptx; } }

        internal CUmodule Handle { get; private set; }
        public ReadOnlyCollection<JittedFunction> Functions { get; private set; }

        internal JittedModule(JitResult jitResult, CUmodule handle)
        {
            JitResult = jitResult;
            Handle = handle;

            var match = Regex.Match(Ptx, @"\.entry\s*(?<entrypoint>\w*?)\s*\(");
            Functions = match.Unfoldi(m => m.NextMatch(), m => m.Success).Select(m =>
            {
                var name = match.Result("${entrypoint}");
                return new JittedFunction(this, name);
            }).ToReadOnly();
        }

        protected override void DisposeUnmanagedResources()
        {
            if (Handle != null)
            {
                var error1 = nvcuda.cuModuleUnload(Handle);
                if (error1 != CUresult.Success) throw new CudaException(error1);
            }
        }
    }
}