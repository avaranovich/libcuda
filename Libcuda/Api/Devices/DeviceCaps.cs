using System;
using System.Diagnostics;
using System.IO;
using Libcuda.Api.Native;
using Libcuda.Api.Native.DataTypes;
using Libcuda.Versions;
using XenoGears.Traits.Dumpable;

namespace Libcuda.Api.Devices
{
    [DebuggerNonUserCode]
    public class DeviceCaps : IDumpableAsText
    {
        public CudaDevice Device { get; private set; }
        public HardwareIsa ComputeCaps { get; private set; }
        public FloatCaps FloatCaps { get; private set; }
        public GridCaps GridCaps { get; private set; }
        public MemoryCaps MemoryCaps { get; private set; }
        public ComputeMode ComputeMode { get; private set; }
        public bool IsKernelTimeoutEnabled { get; private set; }
        public bool IsIntegrated { get; private set; }
        public bool SupportsHostMemoryMapping { get; private set; }
        public bool SupportsGpuOverlap { get; private set; }
        public bool SupportsConcurrentKernels { get; private set; }
        public bool IsEccEnabled { get; private set; }

        internal DeviceCaps(CudaDevice device)
        {
            Device = device;
            ComputeCaps = nvcuda.cuDeviceComputeCapability(device);
            FloatCaps = ((int)ComputeCaps >= 13) ? FloatCaps.NativeDoubles : FloatCaps.EmulateDoubles;
            GridCaps = new GridCaps(device);
            MemoryCaps = new MemoryCaps(device);

            ComputeMode = (ComputeMode)nvcuda.cuDeviceGetAttribute(CUdevice_attribute.ComputeMode, device);
            IsKernelTimeoutEnabled = nvcuda.cuDeviceGetFlag(CUdevice_attribute.KernelExecTimeout, device);
            IsIntegrated = nvcuda.cuDeviceGetFlag(CUdevice_attribute.Integrated, device);
            SupportsHostMemoryMapping = nvcuda.cuDeviceGetFlag(CUdevice_attribute.CanMapHostMemory, device);
            SupportsGpuOverlap = nvcuda.cuDeviceGetFlag(CUdevice_attribute.GPUOverlap, device);
            SupportsConcurrentKernels = nvcuda.cuDeviceGetFlag(CUdevice_attribute.ConcurrentKernels, device);
            IsEccEnabled = nvcuda.cuDeviceGetFlag(CUdevice_attribute.EccEnabled, device);
        }

        public override String ToString() { return ((IDumpableAsText)this).DumpAsText(); }
        void IDumpableAsText.DumpAsText(TextWriter writer)
        {
            writer.WriteLine("Caps of device #{0} \"{1}\" (/pci:{2}/dev:{3})", Device.Index, Device.Name, Device.PciBusId, Device.PciDeviceId);
            writer.WriteLine("    Compute capability           : {0}.{1}", (int)ComputeCaps / 10, (int)ComputeCaps % 10);
            writer.WriteLine("    Max threads in block         : {0}", GridCaps.MaxThreadsInBlock);
            writer.WriteLine("    Max block size (X x Y x Z)   : {0} x {1} x {2}", GridCaps.MaxBlockDim.X, GridCaps.MaxBlockDim.Y, GridCaps.MaxBlockDim.Z);
            writer.WriteLine("    Max grid size (X x Y x Z)    : {0} x {1} x {2}", GridCaps.MaxGridDim.X, GridCaps.MaxGridDim.Y, GridCaps.MaxGridDim.Z);
            writer.WriteLine("    Max pitch                    : {0} bytes", MemoryCaps.MaxPitch);
            writer.WriteLine("    Texture alignment            : {0} bytes", MemoryCaps.TextureAlignment);
            writer.WriteLine("    Max texture 1d (W)           : {0}", MemoryCaps.MaxTexture1dWidth);
            writer.WriteLine("    Max texture 2d (W x H)       : {0} x {1}", MemoryCaps.MaxTexture2dWidth, MemoryCaps.MaxTexture2dHeight);
            writer.WriteLine("    Max texture 3d (W x H x D)   : {0} x {1} x {2}", MemoryCaps.MaxTexture3dWidth, MemoryCaps.MaxTexture3dHeight, MemoryCaps.MaxTexture3dDepth);
            writer.WriteLine("    Max texture 2d array (W x H) : {0} x {1}", MemoryCaps.MaxTexture2dArrayWidth, MemoryCaps.MaxTexture2dHeight);
            writer.WriteLine("    Max texture 2d array splices : {0}", MemoryCaps.MaxTexture2dArrayNumSlices);
            writer.WriteLine("    Surface alignment            : {0} bytes", MemoryCaps.SurfaceAlignment);
            writer.WriteLine("    Compute mode                 : {0}", ComputeMode);
            writer.WriteLine("    Is integrated                : {0}", IsIntegrated);
            writer.WriteLine("    Is kernel timeout enabled    : {0}", IsKernelTimeoutEnabled);
            writer.WriteLine("    Supports host memory mapping : {0}", SupportsHostMemoryMapping);
            writer.WriteLine("    Supports GPU overlap         : {0}", SupportsGpuOverlap);
            writer.WriteLine("    Supports concurrent kernels  : {0}", SupportsConcurrentKernels);
            writer.WriteLine("    Is ECC enabled               : {0}", IsEccEnabled);
        }
    }
}