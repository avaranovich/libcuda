#ifndef _MATMUL_KERNEL_H_
#define _MATMUL_KERNEL_H_

#include <stdio.h>
#include "matrix.h"
#include "math.h"

////////////////////////UTILITY FUNCTIONS////////////////////////

__device__ float mat_get(Matrix& m, int y, int x)
{
    return m.elements[y * m.width + x];
}

__device__ void mat_set(Matrix& m, int y, int x, float value)
{
    m.elements[y * m.width + x] = value;
}

////////////////////GPU-BASED IMPLEMENTATION//////////////////////

// Forward declaration of the matrix multiplication kernel
__global__ void MatMulKernel(const Matrix, const Matrix, Matrix);

// Matrix multiplication - Host code
// Matrix dimensions are assumed to be multiples of BLOCK_SIZE
void MatMul(const Matrix A, const Matrix B, Matrix C)
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
    dim3 dimBlock(min(B.width, 16), min(A.height, 16));
    dim3 dimGrid((int)ceil(1.0 * B.width / dimBlock.x), (int)ceil(1.0 * A.height / dimBlock.y));
    MatMulKernel<<<dimGrid, dimBlock>>>(d_A, d_B, d_C);

    // Read C from device memory
    cudaMemcpy(C.elements, d_C.elements, size, cudaMemcpyDeviceToHost);
    
    // Free device memory
    cudaFree(d_A.elements);
    cudaFree(d_B.elements);
}

// Matrix multiplication kernel called by MatrixMul()
// also see comments to Playground.Conflux\SampleKernels\MatMulKernel.cs
__global__ void MatMulKernel(Matrix A, Matrix B, Matrix C)
{
    int row = blockIdx.y * blockDim.y + threadIdx.y;
    int col = blockIdx.x * blockDim.x + threadIdx.x;
    if (A.height <= row || B.width <= col) return;
    
    float Cvalue = 0;
    for (int dim = 0; dim < A.width; ++dim)
    {
        Cvalue += mat_get(A, row, dim) * mat_get(B, dim, col);
    }
    
    mat_set(C, row, col, Cvalue);
}

#endif