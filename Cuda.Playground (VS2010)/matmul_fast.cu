#ifndef _MATMUL_KERNEL_FAST_H_
#define _MATMUL_KERNEL_FAST_H_

#include <stdio.h>
#include "matrix.h"

////////////////////GPU-BASED IMPLEMENTATION//////////////////////

// Forward declaration of the matrix multiplication kernel
__global__ void MatMulKernel_Fast(const Matrix, const Matrix, Matrix);

// Matrix multiplication - Host code
// Matrix dimensions are assumed to be multiples of BLOCK_SIZE
void MatMul_Fast(const Matrix A, const Matrix B, Matrix C)
{
    // Load A to device memory
    Matrix d_A;
    d_A.width = A.width; d_A.height = A.height;
    size_t size = A.width * A.height * sizeof(float);
    cudaMalloc((void**)&d_A.elements, size);
    cudaMemcpy(d_A.elements, A.elements, size, cudaMemcpyHostToDevice);

    // Load B to device memory
    Matrix d_B;
    d_B.width = B.width; d_B.height = B.height;
    size = B.width * B.height * sizeof(float);
    cudaMalloc((void**)&d_B.elements, size);
    cudaMemcpy(d_B.elements, B.elements, size, cudaMemcpyHostToDevice);

    // Allocate C in device memory
    Matrix d_C;
    d_C.width = C.width; d_C.height = C.height;
    size = C.width * C.height * sizeof(float);
    cudaMalloc((void**)&d_C.elements, size);

    // Invoke kernel
    int mindim = min(min(B.width, 16), min(A.height, 16));
    dim3 dimBlock(mindim, mindim);
    dim3 dimGrid((int)ceil(1.0 * B.width / dimBlock.x), (int)ceil(1.0 * A.height / dimBlock.y));
    MatMulKernel_Fast<<<dimGrid, dimBlock>>>(d_A, d_B, d_C);

    // Read C from device memory
    cudaMemcpy(C.elements, d_C.elements, size, cudaMemcpyDeviceToHost);
    
    // Free device memory
    cudaFree(d_A.elements);
    cudaFree(d_B.elements);
}

////////////////////FRAGMENT OF THE MATRIX////////////////////////

// Matrices are stored in row-major order:
// M(row, col) = *(M.elements + row * M.stride + col)
typedef struct {
    int top;
    int left;
    int height;
    int width;
    int stride;
    float* elements;
} SubMatrix;

__device__ SubMatrix sub_init(Matrix m, int blockRow, int blockCol)
{
    SubMatrix msub;
    msub.top = blockRow * blockDim.y;
    msub.left = blockCol * blockDim.x;
    msub.height = min(blockDim.y, m.height - msub.top);
    msub.width = min(blockDim.x, m.width - msub.left);
    msub.stride = m.width;
    msub.elements = m.elements;
    return msub;
}

__device__ float sub_get(const SubMatrix msub, int row, int col)
{
    return msub.elements[(msub.top + row) * msub.stride + (msub.left + col)];
}

__device__ void sub_set(SubMatrix msub, int row, int col, float value)
{
    msub.elements[(msub.top + row) * msub.stride + (msub.left + col)] = value;
}

////////////////////GPU-BASED IMPLEMENTATION//////////////////////

// Matrix multiplication kernel called by MatMul_Fast()
// also see comments to Playground.Conflux\SampleKernels\MatMulKernel_Fast.cs
__global__ void MatMulKernel_Fast(Matrix A, Matrix B, Matrix C)
{
    int blockSize = blockDim.x;
    
    float c_value = 0;
    for (int i = 0; i < (int)ceil(1.0 * A.width / blockSize); ++i) 
    {
        __shared__ float asub_shared[16][16];
        __shared__ float bsub_shared[16][16];

        SubMatrix asub = sub_init(A, blockIdx.y, i);
        if (asub.height > threadIdx.y && asub.width > threadIdx.x)
            asub_shared[threadIdx.y][threadIdx.x] = sub_get(asub, threadIdx.y, threadIdx.x);

        SubMatrix bsub = sub_init(B, i, blockIdx.x);
        if (bsub.height > threadIdx.y && bsub.width > threadIdx.x)
            bsub_shared[threadIdx.y][threadIdx.x] = sub_get(bsub, threadIdx.y, threadIdx.x);

        __syncthreads();

        int stripLen = min(A.width - i * blockSize, blockSize);
        for (int j = 0; j < stripLen; ++j)
            c_value += asub_shared[threadIdx.y][j] * bsub_shared[j][threadIdx.x];

        __syncthreads();
    }
    
    SubMatrix csub = sub_init(C, blockIdx.y, blockIdx.x);
    if (csub.height > threadIdx.y && csub.width > threadIdx.x)
        sub_set(csub, threadIdx.y, threadIdx.x, c_value);
}

#endif