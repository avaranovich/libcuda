#include <stdlib.h>
#include <malloc.h>
#include <stdio.h>
#include <time.h>
#include "matrix.h"

void mat_init(Matrix& m, int height, int width)
{
    m.width = width; 
    m.height = height;
    size_t size = m.height * m.width * sizeof(float);
    
    m.elements = (float*)malloc(size);
    mat_fill(m);
}

void mat_fill(Matrix& m)
{
    srand((unsigned int)time(NULL));
    
    for(int row = 0; row < m.height; ++row)
    {
        for(int col = 0; col < m.width; ++col)
        {
            *(m.elements + row * m.width + col) = 1;
            // *(m.elements + row * m.width + col) = (float)(rand() % 2);
        }
    }
}

void mat_print(Matrix& m, char* headline)
{
    printf(headline);
    
    for(int row = 0; row < m.height; ++row)
    {
        for(int col = 0; col < m.width; ++col)
        {
            float element = *(m.elements + row * m.width + col);
            printf("%0.0f ", element);
        }
        
        printf("\n");
    }
    
    printf("\n");
}

void mat_free(Matrix& m)
{
    free(m.elements);
    m.elements = NULL;
}
