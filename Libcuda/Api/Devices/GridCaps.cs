using System;
using System.Diagnostics;
using System.IO;
using Libcuda.DataTypes;
using Libcuda.Api.Native;
using Libcuda.Api.Native.DataTypes;
using XenoGears.Traits.Dumpable;

namespace Libcuda.Api.Devices
{
    [DebuggerNonUserCode]
    public class GridCaps : IDumpableAsText
    {
        public CudaDevice Device { get; private set; }
        public int MaxThreadsInBlock { get; private set; }
        public dim3 MaxBlockDim { get; private set; }
        public dim3 MaxGridDim { get; private set; }

        internal GridCaps(CudaDevice device)
        {
            Device = device;

            MaxThreadsInBlock = nvcuda.cuDeviceGetAttribute(CUdevice_attribute.MaxThreadsPerBlock, device);

            var maxBlockX = nvcuda.cuDeviceGetAttribute(CUdevice_attribute.MaxBlockDimX, device);
            var maxBlockY = nvcuda.cuDeviceGetAttribute(CUdevice_attribute.MaxBlockDimY, device);
            var maxBlockZ = nvcuda.cuDeviceGetAttribute(CUdevice_attribute.MaxBlockDimZ, device);
            MaxBlockDim = new dim3(maxBlockX, maxBlockY, maxBlockZ);

            var maxGridX = nvcuda.cuDeviceGetAttribute(CUdevice_attribute.MaxGridDimX, device);
            var maxGridY = nvcuda.cuDeviceGetAttribute(CUdevice_attribute.MaxGridDimY, device);
            var maxGridZ = nvcuda.cuDeviceGetAttribute(CUdevice_attribute.MaxGridDimZ, device);
            MaxGridDim = new dim3(maxGridX, maxGridY, maxGridZ);
        }

        public override String ToString() { return ((IDumpableAsText)this).DumpAsText(); }
        void IDumpableAsText.DumpAsText(TextWriter writer)
        {
            writer.WriteLine("Grid caps of device #{0} \"{1}\" (/pci:{2}/dev:{3})", Device.Index, Device.Name, Device.PciBusId, Device.PciDeviceId);
            writer.WriteLine("    Max threads in block       : {0}", MaxThreadsInBlock);
            writer.WriteLine("    Max block size (X x Y x Z) : {0} x {1} x {2}", MaxBlockDim.X, MaxBlockDim.Y, MaxBlockDim.Z);
            writer.WriteLine("    Max grid size (X x Y x Z)  : {0} x {1} x {2}", MaxGridDim.X, MaxGridDim.Y, MaxGridDim.Z);
        }
    }
}