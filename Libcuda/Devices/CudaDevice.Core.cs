using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using Libcuda.Native;
using Libcuda.Native.DataTypes;
using Libcuda.Native.Exceptions;
using System.Linq;
using XenoGears.Functional;
using XenoGears.Strings;
using XenoGears.Traits.Dumpable;

namespace Libcuda.Devices
{
    [DebuggerNonUserCode]
    public partial class CudaDevice : IDumpableAsText
    {
        internal CUdevice Handle { get; private set; }
        public int Index { get; private set; }
        public String Name { get; private set; }
        public int PciBusId { get; private set; }
        public int PciDeviceId { get; private set; }

        public SimdSpec Simd { get; private set; }
        public ClockSpec Clock { get; private set; }
        public MemorySpec Memory { get; private set; }
        public DeviceCaps Caps { get; private set; }

        internal CudaDevice(int index)
        {
            Index = index;

            CUdevice h_device;
            var error1 = nvcuda.cuDeviceGet(out h_device, index);
            if (error1 != CUresult.Success) throw new CudaException(error1);
            Handle = h_device;

            var deviceName_buf = new StringBuilder{Capacity = 255};
            var error2 = nvcuda.cuDeviceGetName(deviceName_buf, deviceName_buf.Capacity, h_device);
            if (error2 != CUresult.Success) throw new CudaException(error2);
            Name = deviceName_buf.ToString();

            int bus_id;
            var error3 = nvcuda.cuDeviceGetAttribute(out bus_id, CUdevice_attribute.PciBusId, h_device);
            if (error3 != CUresult.Success) throw new CudaException(error3);
            PciBusId = bus_id;

            int device_id;
            var error4 = nvcuda.cuDeviceGetAttribute(out device_id, CUdevice_attribute.PciDeviceId, h_device);
            if (error4 != CUresult.Success) throw new CudaException(error4);
            PciDeviceId = device_id;

            Simd = new SimdSpec(this);
            Clock = new ClockSpec(this);
            Memory = new MemorySpec(this);
            Caps = new DeviceCaps(this);
        }

        public override String ToString() { return ((IDumpableAsText)this).DumpAsText(); }
        void IDumpableAsText.DumpAsText(TextWriter writer)
        {
            var pbag = new List<KeyValuePair<String, String>>();
            Func<KeyValuePair<String, String>, String> fmt = kvp =>
            {
                var maxKey = pbag.Max(kvp1 => kvp1.Key.Length);
                return String.Format("    {0} : {1}", kvp.Key.PadRight(maxKey), kvp.Value);
            };

            Action<String> fillPbag = s =>
            {
                foreach (var line in s.SplitLines().Skip(1).SkipLast(1))
                {
                    var m = Regex.Match(line, "^(?<key>.*?):(?<value>.*)$");
                    var key = m.Result("${key}").Trim();
                    var value = m.Result("${value}").Trim();
                    pbag.Add(new KeyValuePair<String, String>(key, value));
                }
            };

            writer.WriteLine("Device #{0} \"{1}\" (/pci:{2}/dev:{3})", Index, Name, PciBusId, PciDeviceId);
            fillPbag(Simd.ToString());
            fillPbag(Clock.ToString());
            fillPbag(Memory.ToString());
            fillPbag(Caps.ToString());
            pbag.ForEach(kvp => writer.WriteLine(fmt(kvp)));
        }
    }
}