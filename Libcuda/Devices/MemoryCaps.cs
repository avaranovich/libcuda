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
            var h_device = device.Handle;

            int maxPitch;
            var error1 = nvcuda.cuDeviceGetAttribute(out maxPitch, CUdevice_attribute.MaxPitch, h_device);
            if (error1 != CUresult.Success) throw new CudaException(error1);
            MaxPitch = maxPitch;

            int textureAlignment;
            var error2 = nvcuda.cuDeviceGetAttribute(out textureAlignment, CUdevice_attribute.TextureAlignment, h_device);
            if (error2 != CUresult.Success) throw new CudaException(error2);
            TextureAlignment = textureAlignment;

            int maxTexture1dWidth;
            var error3 = nvcuda.cuDeviceGetAttribute(out maxTexture1dWidth, CUdevice_attribute.MaxTexture1dWidth, h_device);
            if (error3 != CUresult.Success) throw new CudaException(error3);
            MaxTexture1dWidth = maxTexture1dWidth;

            int maxTexture2dWidth;
            var error4 = nvcuda.cuDeviceGetAttribute(out maxTexture2dWidth, CUdevice_attribute.MaxTexture2dWidth, h_device);
            if (error4 != CUresult.Success) throw new CudaException(error4);
            MaxTexture2dWidth = maxTexture2dWidth;

            int maxTexture2dHeight;
            var error5 = nvcuda.cuDeviceGetAttribute(out maxTexture2dHeight, CUdevice_attribute.MaxTexture2dHeight, h_device);
            if (error5 != CUresult.Success) throw new CudaException(error5);
            MaxTexture2dHeight = maxTexture2dHeight;

            int maxTexture3dWidth;
            var error6 = nvcuda.cuDeviceGetAttribute(out maxTexture3dWidth, CUdevice_attribute.MaxTexture3dWidth, h_device);
            if (error6 != CUresult.Success) throw new CudaException(error6);
            MaxTexture3dWidth = maxTexture3dWidth;

            int maxTexture3dHeight;
            var error7 = nvcuda.cuDeviceGetAttribute(out maxTexture3dHeight, CUdevice_attribute.MaxTexture3dHeight, h_device);
            if (error7 != CUresult.Success) throw new CudaException(error7);
            MaxTexture3dHeight = maxTexture3dHeight;

            int maxTexture3dDepth;
            var error8 = nvcuda.cuDeviceGetAttribute(out maxTexture3dDepth, CUdevice_attribute.MaxTexture3dDepth, h_device);
            if (error8 != CUresult.Success) throw new CudaException(error8);
            MaxTexture3dDepth = maxTexture3dDepth;

            int maxTexture2dArrayWidth;
            var error9 = nvcuda.cuDeviceGetAttribute(out maxTexture2dArrayWidth, CUdevice_attribute.MaxTexture2dArrayWidth, h_device);
            if (error9 != CUresult.Success) throw new CudaException(error9);
            MaxTexture2dArrayWidth = maxTexture2dArrayWidth;

            int maxTexture2dArrayHeight;
            var error10 = nvcuda.cuDeviceGetAttribute(out maxTexture2dArrayHeight, CUdevice_attribute.MaxTexture2dArrayHeight, h_device);
            if (error10 != CUresult.Success) throw new CudaException(error10);
            MaxTexture2dArrayHeight = maxTexture2dArrayHeight;

            int maxTexture2dArrayNumSplices;
            var error11 = nvcuda.cuDeviceGetAttribute(out maxTexture2dArrayNumSplices, CUdevice_attribute.MaxTexture2dArrayNumSlices, h_device);
            if (error11 != CUresult.Success) throw new CudaException(error11);
            MaxTexture2dArrayNumSlices = maxTexture2dArrayNumSplices;

            int surfaceAlignment;
            var error12 = nvcuda.cuDeviceGetAttribute(out surfaceAlignment, CUdevice_attribute.MaxTexture2dArrayNumSlices, h_device);
            if (error12 != CUresult.Success) throw new CudaException(error12);
            SurfaceAlignment = surfaceAlignment;
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