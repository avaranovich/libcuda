Provides a managed interface to certain functionality of nvcuda driver.

Implemented functionality:
 * Versioning of driver, software and hardware ISAs.
 * Acquisition of device spec and capabilities.
 * JIT compilation of PTX kernels.
 * Launching precompiled or JIT-compiled functions.
 * Low-level p/invoke signatures in the nvcuda static class.
 * Robust error reporting and propagation.

Quick facts:
 * Libcuda is built against nvcuda 3.1 (namely, against NVIDIA Compatible CUDA Driver, Version 257.21).
 * Libcuda was tested at the box with Windows XP Professional 32-bit.
 * Libcuda is strictly single-threaded: the thread that first accesses its functionality becomes its owner, other threads aren't permitted to use the library at all.
 * The latter restriction isn't imposed by CUDA itself - it might be lifted in future versions of the library.
 * Debug builds of Libcuda trace a lot - if you don't like that, logging can be dampen down or completely shut up by configuring the library.
 
