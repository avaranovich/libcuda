Prerequisites:
1. devdriver_3.2_winvista-win7_64_263.06_general.exe
2. Parallel_Nsight_Host_Win64_1.51.11018.msi
3. Neither toolkit, nor SDK were installed (the prior is included in Nsight, the latter ain't necessary for bulding CUDA apps)

Set up the environment (if everything is installed correctly, you won't need to do this - just verify):
1. Set CUDA_PATH_V3_2 environment variable to the root of CUDA toolkit installation (don't forget the trailing slash).
2. Copy CUDA*.* files and Nvda.Build.CudaTasks.dll to MSBuild configuration directory (usually it resides at $(ProgramFiles)\MSBuild\Microsoft.Cpp\v4.0\BuildCustomizations\).

How to create a new project (credit for this guide goes to stackoverflow):
1. Take a look at VS2008 guide in the directory next to this one.
2. You need to do almost the same except few thingies mentioned in notes.

Notes:
1. You must set the General > Platform Toolset property to "vs90" (btw, it's configuration- and platform-specific, so be sure to select all platforms and all configurations).
2. There's no need to add $(CUDA_PATH)\lib\$(PlatformName) to Linker > General, since it's already there (I suspect that's because VS2010 allows custom build rules to adjust project configuration).
3. CUDA rule for VS2008 uses static linking to runtime, however, rule for VS2010 uses dynamic linking. Adjust C/C++ Compilation > Code Generation accordingly (i.e. so that it reads Multithreaded DLL or Multithreaded Debug DLL).
4. Unlike in VS2008, you'll need to manually add *.cu files to build workflow. Right-click the file, choose Properties and then change General > Item Type to "CUDA C/C++".