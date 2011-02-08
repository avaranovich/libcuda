#include <stdio.h>
#include "matrix.h"

#define HEIGHT_OF_A 28
#define WIDTH_OF_A 190

#define HEIGHT_OF_B 190
#define WIDTH_OF_B 18

int main(int argc, char** args)
{
    Matrix a; 
    mat_init(a, HEIGHT_OF_A, WIDTH_OF_A);

    Matrix b; 
    mat_init(b, HEIGHT_OF_B, WIDTH_OF_B);

    Matrix c; 
    mat_init(c, HEIGHT_OF_A, WIDTH_OF_B);

    mat_print(a, "The matrix A:\n");
    mat_print(b, "The matrix B:\n");
    printf("APPLYING KERNEL...\n\n");
    MatMul(a, b, c);
//    MatMul_Fast(a, b, c);
    mat_print(c, "The result:\n");

    mat_free(a);
    mat_free(b);
    mat_free(c);
}