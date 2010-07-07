namespace Libcuda.Exceptions
{
    public enum CudaError
    {
        InvalidGridDim = -700,
        InvalidBlockDim = -701,
        NoDriver = -1,

        Success = 0,
        InvalidValue = 1,
        OutOfMemory = 2,
        NotInitialized = 3,
        Deinitialized = 4,

        NoDevice = 100,
        InvalidDevice = 101,

        InvalidImage = 200,
        InvalidContext = 201,
        ContextAlreadyCurrent = 202,
        MapFailed = 205,
        UnmapFailed = 206,
        ArrayIsMapped = 207,
        AlreadyMapped = 208,
        NoBinaryForGPU = 209,
        AlreadyAcquired = 210,
        NotMapped = 211,
        NotMappedAsArray = 212,
        NotMappedAsPointer = 213,
        EccUncorrectable = 214,
        UnsupportedLimit = 214,

        InvalidSource = 300,
        FileNotFound = 301,
        SharedObjectSymbolNotFound = 302,
        SharedObjectInitFailed = 303,

        InvalidHandle = 400,

        NotFound = 500,

        NotReady = 600,

        LaunchFailed = 700,
        LaunchOutOfResources = 701,
        LaunchTimeout = 702,
        LaunchIncompatibleTexturing = 703,

        PointerIs64Bit = 800,
        SizeIs64Bit = 801,

        Unknown = 999,
    }
}