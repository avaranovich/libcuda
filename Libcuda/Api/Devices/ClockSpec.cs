using System;
using System.Diagnostics;
using System.IO;
using Libcuda.Api.Native;
using Libcuda.Api.Native.DataTypes;
using XenoGears.Traits.Dumpable;

namespace Libcuda.Api.Devices
{
    [DebuggerNonUserCode]
    public class ClockSpec : IDumpableAsText
    {
        public CudaDevice Device { get; private set; }
        public float ClockHz { get { return ClockKhz * 1000; } }
        public float ClockKhz { get; private set; }
        public float ClockMhz { get { return ClockKhz / 1000; } }
        public float ClockGhz { get { return ClockMhz / 1000; } }

        internal ClockSpec(CudaDevice device)
        {
            Device = device;

            ClockKhz = nvcuda.cuDeviceGetAttribute(CUdevice_attribute.ClockRate, device);
        }

        public override String ToString() { return ((IDumpableAsText)this).DumpAsText(); }
        void IDumpableAsText.DumpAsText(TextWriter writer)
        {
            writer.WriteLine("Clock spec of device #{0} \"{1}\" (/pci:{2}/dev:{3})", Device.Index, Device.Name, Device.PciBusId, Device.PciDeviceId);
            writer.WriteLine("    ALU clock : {0:0.00} GHz", ClockGhz);
        }
    }
}