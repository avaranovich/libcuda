using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using Libcuda.Api.Native;
using Libcuda.Api.Native.DataTypes;
using System.Linq;
using XenoGears.Assertions;
using XenoGears.Functional;
using XenoGears.Strings;
using XenoGears.Traits.Dumpable;

namespace Libcuda.Api.Devices
{
    [DebuggerNonUserCode]
    public partial class CudaDevice : IDumpableAsText
    {
        public CUdevice Handle { get; private set; }
        public static implicit operator CUdevice(CudaDevice device) { return device == null ? CUdevice.Null : device.Handle; }
        public static explicit operator CudaDevice(CUdevice device) { return device.IsNull ? null : new CudaDevice(device); }

        public int Index { get; private set; }
        public String Name { get; private set; }
        public int PciBusId { get; private set; }
        public int PciDeviceId { get; private set; }

        public SimdSpec Simd { get; private set; }
        public ClockSpec Clock { get; private set; }
        public MemorySpec Memory { get; private set; }
        public DeviceCaps Caps { get; private set; }

        public CudaDevice(int index)
        {
            CudaDriver.Ensure();

            Index = index;
            Handle = nvcuda.cuDeviceGet(index);

            Name = nvcuda.cuDeviceGetName(this);
            PciBusId = nvcuda.cuDeviceGetAttribute(CUdevice_attribute.PciBusId, this);
            PciDeviceId = nvcuda.cuDeviceGetAttribute(CUdevice_attribute.PciDeviceId, this);

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