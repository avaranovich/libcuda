using System;
using System.Diagnostics;
using System.Linq;
using Libcuda.Api.Jit;
using Libcuda.Api.Run;
using NUnit.Framework;
using XenoGears.Functional;
using XenoGears.Reflection;
using XenoGears.Reflection.Attributes;
using XenoGears.Reflection.Generics;
using XenoGears.Strings;
using Log=XenoGears.Logging.Log;

namespace Libcuda.Playground
{
    [TestFixture]
    public class MetaTests
    {
        [Test]
        public void EnsureMostStuffIsMarkedWithDebuggerNonUserCode()
        {
            var asm = typeof(JitFacade).Assembly;
            var types = asm.GetTypes().Where(t => !t.IsInterface).ToReadOnly();
            var failed_types = types
                .Where(t => !t.HasAttr<DebuggerNonUserCodeAttribute>())
                .Where(t => !t.IsCompilerGenerated())
                .Where(t => !t.Name.Contains("<>"))
                .Where(t => !t.Name.Contains("__StaticArrayInit"))
                .Where(t => !t.IsEnum)
                .Where(t => !t.IsDelegate())
                // exceptions for meaty logic
                .Where(t => t.Namespace != typeof(JitCompiler).Namespace)
                .Where(t => t.Namespace != typeof(KernelInvocation).Namespace)
                .ToReadOnly();

            if (failed_types.IsNotEmpty())
            {
                Log.WriteLine(String.Format("{0} types in Libcuda aren't marked with [DebuggerNonUserCode]:", failed_types.Count()));
                var messages = failed_types.Select(t => t.GetCSharpRef(ToCSharpOptions.InformativeWithNamespaces));
                messages.OrderDescending().ForEach(message => Trace.WriteLine(message));
                Assert.Fail();
            }
        }
    }
}