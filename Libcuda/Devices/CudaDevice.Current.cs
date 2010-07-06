namespace Libcuda.Devices
{
    public partial class CudaDevice
    {
        public static CudaDevice Current
        {
            get
            {
                return CudaDevices.Current;
            }
        }

        public static CudaDevice First
        {
            get
            {
                return CudaDevices.First;
            }
        }

        public static CudaDevice Second
        {
            get
            {
                return CudaDevices.Second;
            }
        }
    }
}