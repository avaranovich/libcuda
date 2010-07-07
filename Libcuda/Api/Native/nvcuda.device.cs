using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using Libcuda.Api.Native.DataTypes;
using Libcuda.Exceptions;
using Libcuda.Versions;

namespace Libcuda.Api.Native
{
    public static partial class nvcuda
    {
        [DllImport("nvcuda")]
        // http://developer.download.nvidia.com/compute/cuda/3_1/toolkit/docs/online/group__CUDEVICE_g09377966b78423897495d7ee6816ab17.html
        private static extern CUresult nativeDeviceGetCount(out int count);

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static int cuDeviceGetCount()
        {
            try
            {
                int count;
                var error = nativeDeviceGetCount(out count);
                if (error != CUresult.CUDA_SUCCESS) throw new CudaException(error);
                return count;
            }
            catch (CudaException)
            {
                throw;
            }
            catch (DllNotFoundException dnfe)
            {
                throw new CudaException(CudaError.NoDriver, dnfe);
            }
            catch (Exception e)
            {
                throw new CudaException(CudaError.Unknown, e);
            }
        }

        [DllImport("nvcuda")]
        // http://developer.download.nvidia.com/compute/cuda/3_1/toolkit/docs/online/group__CUDEVICE_g0864f76e0c101311b2e1eb5c40a54ea9.html
        private static extern CUresult nativeDeviceGet(out CUdevice device, int ordinal);

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static CUdevice cuDeviceGet(int ordinal)
        {
            try
            {
                CUdevice device;
                var error = nativeDeviceGet(out device, ordinal);
                if (error != CUresult.CUDA_SUCCESS) throw new CudaException(error);
                return device;
            }
            catch (CudaException)
            {
                throw;
            }
            catch (DllNotFoundException dnfe)
            {
                throw new CudaException(CudaError.NoDriver, dnfe);
            }
            catch (Exception e)
            {
                throw new CudaException(CudaError.Unknown, e);
            }
        }

        [DllImport("nvcuda", CharSet = CharSet.Ansi)]
        // http://developer.download.nvidia.com/compute/cuda/3_1/toolkit/docs/online/group__CUDEVICE_gea189b5fe198ceb909f2a8bc3188e36f.html
        private static extern CUresult nativeDeviceGetName(StringBuilder name, int len, CUdevice dev);

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static String cuDeviceGetName(CUdevice dev)
        {
            try
            {
                var deviceName_buf = new StringBuilder { Capacity = 255 };
                var error = nativeDeviceGetName(deviceName_buf, deviceName_buf.Capacity, dev);
                if (error != CUresult.CUDA_SUCCESS) throw new CudaException(error);
                return deviceName_buf.ToString();
            }
            catch (CudaException)
            {
                throw;
            }
            catch (DllNotFoundException dnfe)
            {
                throw new CudaException(CudaError.NoDriver, dnfe);
            }
            catch (Exception e)
            {
                throw new CudaException(CudaError.Unknown, e);
            }
        }

        [DllImport("nvcuda")]
        // http://developer.download.nvidia.com/compute/cuda/3_1/toolkit/docs/online/group__CUDEVICE_g2f36c6412efa2b6b89feefa233fb7519.html
        private static extern CUresult cuDeviceComputeCapability(out int major, out int minor, CUdevice dev);

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static HardwareIsa cuDeviceComputeCapability(CUdevice dev)
        {
            try
            {
                int major, minor;
                var error = cuDeviceComputeCapability(out major, out minor, dev);
                if (error != CUresult.CUDA_SUCCESS) throw new CudaException(error);
                return (HardwareIsa)(major * 10 + minor);
            }
            catch (CudaException)
            {
                throw;
            }
            catch (DllNotFoundException dnfe)
            {
                throw new CudaException(CudaError.NoDriver, dnfe);
            }
            catch (Exception e)
            {
                throw new CudaException(CudaError.Unknown, e);
            }
        }

        [DllImport("nvcuda")]
        // http://developer.download.nvidia.com/compute/cuda/3_1/toolkit/docs/online/group__CUDEVICE_ge283d0251a80fe5a82ec8f6e552eb248.html
        private static extern CUresult nativeDeviceGetAttribute(out int pi, CUdevice_attribute attrib, CUdevice dev);

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static int cuDeviceGetAttribute(CUdevice_attribute attrib, CUdevice dev)
        {
            try
            {
                int i;
                var error = nativeDeviceGetAttribute(out i, attrib, dev);
                if (error != CUresult.CUDA_SUCCESS) throw new CudaException(error);
                return i;
            }
            catch (CudaException)
            {
                throw;
            }
            catch (DllNotFoundException dnfe)
            {
                throw new CudaException(CudaError.NoDriver, dnfe);
            }
            catch (Exception e)
            {
                throw new CudaException(CudaError.Unknown, e);
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static bool cuDeviceGetFlag(CUdevice_attribute attrib, CUdevice dev)
        {
            var value = cuDeviceGetAttribute(attrib, dev);
            if (value == 0)
            {
                return false;
            }
            else if (value == 1)
            {
                return true;
            }
            else
            {
                var fex = new FormatException(String.Format("Attribute \"{0}\" has value \"{1}\" which isn't convertible to bool.", attrib, value));
                throw new CudaException(CudaError.InvalidValue, fex);
            }
        }

        [DllImport("nvcuda")]
        // http://developer.download.nvidia.com/compute/cuda/3_1/toolkit/docs/online/group__CUDEVICE_g7ff0e9bbf5268a053a77e7063b8a6bec.html
        private static extern CUresult nativeDeviceTotalMem(out uint bytes, CUdevice dev);

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static uint cuDeviceTotalMem(CUdevice dev)
        {
            try
            {
                uint bytes;
                var error = nativeDeviceTotalMem(out bytes, dev);
                if (error != CUresult.CUDA_SUCCESS) throw new CudaException(error);
                return bytes;
            }
            catch (CudaException)
            {
                throw;
            }
            catch (DllNotFoundException dnfe)
            {
                throw new CudaException(CudaError.NoDriver, dnfe);
            }
            catch (Exception e)
            {
                throw new CudaException(CudaError.Unknown, e);
            }
        }
    }
}
