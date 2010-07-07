using System;
using System.Diagnostics;
using System.IO;
using Libcuda.Api.Native;
using Libcuda.Api.Native.DataTypes;
using XenoGears.Traits.Dumpable;

namespace Libcuda.Api.Devices
{
    [DebuggerNonUserCode]
    public class SimdSpec : IDumpableAsText
    {
        public CudaDevice Device { get; private set; }
        public int Cores { get { return SMs * 8; } }
        public int SMs { get; private set; }
        public int SimdWidth { get; private set; }

        internal SimdSpec(CudaDevice device)
        {
            Device = device;

            SMs = nvcuda.cuDeviceGetAttribute(CUdevice_attribute.MultiProcessorCount, device);
            SimdWidth = nvcuda.cuDeviceGetAttribute(CUdevice_attribute.WarpSize, device);
        }

        public override String ToString() { return ((IDumpableAsText)this).DumpAsText(); }
        void IDumpableAsText.DumpAsText(TextWriter writer)
        {
            writer.WriteLine("SIMD spec of device #{0} \"{1}\" (/pci:{2}/dev:{3})", Device.Index, Device.Name, Device.PciBusId, Device.PciDeviceId);
            writer.WriteLine("    ALU        : {0} multiprocessors ({1} cores)", SMs, Cores);
            writer.WriteLine("    SIMD Width : {0} threads in warp", SimdWidth);
        }
    }
}