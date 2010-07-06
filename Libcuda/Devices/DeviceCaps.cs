using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using Libcuda.Native;
using Libcuda.Native.DataTypes;
using Libcuda.Native.Exceptions;
using Libcuda.Versions;
using XenoGears.Assertions;
using XenoGears.Traits.Dumpable;

namespace Libcuda.Devices
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
            var h_device = device.Handle;

            int cc_major, cc_minor;
            var error1 = nvcuda.cuDeviceComputeCapability(out cc_major, out cc_minor, h_device);
            if (error1 != CUresult.Success) throw new CudaException(error1);
            ComputeCaps = (HardwareIsa)(cc_major * 10 + cc_minor);
            FloatCaps = ((int)ComputeCaps >= 13) ? FloatCaps.NativeDoubles : FloatCaps.EmulateDoubles;
            GridCaps = new GridCaps(device);
            MemoryCaps = new MemoryCaps(device);

            int computeMode;
            var error2 = nvcuda.cuDeviceGetAttribute(out computeMode, CUdevice_attribute.ComputeMode, h_device);
            if (error2 != CUresult.Success) throw new CudaException(error2);
            ComputeMode = (ComputeMode)computeMode;

            int isKernelTimeoutEnabled;
            var error3 = nvcuda.cuDeviceGetAttribute(out isKernelTimeoutEnabled, CUdevice_attribute.KernelExecTimeout, h_device);
            if (error3 != CUresult.Success) throw new CudaException(error3);
            IsKernelTimeoutEnabled = isKernelTimeoutEnabled == 0 ? false : isKernelTimeoutEnabled == 1 ? true : ((Func<bool>)(() => { throw AssertionHelper.Fail(); }))();

            int isIntegrated;
            var error4 = nvcuda.cuDeviceGetAttribute(out isIntegrated, CUdevice_attribute.Integrated, h_device);
            if (error4 != CUresult.Success) throw new CudaException(error4);
            IsIntegrated = isIntegrated == 0 ? false : isIntegrated == 1 ? true : ((Func<bool>)(() => { throw AssertionHelper.Fail(); }))();

            int supportsHostMemoryMapping;
            var error5 = nvcuda.cuDeviceGetAttribute(out supportsHostMemoryMapping, CUdevice_attribute.CanMapHostMemory, h_device);
            if (error5 != CUresult.Success) throw new CudaException(error5);
            SupportsHostMemoryMapping = supportsHostMemoryMapping == 0 ? false : supportsHostMemoryMapping == 1 ? true : ((Func<bool>)(() => { throw AssertionHelper.Fail(); }))();

            int supportsGpuOverlap;
            var error6 = nvcuda.cuDeviceGetAttribute(out supportsGpuOverlap, CUdevice_attribute.GPUOverlap, h_device);
            if (error6 != CUresult.Success) throw new CudaException(error6);
            SupportsGpuOverlap = supportsGpuOverlap == 0 ? false : supportsGpuOverlap == 1 ? true : ((Func<bool>)(() => { throw AssertionHelper.Fail(); }))();

            int supportsConcurrentKernels;
            var error7 = nvcuda.cuDeviceGetAttribute(out supportsConcurrentKernels, CUdevice_attribute.ConcurrentKernels, h_device);
            if (error7 != CUresult.Success) throw new CudaException(error7);
            SupportsConcurrentKernels = supportsConcurrentKernels == 0 ? false : supportsConcurrentKernels == 1 ? true : ((Func<bool>)(() => { throw AssertionHelper.Fail(); }))();

            int isEccEnabled;
            var error8 = nvcuda.cuDeviceGetAttribute(out isEccEnabled, CUdevice_attribute.EccEnabled, h_device);
            if (error8 != CUresult.Success) throw new CudaException(error8);
            IsEccEnabled = isEccEnabled == 0 ? false : isEccEnabled == 1 ? true : ((Func<bool>)(() => { throw AssertionHelper.Fail(); }))();
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