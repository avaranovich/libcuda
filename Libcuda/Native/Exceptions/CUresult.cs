namespace Libcuda.Native.Exceptions
{
    public enum CUresult
    {
        Success = 0,
        ErrorInvalidValue = 1,
        ErrorOutOfMemory = 2,
        ErrorNotInitialized = 3,
        ErrorDeinitialized = 4,

        ErrorNoDevice = 100,
        ErrorInvalidDevice = 101,

        ErrorInvalidImage = 200,
        ErrorInvalidContext = 201,
        ErrorContextAlreadyCurrent = 202,
        ErrorMapFailed = 205,
        ErrorUnmapFailed = 206,
        ErrorArrayIsMapped = 207,
        ErrorAlreadyMapped = 208,
        ErrorNoBinaryForGPU = 209,
        ErrorAlreadyAcquired = 210,
        ErrorNotMapped = 211,
        ErrorNotMappedAsArray = 212,
        ErrorNotMappedAsPointer = 213,
        ErrorEccUncorrectable = 214,
        ErrorUnsupportedLimit = 214,

        ErrorInvalidSource = 300,
        ErrorFileNotFound = 301,
        SharedObjectSymbolNotFound = 302,
        SharedObjectInitFailed = 303,

        ErrorInvalidHandle = 400,

        ErrorNotFound = 500,

        ErrorNotReady = 600,

        ErrorLaunchFailed = 700,
        ErrorLaunchOutOfResources = 701,
        ErrorLaunchTimeout = 702,
        ErrorLaunchIncompatibleTexturing = 703,

        ErrorPointerIs64Bit = 800,
        ErrorSizeIs64Bit = 801,

        ErrorUnknown = 999,
    }
}