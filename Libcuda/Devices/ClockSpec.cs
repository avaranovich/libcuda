using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using Libcuda.Native;
using Libcuda.Native.DataTypes;
using Libcuda.Native.Exceptions;
using XenoGears.Traits.Dumpable;

namespace Libcuda.Devices
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
            var h_device = device.Handle;

            int clockKhz;
            var error24 = nvcuda.cuDeviceGetAttribute(out clockKhz, CUdevice_attribute.ClockRate, h_device);
            if (error24 != CUresult.Success) throw new CudaException(error24);
            ClockKhz = clockKhz;
        }

        public override String ToString() { return ((IDumpableAsText)this).DumpAsText(); }
        void IDumpableAsText.DumpAsText(TextWriter writer)
        {
            writer.WriteLine("Clock spec of device #{0} \"{1}\" (/pci:{2}/dev:{3})", Device.Index, Device.Name, Device.PciBusId, Device.PciDeviceId);
            writer.WriteLine("    ALU clock : {0:0.00} GHz", ClockGhz);
        }
    }
}