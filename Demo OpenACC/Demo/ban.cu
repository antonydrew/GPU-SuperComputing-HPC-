#include <stdio.h>
#include <math.h>
#include <io.h>
#include <stdlib.h>
#include <string.h> 
#include <float.h>
#include "utility.h"
#include "utility.c"
//#include "Trade.cuh"
#include <time.h>
#include <malloc.h>
#include <cuda_runtime.h>
#include <cuda_runtime_api.h>
//#include <unistd.h>
#include <omp.h>
//#include "helper_cuda.h"
//#include "helper_functions.h"

#pragma warning( disable : 4996 )

int main () {

  // banner
  printf ("\n\n     Coding Exercise 3\n");
  printf (    "     ===============================\n");
  printf (  "\n     Matrix-Matrix Multiplication\n");
  printf (    "     PGI / OpenACC acceleration \n");

  // define parameters 
  int n = 1024;  // matrix dimension
  
  // allocate arrays
  double *a = (double *) malloc ( n*n*sizeof(double) );
  double *b = (double *) malloc ( n*n*sizeof(double) );
  double *c = (double *) malloc ( n*n*sizeof(double) );
  
  // initialize data
  for ( int row = 0; row<n; row++ ) {
    for ( int col = 0; col<n; col++ ) {
      // data is in row-major format
      a[row*n+col] = sin( 0.01*col ) + cos( 0.013*row );
      b[row*n+col] = sin( 0.017*col ) + cos( 0.03*row );
    }
  }

//#pragma acc data copy( c[0:n*n] )
#pragma acc kernels loop copy( c[0:n*n] )
  for ( int i = 0; i<n*n; i++ ) {
      c[i] = 0.0;
  }

  // record start time - use cuda events, accurate
  double t_start = omp_get_wtime();

#pragma acc data copyin(a[0:n*n],b[0:n*n]) copyout(c[0:n*n])
{
  // PERFORM MULTIPLICATION

  // loop over output rows
  #pragma acc kernels
  {

  #pragma acc loop independent
  for ( int row=0; row<n; row++ ) {

    // loop over output columns
    #pragma acc loop independent
    for ( int col=0; col<n; col++ ) {

      // initialize output result to zero
      double val = 0;

      // loop over inner dimension
      #pragma acc loop independent
      for ( int k=0; k<n; k++ ) {
        // sum
	val += a[row*n+k] * b[k*n+col];
	
      }
      c[row*n+col] = val;
	  //printf("%4.4f\n", val);
    }
  }
  }
}

  // compute elapsed time
  double et = omp_get_wtime() - t_start;

  // report results
  printf("\n     reference (768,768) = %4.4f \n", c[768*n+768]);
  printf(  "     elapsedTime         = %4.4f seconds\n", et);  // cudaEventElapsedTime is in milliseconds
  printf(  "     gigaflops achieved  = %4.4f Gflops/s\n\n\n", 2.0e-9*n*n*n/et); // 2( * and + ) *n (inner dimension)*n^2(result size)/(time in s.)

  system("pause");

}