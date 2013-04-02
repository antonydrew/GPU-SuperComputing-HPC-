#include <stdio.h>

// For the CUDA runtime library/routines (prefixed with "cuda_") - must include this file
#include <cuda_runtime.h>

/* CUDA Kernel Device code
 * Computes the vector addition of 10 to each iteration i */
__global__ void kernelTest(int* i, int length){

    unsigned int tid = blockIdx.x*blockDim.x + threadIdx.x;

    if(tid < length)
        i[tid] = i[tid] + 10;}

/* This is the main routine which declares and initializes the integer vector, moves it to the device, launches kernel
 * brings the result vector back to host and dumps it on the console. */
int main(){
	
	//declare pointer and allocate memory for host CPU variable - must use MALLOC of CudaHostAlloc here
    int length  = 100;
    int* i = (int*)malloc(length*sizeof(int));

	//fill CPU variable with values from 1 to 100 via loop
    for(int x=0;x<length;x++)
        i[x] = x;

	//declare pointer and allocate memory for device GPU variable denoted with "_d"
    int* i_d;
    cudaMalloc((void**)&i_d,length*sizeof(int));

	//copy contents of host CPU variable over to GPU variable on GPU device
    cudaMemcpy(i_d, i, length*sizeof(int), cudaMemcpyHostToDevice);

	//designate how many threads and blocks to use on GPU for CUDA function call/calculation - this depends on each device
    dim3 threads; threads.x = 256;
    dim3 blocks; blocks.x = (length/threads.x) + 1;

	//call CUDA C function - note triple chevron here - this is CUDA syntax
    kernelTest<<<threads,blocks>>>(i_d,length);
	
	//wait for CUDA C function to finish and then copy results from GPU variable on device back over to CPU variable on host
    cudaMemcpy(i, i_d, length*sizeof(int), cudaMemcpyDeviceToHost);

	//print results of CPU variable to console
    for(int x=0;x<length;x++)
        printf("%d\t",i[x]);

	//free memory for both CPU and GPU variables/pointers
	free (i); cudaFree (i_d);

	//reset GPU device
	system("pause");
	cudaDeviceReset();  }
	