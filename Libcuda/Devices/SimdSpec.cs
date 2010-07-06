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
    public class SimdSpec : IDumpableAsText
    {
        public CudaDevice Device { get; private set; }
        public int Cores { get { return SMs * 8; } }
        public int SMs { get; private set; }
        public int SimdWidth { get; private set; }

        internal SimdSpec(CudaDevice device)
        {
            Device = device;
            var h_device = device.Handle;

            int sms;
            var error1 = nvcuda.cuDeviceGetAttribute(out sms, CUdevice_attribute.MultiProcessorCount, h_device);
            if (error1 != CUresult.Success) throw new CudaException(error1);
            SMs = sms;

            int simdWidth;
            var error2 = nvcuda.cuDeviceGetAttribute(out simdWidth, CUdevice_attribute.WarpSize, h_device);
            if (error2 != CUresult.Success) throw new CudaException(error2);
            SimdWidth = simdWidth;
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