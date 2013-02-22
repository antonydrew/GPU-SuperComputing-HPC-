#include <stdio.h>
// For the CUDA runtime routines (prefixed with "cuda_")
#include <cuda_runtime.h>
#include <cuda_runtime_api.h>

//CUDA Kernel Device code - Computes the vector addition of 10 to each iteration i
__global__ void kernelTest(int* i, int length){

    unsigned int tid = blockIdx.x*blockDim.x + threadIdx.x;

    if(tid < length)
        i[tid] = i[tid] + 10; }

 /* This is the main routine which declares and initializes the integer vector, moves it to the device, launches kernel
 * brings the result vector back to host and dumps it on the console. */
int main(void){

	// Error code to check return values for CUDA calls
    cudaError_t err = cudaSuccess;

	int cumsum[200]={0},x=0;
    int length  = 100;
	printf("[Vector multiplication of %d elements]\n", length);

	// Allocate the host input vector A
    int* i = (int*)malloc(length*sizeof(int));

    for(int x=0;x<length;x++)
        i[x] = x;

	 // Allocate the device input vector 
    int* i_d;
    err=cudaMalloc((void**)&i_d,length*sizeof(int));

	if (err != cudaSuccess)
    {
        fprintf(stderr, "Failed to allocate device matrix  (error code %s)!\n", cudaGetErrorString(err));
        exit(EXIT_FAILURE); }

	// Copy the host input vectors A and B in host memory to the device input vectors in
	printf("Copy input data from the host memory to the CUDA device\n");
    err = cudaMemcpy(i_d, i, length*sizeof(int), cudaMemcpyHostToDevice);

	if (err != cudaSuccess)
    {
        fprintf(stderr, "Failed to copy matrix from host to device (error code %s)!\n", cudaGetErrorString(err));
        exit(EXIT_FAILURE); }

	// Launch the Vector Add CUDA Kernel
    dim3 threads; threads.x = 256;
    dim3 blocks; blocks.x = (length/threads.x) + 1;
    kernelTest<<<threads,blocks>>>(i_d,length);

	err = cudaGetLastError();

    if (err != cudaSuccess)
    {
        fprintf(stderr, "Failed to launch vectorMultiply kernel (error code %s)!\n", cudaGetErrorString(err));
        exit(EXIT_FAILURE); }

	 // Copy the device result vector in device memory to the host result vector
	printf("Copy output data from the CUDA device to the host memory\n");
    err = cudaMemcpy(i, i_d, length*sizeof(int), cudaMemcpyDeviceToHost);

	 if (err != cudaSuccess)
    {
        fprintf(stderr, "Failed to copy matrix from device to host (error code %s)!\n", cudaGetErrorString(err));
        exit(EXIT_FAILURE); }

    for(int x=0;x<length;x++)
        printf("%d\t",i[x]);

	// Verify that the result vector is correct
    for (int x = 1; x <= length; ++x)
    {     
		cumsum[x] = cumsum[x-1]+i[x]; }

	if (cumsum[length-1]+i[0] != 5950)
        {
            fprintf(stderr,"Result verification failed at element %i!\n", cumsum[length-1]);
            exit(EXIT_FAILURE); }
	
	// Free host and device memory
    free(i); cudaFree(i_d);
  
    // Reset the device and exit
    err = cudaDeviceReset();

    if (err != cudaSuccess)
    {
        fprintf(stderr, "Failed to deinitialize the device! error=%s\n", cudaGetErrorString(err));
        exit(EXIT_FAILURE); }

    printf("Done\n");
	return; }