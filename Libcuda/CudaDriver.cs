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
        public static String Name
        {
            get
            {
                var sample = typeof(nvcuda).GetMethod("nativeDriverGetVersion", BF.All);
                return sample.Attr<DllImportAttribute>().Value + ".dll";
            }
        }

        public static FileInfo FileInfo
        {
            get
            {
                var system_dir = Environment.GetFolderPath(Environment.SpecialFolder.System);
                var fileInfo = new FileInfo(System.IO.Path.Combine(system_dir, Name));
                return fileInfo.Exists ? fileInfo : null;
            }
        }

        public static Version Version
        {
            get
            {
                if (FileInfo == null) return null;
                var fvi = FileVersionInfo.GetVersionInfo(FileInfo.FullName);
                return new Version(fvi.FileMajorPart, fvi.FileMinorPart, fvi.FileBuildPart, fvi.FilePrivatePart);
            }
        }
    }
}