#pragma warning( disable : 4996 )



////////////////////////////////////////////////////////////////////////////////
//Process an array of SIMULATIONS options on GPU
////////////////////////////////////////////////////////////////////////////////
__global__ void kernelSim(float *zscores_d,float *rets_d,float *pnl_d,float *pos_d,int start,int stop,double zcut){
    
	
	
	//float zcut = *a_d; 
	//int starty = *start_d;
	//int stoppy = *stop_d;
	//int opt = 0;
	int buy =1;
	int sell=-1;
	
	/*unsigned int tid = blockIdx.x*blockDim.x + threadIdx.x;

    if(tid < length)
        i[tid] = i[tid] + 10;*/
	//Thread index
    const int      tid = blockDim.x * blockIdx.x + threadIdx.x;
    //Total number of threads in execution grid
    const int THREAD_N = blockDim.x * gridDim.x;

    //No matter how small is execution grid or how large OptN is,
    //exactly OptN indices will be processed with perfect memory coalescing

	//for(int opt = tid+start_d[0]; opt < stop_d[0]; opt += THREAD_N){	

	for(int opt = tid+start; opt < stop; opt += THREAD_N){		
	//if(tid < stoppy){
		if(zscores_d[opt] > zcut) pos_d[opt] = buy;														
		if(zscores_d[opt] < -zcut) pos_d[opt] = sell;			
		pnl_d[opt] = (pos_d[opt] * rets_d[opt]); }

   
}
