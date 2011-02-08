Prerequisites:
1. devdriver_3.2_winvista-win7_64_263.06_general.exe
2. Parallel_Nsight_Host_Win64_1.51.11018.msi
3. Neither toolkit, nor SDK were installed (the prior is included in Nsight, the latter ain't necessary for bulding CUDA apps)

Set up the environment (if everything is installed correctly, you won't need to do this - just verify):
1. Set CUDA_PATH_V3_2 environment variable to the root of CUDA toolkit installation (don't forget the trailing slash).
2. Copy NvCuda*.rules files to $(VS Installation Directory)\VC\VCProjectDefaults\.

How to create a new project (credit for this guide goes to stackoverflow):
1. Create a new project using the standard MS wizards (e.g. an empty console project)
2. Implement your host (serial) code in .c or .cpp files
3. Implement your wrappers and kernels in .cu files
4. Add the NvCudaRuntimeApi.rules (right click on the project, Custom Build Rules, tick the relevant box), see note [1]
5. Add the CUDA runtime library (right click on the project and choose Properties, then in Linker -> General add $(CUDA_PATH)\lib\$(PlatformName) to the Additional Library Directories and in Linker -> Input add cudart.lib to the Additional Dependencies), see notes [2] and [3]
6. Optionally add the CUDA include files to the search path, required if you include any CUDA files in your .cpp files (as opposed to .cu files) (right click on the project and choose Properties, then in C/C++ -> General add $(CUDA_PATH)\include to the Additional Include Directories), see note [3]
7. Then just build your project and the .cu files will be compiled to .obj and added to the link automatically

Some other tips:
1. Change the code generation to use statically loaded C runtime to match the CUDA runtime; right click on the project and choose Properties, then in C/C++ -> Code Generation change the Runtime Library to /MT (or /MTd for debug, in which case you will need to mirror this in Runtime API -> Host -> Runtime Library), see note [4]
2. Enable syntax highlighting using the usertype.dat file included with the SDK, see the readme.txt in <sdk_install_dir>\C\doc\syntax_highlighting\visual_studio_8

Notes
1. You can also use a Toolkit-version-specific rules fule e.g. NvCudaRuntimeApi.v3.2.rules. This means that instead of looking for the CUDA Toolkit in %CUDA_PATH% it will look in %CUDA_PATH_V3_2%, which in turn means that you can have multiple versions of the CUDA Toolkit installed on your system and different projects can target different versions. See also note [3].
2. The rules file cannot modify the C/C++ compilation and linker settings, since it is simply adding compilation settings for the CUDA code. Therefore you need to do this step manually. Remember to do it for all configurations!
3. If you want to stabilise on a specific CUDA Toolkit version then you should replace CUDA_PATH with CUDA_PATH_V3_2. See also note [1].
4. Having mismatched version of the C runtime can cause a variety of problems; in particular if you have any errors regarding LIBCMT (e.g. LNK4098: defaultlib 'LIBCMT' conflicts with use of other libs) or multiply defined symbols for standard library functions, then this should be your first suspect.