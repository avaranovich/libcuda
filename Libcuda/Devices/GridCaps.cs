using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using Libcuda.DataTypes;
using Libcuda.Native;
using Libcuda.Native.DataTypes;
using Libcuda.Native.Exceptions;
using XenoGears.Traits.Dumpable;

namespace Libcuda.Devices
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
            var h_device = device.Handle;

            int maxThreadsInBlock;
            var error1 = nvcuda.cuDeviceGetAttribute(out maxThreadsInBlock, CUdevice_attribute.MaxThreadsPerBlock, h_device);
            if (error1 != CUresult.Success) throw new CudaException(error1);
            MaxThreadsInBlock = maxThreadsInBlock;

            int maxBlockX, maxBlockY, maxBlockZ;
            var error2 = nvcuda.cuDeviceGetAttribute(out maxBlockX, CUdevice_attribute.MaxBlockDimX, h_device);
            if (error2 != CUresult.Success) throw new CudaException(error2);
            var error3 = nvcuda.cuDeviceGetAttribute(out maxBlockY, CUdevice_attribute.MaxBlockDimY, h_device);
            if (error3 != CUresult.Success) throw new CudaException(error3);
            var error4 = nvcuda.cuDeviceGetAttribute(out maxBlockZ, CUdevice_attribute.MaxBlockDimZ, h_device);
            if (error4 != CUresult.Success) throw new CudaException(error4);
            MaxBlockDim = new dim3(maxBlockX, maxBlockY, maxBlockZ);

            int maxGridX, maxGridY, maxGridZ;
            var error5 = nvcuda.cuDeviceGetAttribute(out maxGridX, CUdevice_attribute.MaxGridDimX, h_device);
            if (error5 != CUresult.Success) throw new CudaException(error5);
            var error6 = nvcuda.cuDeviceGetAttribute(out maxGridY, CUdevice_attribute.MaxGridDimY, h_device);
            if (error6 != CUresult.Success) throw new CudaException(error6);
            var error7 = nvcuda.cuDeviceGetAttribute(out maxGridZ, CUdevice_attribute.MaxGridDimZ, h_device);
            if (error7 != CUresult.Success) throw new CudaException(error7);
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