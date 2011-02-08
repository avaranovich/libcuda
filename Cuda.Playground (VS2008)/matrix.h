#ifndef _MATRIX_H_
#define _MATRIX_H_

// Matrices are stored in row-major order:
// M(row, col) = *(M.elements + row * M.width + col)
typedef struct {
    int width;
    int height;
    float* elements;
} Matrix;

void mat_init(Matrix& m, int height, int width);
void mat_fill(Matrix& m);
void mat_print(Matrix& m, char* headline);
void mat_free(Matrix& m);

// Forward declaration of the matrix multiplication API
void MatMul(const Matrix, const Matrix, Matrix);
void MatMul_Fast(const Matrix, const Matrix, Matrix);

#endif