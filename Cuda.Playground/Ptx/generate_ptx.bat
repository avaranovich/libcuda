@echo off

echo Compiling "%3" into PTX: bitness = %1, arch = %2

if "%CUDA_BIN_PATH%"=="" (
    echo [Fatal error] The CUDA_BIN_PATH environment variable is not defined. Possible reason: CUDA Toolkit isn't installed.
    @pause
    exit /B 1
) else (
    echo Using CUDA toolkit at %CUDA_BIN_PATH%
    
    set toolkit_is_64bit=%CUDA_BIN_PATH:~-2%
    if "%toolkit_is_64bit%"=="64" (
        if not "%1"=="64" (
            echo [Fatal error] Bitness of the CUDA toolkit (64^) doesn't match bitness of the compilation (%1^). You must install 32-bit version of CUDA toolkit to proceed.
            @pause
            exit /B 1
        )
    ) else (
        if not "%1"=="32" (
            echo [Fatal error] Bitness of the CUDA toolkit (32^) doesn't match bitness of the compilation (%1^). You must install 64-bit version of CUDA toolkit to proceed.
            @pause
            exit /B 1
        )
    )
)

if "%1"=="" (
    echo [Fatal error] Bitness of the compilation is not specified. Please, provide a single parameter to this batch file - either "32" or "64".
    exit /B 1
) else ( if "%1"=="32" (
    if exist "c:\Program Files\Microsoft Visual Studio 9.0\VC\bin\vcvars32.bat" (
        call "c:\Program Files\Microsoft Visual Studio 9.0\VC\bin\vcvars32.bat"
    ) else ( if exist "c:\Program Files (x86)\Microsoft Visual Studio 9.0\VC\bin\vcvars32.bat" (
        call "c:\Program Files (x86)\Microsoft Visual Studio 9.0\VC\bin\vcvars32.bat"
    ) else (
        echo [Fatal error] Cannot find the "vcvars32.bat" file. Possible reason: Microsoft Visual Studio isn't installed.
        @pause
        exit /B 1
    ))
) else ( if "%1"=="64" (
    if exist "c:\Program Files\Microsoft Visual Studio 9.0\VC\bin\amd64\vcvarsamd64.bat" (
        call "c:\Program Files\Microsoft Visual Studio 9.0\VC\bin\amd64\vcvarsamd64.bat"
    ) else ( if exist "c:\Program Files (x86)\Microsoft Visual Studio 9.0\VC\bin\amd64\vcvarsamd64.bat" (
        call "c:\Program Files (x86)\Microsoft Visual Studio 9.0\VC\bin\amd64\vcvarsamd64.bat"
    ) else (
        echo [Fatal error] Cannot find the "vcvarsamd64.bat" file. Possible reason: Microsoft Visual Studio isn't installed.
        @pause
        exit /B 1
    ))
) else (
    echo [Fatal error] Unsupported bitness of the compilation "%1". Only "32" or "64" are supported.
    @pause
    exit /B 1
)))

echo Launching the NVCC...
"%CUDA_BIN_PATH%\nvcc.exe" -ptx -arch %2 %3
echo.