using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using Libcuda.Api.Native;
using XenoGears.Reflection.Attributes;
using XenoGears.Reflection.Shortcuts;

namespace Libcuda
{
    [DebuggerNonUserCode]
    public static class CudaDriver
    {
        public static void Ensure()
        {
            nvcuda.Ensure();
        }

        public static FileInfo Current
        {
            get
            {
                // todo. this won't work under Linux
                var sample = typeof(nvcuda).GetMethod("nativeDriverGetVersion", BF.All);
                var filename = sample.Attr<DllImportAttribute>().Value + ".dll";
                var system_dir = Environment.GetFolderPath(Environment.SpecialFolder.System);
                var fileInfo = new FileInfo(System.IO.Path.Combine(system_dir, filename));
                return fileInfo.Exists ? fileInfo : null;
            }
        }
    }
}