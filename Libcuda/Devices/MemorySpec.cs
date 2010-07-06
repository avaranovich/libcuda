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
    public class MemorySpec : IDumpableAsText
    {
        public CudaDevice Device { get; private set; }
        public MemoryCaps Caps { get; private set; }

        public int RegistersPerSM { get; private set; }
        public int Registers { get { return RegistersPerSM * Device.Simd.SMs; } }
        public int RegisterMemory { get { return Registers * 32 / 8; } }
        public int SharedMemoryPerSM { get; private set; }
        public int SharedMemory { get { return SharedMemoryPerSM * Device.Simd.SMs; } }
        public int ConstantMemory { get; private set; }
        public long GlobalMemory { get; private set; }

        internal MemorySpec(CudaDevice device)
        {
            Device = device;
            var h_device = device.Handle;
            Caps = new MemoryCaps(device);

            int registersPerSM;
            var error1 = nvcuda.cuDeviceGetAttribute(out registersPerSM, CUdevice_attribute.MaxRegistersPerBlock, h_device);
            if (error1 != CUresult.Success) throw new CudaException(error1);
            RegistersPerSM = registersPerSM;

            int sharedMemoryPerSM;
            var error2 = nvcuda.cuDeviceGetAttribute(out sharedMemoryPerSM, CUdevice_attribute.MaxSharedMemoryPerBlock, h_device);
            if (error2 != CUresult.Success) throw new CudaException(error2);
            SharedMemoryPerSM = sharedMemoryPerSM;

            int constantMemory;
            var error3 = nvcuda.cuDeviceGetAttribute(out constantMemory, CUdevice_attribute.TotalConstantMemory, h_device);
            if (error3 != CUresult.Success) throw new CudaException(error3);
            ConstantMemory = constantMemory;

            uint globalMemory;
            var error4 = nvcuda.cuDeviceTotalMem(out globalMemory, h_device);
            if (error4 != CUresult.Success) throw new CudaException(error4);
            GlobalMemory = globalMemory;
        }

        public override String ToString() { return ((IDumpableAsText)this).DumpAsText(); }
        void IDumpableAsText.DumpAsText(TextWriter writer)
        {
            writer.WriteLine("Memory spec of device #{0} \"{1}\" (/pci:{2}/dev:{3})", Device.Index, Device.Name, Device.PciBusId, Device.PciDeviceId);
            writer.WriteLine("    Registers (per multiprocessor)     : {0} x {1} bits", RegistersPerSM, 32);
            writer.WriteLine("    Shared memory (per multiprocessor) : {0} bytes", SharedMemoryPerSM);
            writer.WriteLine("    Constant memory (device-wide)      : {0} bytes", ConstantMemory);
            writer.WriteLine("    Global memory (device-wide)        : {0} bytes", GlobalMemory);
            writer.WriteLine("    Max pitch                          : {0} bytes", Caps.MaxPitch);
            writer.WriteLine("    Texture alignment                  : {0} bytes", Caps.TextureAlignment);
            writer.WriteLine("    Max texture 1d (W)                 : {0}", Caps.MaxTexture1dWidth);
            writer.WriteLine("    Max texture 2d (W x H)             : {0} x {1}", Caps.MaxTexture2dWidth, Caps.MaxTexture2dHeight);
            writer.WriteLine("    Max texture 3d (W x H x D)         : {0} x {1} x {2}", Caps.MaxTexture3dWidth, Caps.MaxTexture3dHeight, Caps.MaxTexture3dDepth);
            writer.WriteLine("    Max texture 2d array (W x H)       : {0} x {1}", Caps.MaxTexture2dArrayWidth, Caps.MaxTexture2dHeight);
            writer.WriteLine("    Max texture 2d array splices       : {0}", Caps.MaxTexture2dArrayNumSlices);
            writer.WriteLine("    Surface alignment                  : {0} bytes", Caps.SurfaceAlignment);
        }
    }
}