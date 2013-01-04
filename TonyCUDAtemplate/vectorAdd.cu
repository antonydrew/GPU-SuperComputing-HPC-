/**
 * Copyright 1993-2012 NVIDIA Corporation.  All rights reserved.
 *
 * Please refer to the NVIDIA end user license agreement (EULA) associated
 * with this source code for terms and conditions that govern your use of
 * this software. Any use, reproduction, disclosure, or distribution of
 * this software and related documentation outside the terms of the EULA
 * is strictly prohibited.
 *
 */

/**
 * Vector addition: C = A + B.
 *
 * This sample is a very basic sample that implements element by element
 * vector addition. It is the same as the sample illustrating Chapter 2
 * of the programming guide with some additions like error checking.
 */



#include <stdio.h>

// For the CUDA runtime routines (prefixed with "cuda_")
#include <cuda_runtime.h>

/**
 * CUDA Kernel Device code
 *
 * Computes the vector addition of A and B into C. The 3 vectors have the same
 * number of elements numElements.
 */
__global__ void kernelTest(int* i, int length){

    unsigned int tid = blockIdx.x*blockDim.x + threadIdx.x;

    if(tid < length)
        i[tid] = i[tid] + 10;
}

/*
 * This is the main routine which declares and initializes the integer vector, moves it to the device, launches kernel
 * brings the result vector back to host and dumps it on the console.
 */
int main(){

    int length  = 100;
    int* i = (int*)malloc(length*sizeof(int));

    for(int x=0;x<length;x++)
        i[x] = x;

    int* i_d;
    cudaMalloc((void**)&i_d,length*sizeof(int));

    cudaMemcpy(i_d, i, length*sizeof(int), cudaMemcpyHostToDevice);

    dim3 threads; threads.x = 256;
    dim3 blocks; blocks.x = (length/threads.x) + 1;

    kernelTest<<<threads,blocks>>>(i_d,length);
	

    cudaMemcpy(i, i_d, length*sizeof(int), cudaMemcpyDeviceToHost);

    for(int x=0;x<length;x++)
        printf("%d\t",i[x]);

	system("pause");


}