using System;
using System.Diagnostics;
using System.IO;
using Libcuda.Api.Native;
using Libcuda.Api.Native.DataTypes;
using XenoGears.Traits.Dumpable;

namespace Libcuda.Api.Devices
{
    [DebuggerNonUserCode]
    public class MemoryCaps : IDumpableAsText
    {
        public CudaDevice Device { get; private set; }

        public int MaxPitch { get; private set; }
        public int TextureAlignment { get; private set; }
        public int MaxTexture1dWidth { get; private set; }
        public int MaxTexture2dWidth { get; private set; }
        public int MaxTexture2dHeight { get; private set; }
        public int MaxTexture3dWidth { get; private set; }
        public int MaxTexture3dHeight { get; private set; }
        public int MaxTexture3dDepth { get; private set; }
        public int MaxTexture2dArrayWidth { get; private set; }
        public int MaxTexture2dArrayHeight { get; private set; }
        public int MaxTexture2dArrayNumSlices { get; private set; }
        public int SurfaceAlignment { get; private set; }

        internal MemoryCaps(CudaDevice device)
        {
            Device = device;

            MaxPitch = nvcuda.cuDeviceGetAttribute(CUdevice_attribute.MaxPitch, device);
            TextureAlignment = nvcuda.cuDeviceGetAttribute(CUdevice_attribute.TextureAlignment, device);
            MaxTexture1dWidth = nvcuda.cuDeviceGetAttribute(CUdevice_attribute.MaxTexture1dWidth, device);
            MaxTexture2dWidth = nvcuda.cuDeviceGetAttribute(CUdevice_attribute.MaxTexture2dWidth, device);
            MaxTexture2dHeight = nvcuda.cuDeviceGetAttribute(CUdevice_attribute.MaxTexture2dHeight, device);
            MaxTexture3dWidth = nvcuda.cuDeviceGetAttribute(CUdevice_attribute.MaxTexture3dWidth, device);
            MaxTexture3dHeight = nvcuda.cuDeviceGetAttribute(CUdevice_attribute.MaxTexture3dHeight, device);
            MaxTexture3dDepth = nvcuda.cuDeviceGetAttribute(CUdevice_attribute.MaxTexture3dDepth, device);
            MaxTexture2dArrayWidth = nvcuda.cuDeviceGetAttribute(CUdevice_attribute.MaxTexture2dArrayWidth, device);
            MaxTexture2dArrayHeight = nvcuda.cuDeviceGetAttribute(CUdevice_attribute.MaxTexture2dArrayHeight, device);
            MaxTexture2dArrayNumSlices = nvcuda.cuDeviceGetAttribute(CUdevice_attribute.MaxTexture2dArrayNumSlices, device);
            SurfaceAlignment = nvcuda.cuDeviceGetAttribute(CUdevice_attribute.MaxTexture2dArrayNumSlices, device);
        }

        public override String ToString() { return ((IDumpableAsText)this).DumpAsText(); }
        void IDumpableAsText.DumpAsText(TextWriter writer)
        {
            writer.WriteLine("Memory caps of device #{0} \"{1}\" (/pci:{2}/dev:{3})", Device.Index, Device.Name, Device.PciBusId, Device.PciDeviceId);
            writer.WriteLine("    Max pitch                    : {0} bytes", MaxPitch);
            writer.WriteLine("    Texture alignment            : {0} bytes", TextureAlignment);
            writer.WriteLine("    Max texture 1d (W)           : {0}", MaxTexture1dWidth);
            writer.WriteLine("    Max texture 2d (W x H)       : {0} x {1}", MaxTexture2dWidth, MaxTexture2dHeight);
            writer.WriteLine("    Max texture 3d (W x H x D)   : {0} x {1} x {2}", MaxTexture3dWidth, MaxTexture3dHeight, MaxTexture3dDepth);
            writer.WriteLine("    Max texture 2d array (W x H) : {0} x {1}", MaxTexture2dArrayWidth, MaxTexture2dHeight);
            writer.WriteLine("    Max texture 2d array splices : {0}", MaxTexture2dArrayNumSlices);
            writer.WriteLine("    Surface alignment            : {0} bytes", SurfaceAlignment);
        }
    }
}