#include <stdio.h>
#include <math.h>
#include <stdlib.h>
#include <string.h>
#include <float.h>
#include <time.h>
#include <malloc.h>
#include <cuda_runtime.h>
#include <cuda_runtime_api.h>
//#include <sys/io.h>

//#pragma warning( disable : 4996 )

//Declaring macros and constants in pre-processor - STEP A//INSERT NEW STUFF DOWN HERE EACH TIME START SUB-FUNCTION******PRE-PROCESSOR AREA*******//
#define ABS(X) (X>=0?X:-X)
#define MAX(X,Y) (X>=Y?X:Y)
#define MIN(X,Y) (X<=Y?X:Y)
#define SIGN(X) (X>=0?(X==0?0:1):-1)
#define ROUND(X,Y) ((X>=0?(X<<1)+Y:(X<<1)-Y)/(Y<<1))*Y

//Change path below for UNIX "c://usr"
#define PATH "/home/tony/DemoCuMultiGPU/"
#define LOOKBACK 1597		// 1597-987-610-377-144-89 fibos rolling optimization historical period
#define STEP 377			// or 89 fibos step forward in time period for next rolling optimization
#define NUMI 27				//up to 27 number of markets
#define STR 2				//number of streams - use 2 to be safe since most GPU's can handle 2 streams if threads are low on each stream
#define THR 256				//initial threads - must specify this PARAM - can affect OCCUPANCY or %USE OF GPU

long f_line(FILE *f);
FILE * f_openr( char *filer);
FILE * f_openw( char *filer);
void f_close(char *filer,FILE *f);
void snf(double *op, double *lo, double *hi, long end, double *price, double *smooth, double *detrender, double *period, double *qu, double *iu, double *ji, double *jq, double *ib, double *qb, double *sib, double *sqb, double *re, double *im, double *sre, double *sim, double *speriod, double *smperiod, double *qc, double *ic, double *ric, int *intperiod, double *sig, double *nois, double *snr);
void zscore(int lens, double *op, double *sumv, double *varv, long end, float *zscores, double *stdevv, double *m_avev);
void ret(double *p, long end, float *rets);
void sharpe(float *pnl, double *sumi, double *vari, long end, double *stdevi, double *m_avei, double *sharp);
void sharpep(float *pnl, double *sumip, double *varip, int start, int stop, double *stdevip, double *m_aveip, double *sharpp);
__global__ void kernelSim(float *zscores_d,float *rets_d,float *pnl_d,float *pos_d,int start,int stop,float zcut, int lens);

//INT MAIN//INSERT NEW STUFF HERE EACH TIME START SUB-FUNCTION*******MAIN AREA****//Declare each new variable here - initializing and declaring space/memory for return arrays of variables or output we want****STEP B//
int main(int argc, char **argv){


	FILE *recon, *fin, *ferr, *fins,*ferri;
	int *intperiod,*start_h,*stop_h,c, lens=0, combos=0,counters=0,starto=0,tachy=0,lenny=0,gap=0,gap2=0,GPUn=0,gapn=0,gapo=0,dd=0,startg=0,stopg=0;
	long *dt;
	float *zscores_d, *pnl_d, *pos_d, *rets_d,*a_d,*a_h,*pos,*pnl,*zscores,*rets;
	double *op,*hi,*lo,*p,*price, *smooth, *detrender, *period, *qu, *iu, *ji, *jq, *ib, *qb, *sib, *sqb,*re, *im,*sre,*sim,*speriod,*smperiod,*qc,*ic,*ric,*sig,*nois,*snr, *cumpnl, *sharp;
	double *sumi, *vari, *stdevi, *m_avei,*sumv, *varv, *stdevv, *m_avev, *dolls, *cumdolls, *sumip, *varip, *stdevip, *m_aveip, *sharpp;
	int i=0,combo=0,ii=0,zz=0,wins=0,counter=0,start=1,stop=0,*start_d,*stop_d,startf=0,stopf=0,beg=1,high=0,m=0,mm=0,gg=0; char desty[50],dest[50],destr[50],desta[50],tmp[50],strs[50],foldr[50],fnum[50],fnums[50],fnumss[50],dir[50]; int peri[100] = { { 0 } };

	double pp[] = { 42000.00, 42000.00, 50.00, 20.00, 100.00, 100.00, 10.00, 25.00, 5.00, 1000.00, 1000.00, 2000.00, 1000.00, 2500.00, 100.00, 25000.00, 5000.00, 50.00, 100000.00, 125000.00, 125000.00, 125000.00, 62500.00, 50.00, 1000.00, 10000.00, 50.00 };
	char *marks[] = {"RBOB","HO","SP", "ND", "EMD", "TF", "FESX", "FDAX", "NK", "US", "TY", "TU", "FGBL", "ED", "GC", "HG", "SI", "PL", "AD", "EC", "SF", "JY", "BP", "S", "CL", "NG", "C"};
	char *dfiles[] = {PATH"data0.dat",PATH"data1.dat",PATH"data2.dat",PATH"data3.dat",PATH"data4.dat",PATH"data5.dat",PATH"data6.dat",PATH"data7.dat",PATH"data8.dat",PATH"data9.dat",PATH"data10.dat",PATH"data11.dat",PATH"data12.dat",PATH"data13.dat",
		PATH"data14.dat",PATH"data15.dat",PATH"data16.dat",PATH"data17.dat",PATH"data18.dat",PATH"data19.dat",PATH"data20.dat",PATH"data21.dat",PATH"data22.dat",PATH"data23.dat",PATH"data24.dat",PATH"data25.dat",PATH"data26.dat"};

	char sources[60],source[60],line[100]; long end, endf;
	double a[] = { 1.25, 1.50 };		//array holder for parameter combinations later on aka "parameter sweeps" which GPU can greatly speed up// a[] is # standard deviations//
	double b[] = { 21.00, 34.00 };
	//double b[] = { 3.0, 5.0, 8.0, 10.0 };
	double lensa=sizeof(a)/sizeof(double); double maxi=0.00,mat=0.00;
	double lensb=sizeof(b)/sizeof(double);
	double lensc=lensa * lensb;				//number of parameter combinations
	double sharplist[100][7][100] = { { 0 } };
	double table[100][5]= { { 0 } }; double ocum[100][3][100]= { { 0 } };
	int z=0, j=0, winos=0,lensz=0; double sumss[100] = { { 0 } }; double avv=0.00, sharpie=0.00, sharpies=0.00;
	
	//CHECK for USER INPUT larger than MAX NUMI of Markets
	if (NUMI > 27)
    {
        fprintf(stderr, "You entered too many markets! MAX number is 27! Please try again!\n");
        exit(EXIT_FAILURE); }

	// Error code to check return values for CUDA calls - check for num of GPU devices and return error in NA
    	cudaError_t err = cudaSuccess;
	err=cudaGetDeviceCount(&GPUn);

	if (err != cudaSuccess)
    {
        fprintf(stderr, "Failed to find GPU devices  (error code %s)!\n", cudaGetErrorString(err));
        exit(EXIT_FAILURE); }

	//loop thru as many GPU devices as there are available - so now we are MULTI-STREAMING and using MULTI-GPU's - this is FULL POWER of GPU computing
for (dd = 0; dd < GPUn; dd++) {

	clock_t ff, ss; float diff=0.00f;
	ff = dd;
	ff = clock();
	strcpy(destr, PATH"recon");		//output RECON directory for each GPU
	strcpy(foldr, ".dat"); 
	sprintf(fnumss, "%d", dd);
	strcat(destr,fnumss);  
	strcat(destr,foldr); 
	recon=f_openw(destr);	

	err=cudaSetDevice(dd);
	if (err != cudaSuccess)
    {
        fprintf(stderr, "Failed to find GPU device  (error code %s)!\n", cudaGetErrorString(err));
        exit(EXIT_FAILURE); }

	//divide market data array up into blocks of markets to run on EACH GPU
	gapn = NUMI/GPUn;
	gapo = NUMI/GPUn;
	startg = 1 + (dd*gapn);

	//check for odd number of markets divided by GPUs and append odd number to last iteration of this outer loop
	if (dd == GPUn-1 && NUMI % GPUn != 0) gapn = (((int)((NUMI % GPUn)* GPUn))+gapo)-1;
	if (gapn == 0) gapn = 1;

for (gg = 1; gg <= gapn; gg++) {		// top loop for number of market data files passed thru dfiles[] // must change NUMI in #def as add number of markets // should I use STRUCT instead to store file names?

	
	sprintf(sources,dfiles[(gg-1)+(dd*gapo)]);			//find and open price data files to get lengths for periodicities//
	fins=f_openr(sources);
	endf=f_line(fins);
	endf--;
	peri[gg] = (int)(((endf-LOOKBACK)/STEP)+1); //number of rolling periods in each data set for rolling optimization (aka moving average)//
	f_close(sources,fins);
	sprintf(fnums, "%d", gg);
	strcpy(desta, PATH"OSrunALL");		//output directory for out-of-sample tests for all combined tests per market
	strcpy(foldr, ".dat");
	strcat(desta,"-");
	strcat(desta,fnums);
	strcat(desta,"-");
	strcat(desta,marks[(gg-1)+(dd*gapo)]);
	strcat(desta,foldr);
	ferri=f_openw(desta);

 for (ii = 1; ii <= peri[gg] ; ii++) {	// loop is for periodicity - so 30yrs of price data divided into sub-units for rolling optimization (aka parameter sweeps)
  for (z = 0; z < lensa; z++) {			// 2 nested for loops for parameter sweep or combination of arrays a[] and b[]//
   for (j = 0; j < lensb; j++) {

	lens = (int)(b[j]);
	mm=(lensa*z)+j;
	sprintf(strs, "%d", mm);
	sprintf(fnum, "%d", gg);
	sprintf(tmp, "%d", ii);
	strcpy(desty, PATH"ISout");		//output directory for in-sample tests
	strcpy(foldr, ".dat");
	strcat(desty,strs);
	strcat(desty,"-");
	strcat(desty,fnum);
	strcat(desty,"-");
	strcat(desty,tmp);
	strcat(desty,"-");
	strcat(desty,marks[(gg-1)+(dd*gapo)]);
	strcat(desty,foldr);
	ferr=f_openw(desty);				//find and open output file//
	sprintf(source,dfiles[(gg-1)+(dd*gapo)]);			//find and open price data file//
	fin=f_openr(source);
	end=f_line(fin);
	end--;
	start = beg + (STEP*(ii-1));		//start stop dates for inner loop ii for rolling period optimizations
	stop = LOOKBACK + (STEP*(ii-1));

	dt=(long*) calloc(end+1,sizeof(long));
	op=(double*) calloc(end+1,sizeof(double));
	hi=(double*) calloc(end+1,sizeof(double));
	lo=(double*) calloc(end+1,sizeof(double));
	p=(double*) calloc(end+1,sizeof(double));
	price=(double*) calloc(end+1,sizeof(double));
	smooth=(double*) calloc(end+1,sizeof(double));
	detrender=(double*) calloc(end+1,sizeof(double));
	period=(double*) calloc(end+1,sizeof(double));
	qu=(double*) calloc(end+1,sizeof(double));
	iu=(double*) calloc(end+1,sizeof(double));
	ji=(double*) calloc(end+1,sizeof(double));
	jq=(double*) calloc(end+1,sizeof(double));
	ib=(double*) calloc(end+1,sizeof(double));
	qb=(double*) calloc(end+1,sizeof(double));
	sib=(double*) calloc(end+1,sizeof(double));
	sqb=(double*) calloc(end+1,sizeof(double));
	re=(double*) calloc(end+1,sizeof(double));
	im=(double*) calloc(end+1,sizeof(double));
	sre=(double*) calloc(end+1,sizeof(double));
	sim=(double*) calloc(end+1,sizeof(double));
	speriod=(double*) calloc(end+1,sizeof(double));
	smperiod=(double*) calloc(end+1,sizeof(double));
	qc=(double*) calloc(end+1,sizeof(double));
	ic=(double*) calloc(end+1,sizeof(double));
	ric=(double*) calloc(end+1,sizeof(double));
	intperiod=(int*) calloc(end+1,sizeof(double));
	sig=(double*) calloc(end+1,sizeof(double));
	nois=(double*) calloc(end+1,sizeof(double));
	snr=(double*) calloc(end+1,sizeof(double));


	//** ALLOCATE SPACE FOR MEMORY FOR CUDA-RELATED HOST VARIABLES - USE PINNED/SHARED MEMORY FOR MORE SPEED!!! **

	//rets=(float*) calloc(end+1,sizeof(float));
	cudaHostAlloc(&rets, (int)(end)*sizeof(float), cudaHostAllocDefault);
	//memset(rets, 0, (int)(end)*sizeof(float));
	dolls=(double*) calloc(end+1,sizeof(double));
	cumdolls=(double*) calloc(end+1,sizeof(double));
	//pos=(float*) calloc(end+1,sizeof(float));
	cudaHostAlloc(&pos, (int)(end)*sizeof(float), cudaHostAllocDefault);
	//memset(pos, 0, (int)(end)*sizeof(float));
	//pnl=(float*) calloc(end+1,sizeof(float));
	cudaHostAlloc(&pnl, (int)(end)*sizeof(float), cudaHostAllocDefault);
	//memset(pnl, 0, (int)(end)*sizeof(float));
	cumpnl=(double*) calloc(end+1,sizeof(double));
	//zscores=(float*) calloc(end+1,sizeof(float));
	cudaHostAlloc(&zscores, (int)(end)*sizeof(float), cudaHostAllocDefault);
	//memset(zscores, 0, (int)(end)*sizeof(float));

	sharpp=(double*) calloc(end+1,sizeof(double));
	sumip=(double*) calloc(end+1,sizeof(double));
	varip=(double*) calloc(end+1,sizeof(double));
	stdevip=(double*) calloc(end+1,sizeof(double));
	m_aveip=(double*) calloc(end+1,sizeof(double));
	sharp=(double*) calloc(end+1,sizeof(double));
	sumi=(double*) calloc(end+1,sizeof(double));
	vari=(double*) calloc(end+1,sizeof(double));
	stdevi=(double*) calloc(end+1,sizeof(double));
	m_avei=(double*) calloc(end+1,sizeof(double));
	sumv=(double*) calloc(end+1,sizeof(double));
	varv=(double*) calloc(end+1,sizeof(double));
	stdevv=(double*) calloc(end+1,sizeof(double));
	m_avev=(double*) calloc(end+1,sizeof(double));


	//** ALLOCATE SPACE FOR MEMORY FOR CUDA-RELATED DEVICE VARIABLES**

	cudaMalloc((void**)&zscores_d, (int)(end)*sizeof(float));
	//cudaMemset(zscores_d, 0, (int)(end)*sizeof(float));
	cudaMalloc((void**)&pos_d, (int)(end)*sizeof(float));
	cudaMemset(pos_d, 0, (int)(end)*sizeof(float));
	cudaMalloc((void**)&pnl_d, (int)(end)*sizeof(float));
	cudaMemset(pnl_d, 0, (int)(end)*sizeof(float));
	cudaMalloc((void**)&rets_d, (int)(end)*sizeof(float));
	//cudaMemset(rets_d, 0, (int)(end)*sizeof(float));
	/*cudaMalloc((void**)&a_d, 1);
	cudaMalloc((void**)&start_d, 1);
	cudaMalloc((void**)&stop_d, 1);*/



//INSERT NEW STUFF HERE EACH TIME START SUB-FUNCTION*************///CALLING function//STEP C//

	i=0;
	while(fgets(line,100,fin)>0){
		sscanf(line,"%ld %lf %lf %lf %lf",&dt[i],&op[i],&hi[i],&lo[i],&p[i]);i++;}  //scan lines from data file and store in arrays - this is price data here//
	f_close(source,fin);															//close data file

	//Using or CALLING function here//  DO NOT NEED TO DEFINE INPUTS-OUTPUTS here - that is done at bottom down BELOW!!//

	ret(p, end, rets);
	//snf(op, lo, hi, end, price, smooth, detrender, period, qu, iu, ji, jq, ib, qb, sib, sqb, re, im, sre, sim, speriod, smperiod, qc, ic, ric, intperiod, sig, nois, snr);
	zscore(lens, p, sumv, varv, end, zscores, stdevv, m_avev);
	m = (lensa*z)+j;

	//** COPY CUDA VARIABLES FROM CPU (HOST) TO GPU (DEVICE) - USE ASYNC TRANSFER FOR MORE SPEED SO CPU DOES NOT HAVE TO WAIT FOR GPU TO FINISH OPERATION AND CAN PROCEED FURTHER IN THE MAIN PROGRAM**
	cudaMemcpyAsync(zscores_d, zscores, (int)(end)*sizeof(float), cudaMemcpyHostToDevice,0);
	cudaMemcpyAsync(rets_d, rets, (int)(end)*sizeof(float), cudaMemcpyHostToDevice,0);

	gap = (stop - start)/STR;
	lenny=stop-start;
	dim3 threads; threads.x = THR;		//use 896 threads as per specific GPU device for higher OCCUPANCY/USE OF CARD - trial-and-error via PROFILING
    dim3 blocks; blocks.x = (lenny/threads.x) + 1;  //max blocks is 112 on GTX 670 device
	//kernelSim<<<threads,blocks>>>(zscores_d,rets_d,pnl_d,pos_d,start,stop,a[z]);

	// allocate and initialize an array of stream handles
    cudaStream_t *streams = (cudaStream_t *) malloc(STR * sizeof(cudaStream_t));

	//** Create Streams for Concurrency or Multi-Streaming - now we will call several KERNELS simultaneously**
	for(int i = 0; i < STR; i++) cudaStreamCreate(&(streams[i]));

	//** CALL GPU FUNCTION/KERNEL HERE FOR MODEL PARAMETER SWEEP TO GENERATE IN_SAMPLE RESULTS**THIS IS THREAD REDUCTION DUE TO CONCURRENCY!
    //kernelSim<<<threads,32>>>(zscores_d,rets_d,pnl_d,pos_d,start,stop,(float)(a[z]),lens);
	for (i = 0; i < STR; i++)
		{
			kernelSim<<<32,threads,0,streams[i]>>>(zscores_d,rets_d,pnl_d,pos_d,start+(i*gap),start+((i+1)*gap),(float)(a[z]),lens);
			if (i == STR-1) kernelSim<<<32,threads,0,streams[i]>>>(zscores_d,rets_d,pnl_d,pos_d,start+(i*gap),stop,(float)(a[z]),lens);
		}

	//SYNC up STREAMS before copying back data to CPU
	cudaStreamSynchronize(streams[STR-1]);

	//** COPY CUDA VARIABLES/RESULTS FROM GPU (DEVICE) BACK TO CPU (HOST) - MUST WAIT FOR GPU OPERATION/FUNCTION TO FINISH HERE SINCE LOW ASYNC/CONCURRENCY ON NON_TESLA GPU DEVICES**
	cudaMemcpy(pos, pos_d, (int)(end)*sizeof(float)/*stop-start*/, cudaMemcpyDeviceToHost);
	cudaMemcpy(pnl, pnl_d, (int)(end)*sizeof(float), cudaMemcpyDeviceToHost);

	//** Destroy Streams for Concurrency or Multi-Streaming - now we will RELEASE resources back to GPU**
	for(int i = 0; i < STR; i++) cudaStreamDestroy(streams[i]);

	//for(i=start;i<stop;i++){														//IN-sample rolling optimization for old CPU CODE - NOW WE'RE USING GPU INSTEAD FOR MORE SPEED**
	//
	//	if(zscores[i] > a[z]) pos[i] = 1.00;
	//	if(zscores[i] < -a[z]) pos[i] = -1.00;
	//	pnl[i] = (pos[i] * rets[i]); }


		sharpep(pnl, sumip, varip, start, stop, stdevip, m_aveip, sharpp);
		table[m][0] = m;
		table[m][1] = a[z];
		table[m][2] = b[j];
		table[m][3] = sharpp[stop-1];
		sharpie = sharpie + sharpp[stop-1];//end?
		if (table[m][3] > 0.00) combo = combo + 1;
		counter=counter+1;
		//table[m][4] = cumpnl[stop-1];
		printf("\nIS Test%.0f Market%d-%s Period%d", table[m][0],gg,marks[(gg-1)+(dd*gapo)],ii);
		printf("\nSharpe\t%.2f", table[m][3]);
		printf("\nParam1\t%.2f", table[m][1]);
		printf("\nParam2\t%.0f", table[m][2]);
		//printf("\nCum Ret\t%.2f%%", table[m][4]*100);
		//printf("\nAnn Ret\t%.2f%%", (table[m][4]*100)/(LOOKBACK/260));
		//printf("\nAnn Vol\t%.2f%%", ABS(((table[m][4]*100)/(LOOKBACK/260))/table[m][3]));
		printf("\nNum of Years: %.2f thru %.2f of %.2f total\n", ((((ii-1)*(double)(STEP)))/260),(((double)(LOOKBACK) + (ii*(double)(STEP)))/260)-(double)(STEP)/260,((double)(end)/260));
		fprintf(recon,"\nIS Test%.0f Market%d-%s Period%d", table[m][0],gg,marks[(gg-1)+(dd*gapo)],ii);
		fprintf(recon,"\nSharpe\t%.2f", table[m][3]);
		fprintf(recon,"\nParam1\t%.2f", table[m][1]);
		fprintf(recon,"\nParam2\t%.0f", table[m][2]);
		//fprintf(recon,"\nCum Ret\t%.2f%%", table[m][4]*100);
		//fprintf(recon,"\nAnn Ret\t%.2f%%", (table[m][4]*100)/(LOOKBACK/260));
		//fprintf(recon,"\nAnn Vol\t%.2f%%", ABS(((table[m][4]*100)/(LOOKBACK/260))/table[m][3]));
		fprintf(recon,"\nNum of Years: %.2f thru %.2f of %.2f total\n", ((((ii-1)*(double)(STEP)))/260),(((double)(LOOKBACK) + (ii*(double)(STEP)))/260)-(double)(STEP)/260,((double)(end)/260));

	for(i=start;i<stop;i++) {
		fprintf(ferr,"%ld\t %10.6lf\t %10.3lf\t %10.2lf\t %10.5lf\t %10.5lf\t %10.5lf\n",dt[i],p[i],zscores[i],pos[i],rets[i],pnl[i],sharpp[i]); } //

	f_close(desty,ferr);														//close output file

	for (i = 0; i < lensc; i++)													//find best sharpe ratio from table
            {
                if (table[i][3] > maxi) maxi = table[i][3];
                if (maxi == table[i][3]) high=i;
            }

			sharplist[gg][0][ii] = high;										//row of max sharpe recap
			sharplist[gg][6][ii] = table[high][3];								//max sharpe
			sharplist[gg][1][ii] = table[high][1];								//param 1 recap
			sharplist[gg][2][ii] = table[high][2];								//param 2 recap
			sharplist[gg][3][ii] = table[high][4];								//cum ret recap
			sharplist[gg][4][ii] = table[high][0];								//test number recap
			sharplist[gg][5][ii] = gg;											//market number recap

            maxi=0.00;

//ADD IN EACH POINTER VARIABLE HERE - FREEING UP SPACE IN MEMORY*******STEP D//

	//cudaDeviceReset();

	free(hi);free(lo);free(p);free(price);free(smooth);free(detrender);free(period);free(qu);free(iu);free(ji);free(jq);free(ib);free(qb);free(sib);free(re);free(im);free(sre);free(sim);free(speriod);free(smperiod);free(qc);free(ic);free(ric);free(intperiod);free(sig);free(nois);free(snr);
	cudaFreeHost(rets);cudaFreeHost(zscores);free(cumpnl);free(op);free(sharp);free(sumi);free(vari);free(stdevi);free(m_avei);
	free(sumv);free(varv);free(stdevv);free(m_avev);free(dolls);free(cumdolls);free(sharpp);free(sumip);free(varip);free(stdevip);free(m_aveip);//free(a_h);free(start_h);free(stop_h);
	cudaFree(zscores_d);cudaFree(pnl_d);cudaFree(pos_d);cudaFree(rets_d);cudaFreeHost(pos);cudaFreeHost(pnl);
	//cudaFreeHost(zscores_d);cudaFreeHost(pnl_d);cudaFreeHost(pos_d);cudaFreeHost(rets_d);

		}
	  }

	avv = avv+sharplist[gg][6][ii]/peri[gg];									//avg max sharpe
	if (sharplist[gg][6][ii] > 0.00) wins = wins+ii/peri[gg];					//winning markets
	printf("\n%.2f Max Sharpe of Market%d-%s Period%d is Test %.0f with STD %.2f and MA %.0f\n", sharplist[gg][6][ii],gg,marks[(gg-1)+(dd*gapo)],ii,sharplist[gg][0][ii],sharplist[gg][1][ii],sharplist[gg][2][ii]);
	fprintf(recon,"\n%.2f Max Sharpe of Market%d-%s Period%d is Test %.0f with STD %.2f and MA %.0f\n", sharplist[gg][6][ii],gg,marks[(gg-1)+(dd*gapo)],ii,sharplist[gg][0][ii],sharplist[gg][1][ii],sharplist[gg][2][ii]);


	sprintf(fnum, "%d", gg);
	sprintf(tmp, "%d", ii);
	strcpy(dest, PATH"OSrun");		//output directory for out-of-sample tests
	strcpy(foldr, ".dat");
	strcat(dest,"-");
	strcat(dest,fnum);
	strcat(dest,"-");
	strcat(dest,tmp);
	strcat(dest,"-");
	strcat(dest,marks[(gg-1)+(dd*gapo)]);
	strcat(dest,foldr);
	ferr=f_openw(dest);				//find and open output file//
	sprintf(source,dfiles[(gg-1)+(dd*gapo)]);	//find and open price data file//
	fin=f_openr(source);
	end=f_line(fin);
	end--;


	dt=(long*) calloc(end+1,sizeof(long));
	op=(double*) calloc(end+1,sizeof(double));
	hi=(double*) calloc(end+1,sizeof(double));
	lo=(double*) calloc(end+1,sizeof(double));
	p=(double*) calloc(end+1,sizeof(double));
	price=(double*) calloc(end+1,sizeof(double));
	smooth=(double*) calloc(end+1,sizeof(double));
	detrender=(double*) calloc(end+1,sizeof(double));
	period=(double*) calloc(end+1,sizeof(double));
	qu=(double*) calloc(end+1,sizeof(double));
	iu=(double*) calloc(end+1,sizeof(double));
	ji=(double*) calloc(end+1,sizeof(double));
	jq=(double*) calloc(end+1,sizeof(double));
	ib=(double*) calloc(end+1,sizeof(double));
	qb=(double*) calloc(end+1,sizeof(double));
	sib=(double*) calloc(end+1,sizeof(double));
	sqb=(double*) calloc(end+1,sizeof(double));
	re=(double*) calloc(end+1,sizeof(double));
	im=(double*) calloc(end+1,sizeof(double));
	sre=(double*) calloc(end+1,sizeof(double));
	sim=(double*) calloc(end+1,sizeof(double));
	speriod=(double*) calloc(end+1,sizeof(double));
	smperiod=(double*) calloc(end+1,sizeof(double));
	qc=(double*) calloc(end+1,sizeof(double));
	ic=(double*) calloc(end+1,sizeof(double));
	ric=(double*) calloc(end+1,sizeof(double));
	intperiod=(int*) calloc(end+1,sizeof(double));
	sig=(double*) calloc(end+1,sizeof(double));
	nois=(double*) calloc(end+1,sizeof(double));
	snr=(double*) calloc(end+1,sizeof(double));

	//** ALLOCATE SPACE FOR MEMORY FOR CUDA-RELATED HOST VARIABLES - USE PINNED/SHARED MEMORY FOR MORE SPEED!!! **

	//rets=(float*) calloc(end+1,sizeof(float));
	cudaHostAlloc(&rets, (int)(end)*sizeof(float), cudaHostAllocDefault);
	//memset(rets, 0, (int)(end)*sizeof(float));
	dolls=(double*) calloc(end+1,sizeof(double));
	cumdolls=(double*) calloc(end+1,sizeof(double));
	//pos=(float*) calloc(end+1,sizeof(float));
	cudaHostAlloc(&pos, (int)(end)*sizeof(float), cudaHostAllocDefault);
	//memset(pos, 0, (int)(end)*sizeof(float));
	//pnl=(float*) calloc(end+1,sizeof(float));
	cudaHostAlloc(&pnl, (int)(end)*sizeof(float), cudaHostAllocDefault);
	//memset(pnl, 0, (int)(end)*sizeof(float));
	cumpnl=(double*) calloc(end+1,sizeof(double));
	//zscores=(float*) calloc(end+1,sizeof(float));
	cudaHostAlloc(&zscores, (int)(end)*sizeof(float), cudaHostAllocDefault);
	//memset(zscores, 0, (int)(end)*sizeof(float));

	sharpp=(double*) calloc(end+1,sizeof(double));
	sumip=(double*) calloc(end+1,sizeof(double));
	varip=(double*) calloc(end+1,sizeof(double));
	stdevip=(double*) calloc(end+1,sizeof(double));
	m_aveip=(double*) calloc(end+1,sizeof(double));
	sharp=(double*) calloc(end+1,sizeof(double));
	sumi=(double*) calloc(end+1,sizeof(double));
	vari=(double*) calloc(end+1,sizeof(double));
	stdevi=(double*) calloc(end+1,sizeof(double));
	m_avei=(double*) calloc(end+1,sizeof(double));
	sumv=(double*) calloc(end+1,sizeof(double));
	varv=(double*) calloc(end+1,sizeof(double));
	stdevv=(double*) calloc(end+1,sizeof(double));
	m_avev=(double*) calloc(end+1,sizeof(double));

	//** ALLOCATE SPACE FOR MEMORY FOR CUDA-RELATED DEVICE VARIABLES**

	cudaMalloc((void**)&zscores_d, (int)(end)*sizeof(float));
	//cudaMemset(zscores_d, 0, (int)(end)*sizeof(float));
	cudaMalloc((void**)&pos_d, (int)(end)*sizeof(float));
	cudaMemset(pos_d, 0, (int)(end)*sizeof(float));
	cudaMalloc((void**)&pnl_d, (int)(end)*sizeof(float));
	cudaMemset(pnl_d, 0, (int)(end)*sizeof(float));
	cudaMalloc((void**)&rets_d, (int)(end)*sizeof(float));


	starto = LOOKBACK + (STEP*(ii-1));
	stopf = LOOKBACK + (STEP*(ii-0));
	if(ii>1) tachy = 1;			//use to go back n peroids for max sharpe offset
	if(stopf>=end) stopf = end;

	i=0;
	while(fgets(line,100,fin)>0){
		sscanf(line,"%ld %lf %lf %lf %lf",&dt[i],&op[i],&hi[i],&lo[i],&p[i]);i++;}  //scan lines from data file and store in arrays - this is price data here//
	f_close(source,fin);															//close data file
	ret(p, end, rets);
	lensz = (int)(sharplist[gg][2][ii-tachy] );
	zscore(lensz, p, sumv, varv, end, zscores, stdevv, m_avev);

	//** COPY CUDA VARIABLES FROM CPU (HOST) TO GPU (DEVICE) - USE ASYNC TRANSFER FOR MORE SPEED SO CPU DOES NOT HAVE TO WAIT FOR GPU TO FINISH OPERATION AND CAN PROCEED FURTHER IN THE MAIN PROGRAM**
	cudaMemcpyAsync(zscores_d, zscores, (int)(end)*sizeof(float), cudaMemcpyHostToDevice,0);
	cudaMemcpyAsync(rets_d, rets, (int)(end)*sizeof(float), cudaMemcpyHostToDevice,0);

	gap2 = (stopf - starto)/STR;
	lenny=stopf-starto;
	dim3 threads; threads.x = THR;  //use 896 threads as per specific GPU device for higher OCCUPANCY/USE OF CARD - trial-and-error via PROFILING
    dim3 blocks; blocks.x = (lenny/threads.x) + 1;
	//kernelSim<<<threads,blocks>>>(zscores_d,rets_d,pnl_d,pos_d,start,stop,a[z]);

	// allocate and initialize an array of stream handles
    cudaStream_t *streams = (cudaStream_t *) malloc(STR * sizeof(cudaStream_t));

	//** Create Streams for Concurrency or Multi-Streaming - now we will call several KERNELS simultaneously**
	for(int i = 0; i < STR; i++) cudaStreamCreate(&(streams[i]));

	//** CALL GPU FUNCTION/KERNEL HERE FOR MODEL PARAMETER SWEEP TO GENERATE OS_SAMPLE RESULTS**THIS IS THREAD REDUCTION DUE TO CONCURRENCY!
    //kernelSim<<<threads,32>>>(zscores_d,rets_d,pnl_d,pos_d,start,stop,(float)(a[z]),lens);
	for (i = 0; i < STR; i++)
		{
			kernelSim<<<32,threads,0,streams[i]>>>(zscores_d,rets_d,pnl_d,pos_d,starto+(i*gap2),starto+((i+1)*gap2),(float)(sharplist[gg][1][ii-tachy]),lensz);
			if (i == STR-1) kernelSim<<<32,threads,0,streams[i]>>>(zscores_d,rets_d,pnl_d,pos_d,starto+(i*gap2),stopf,(float)(sharplist[gg][1][ii-tachy]),lensz);
		}

	//SYNC up STREAMS before copying back data to CPU
	cudaStreamSynchronize(streams[STR-1]);

	//** COPY CUDA VARIABLES/RESULTS FROM GPU (DEVICE) BACK TO CPU (HOST) - MUST WAIT FOR GPU OPERATION/FUNCTION TO FINISH HERE SINCE LOW ASYNC/CONCURRENCY ON NON_TESLA GPU DEVICES**
	cudaMemcpy(pos, pos_d, (int)(end)*sizeof(float)/*stop-start*/, cudaMemcpyDeviceToHost);
	cudaMemcpy(pnl, pnl_d, (int)(end)*sizeof(float), cudaMemcpyDeviceToHost);

	//** Destroy Streams for Concurrency or Multi-Streaming - now we will RELEASE resources back to GPU**
	for(int i = 0; i < STR; i++) cudaStreamDestroy(streams[i]);


	//for(i=starto;i<stopf;i++){														//OUT-OF-SAMPLE runs for old CPU CODE - NOW WE'RE USING GPU INSTEAD FOR MORE SPEED**
	//
	//	if(zscores[i] > sharplist[gg][1][ii-tachy]) pos[i] = 1.00;
	//	if(zscores[i] < -sharplist[gg][1][ii-tachy]) pos[i] = -1.00;
	//	pnl[i] = (pos[i] * rets[i]);}




		sharpep(pnl, sumip, varip, starto, stopf, stdevip, m_aveip, sharpp);
		//ocum[gg][0][ii] = cumpnl[stopf-1];
		//ocum[gg][1][ii] = cumdolls[stopf-1];
		ocum[gg][2][ii] = sharpp[stopf-1];
		if (sharpp[stopf-1] > 0.00) combos = combos + 1;
		sharpies = sharpies + sharpp[stopf-1]/(peri[gg]);
		counters=counters+1;
		mat =(((((ii+0)*(double)(STEP)))+LOOKBACK)/260);
		if (mat >= ((double)(end)/260)) mat = ((double)(end)/260);
		if (stop>=end) mat = ((double)(end)/260);
		printf("\nOS PNL: Market%d-%s Period%d", gg,marks[(gg-1)+(dd*gapo)],ii);
		printf("\nOS-Sharpe\t%.2f", sharpp[stopf-1]);
		printf("\nOS-Param1\t%.2f", sharplist[gg][1][ii-tachy]);
		printf("\nOS-Param2\t%.0f", sharplist[gg][2][ii-tachy]);
		//printf("\nOS-Cum Ret\t%.2f%%", cumpnl[stopf-1]*100);
		//printf("\nOS-Ann Ret\t%.2f%%", (cumpnl[stopf-1]*100)/(LOOKBACK/260));
		//printf("\nOS-Ann Vol\t%.2f%%", ABS(((cumpnl[stopf-1]*100)/(LOOKBACK/260))/sharpp[stopf-1]));
		printf("\nNum of Years: %.2f thru %.2f of %.2f total\n", (((((ii-1)*(double)(STEP)))+LOOKBACK)/260),mat,((double)(end)/260));
		fprintf(recon,"\nOS PNL: Market%d-%s Period%d", gg,marks[(gg-1)+(dd*gapo)],ii);
		fprintf(recon,"\nOS-Sharpe\t%.2f", sharpp[stopf-1]);
		fprintf(recon,"\nOS-Param1\t%.2f", sharplist[gg][1][ii-tachy]);
		fprintf(recon,"\nOS-Param2\t%.0f", sharplist[gg][2][ii-tachy]);
		//fprintf(recon,"\nOS-Cum Ret\t%.2f%%", cumpnl[stopf-1]*100);
		//fprintf(recon,"\nOS-Ann Ret\t%.2f%%", (cumpnl[stopf-1]*100)/(LOOKBACK/260));
		//fprintf(recon,"\nOS-Ann Vol\t%.2f%%", ABS(((cumpnl[stopf-1]*100)/(LOOKBACK/260))/sharpp[stopf-1]));
		fprintf(recon,"\nNum of Years: %.2f thru %.2f of %.2f total\n", (((((ii-1)*(double)(STEP)))+LOOKBACK)/260),mat,((double)(end)/260));

	for(i=starto;i<stopf;i++) {
		fprintf(ferr,"%ld\t %10.6lf\t %10.3lf\t %10.2lf\t %10.5lf\t %10.5lf\t %10.5lf\n",dt[i],p[i],zscores[i],pos[i],rets[i],pnl[i],sharpp[i]);
		fprintf(ferri,"%ld\t %10.6lf\t %10.3lf\t %10.2lf\t %10.5lf\t %10.5lf\t %10.5lf\n",dt[i],p[i],zscores[i],pos[i],rets[i],pnl[i],sharpp[i]+ocum[gg][2][ii-1]); } //


	f_close(dest,ferr);							//close output file

	if (sharpp[stopf-1] > 0.00) winos = winos+ii/(peri[gg]);
	/*ocum[gg][0][ii] = cumpnl[stopf-1];
	ocum[gg][1][ii] = cumdolls[stopf-1];
	ocum[gg][2][ii] = sharpp[stopf-1];*/
	tachy=0;

	free(hi);free(lo);free(p);free(price);free(smooth);free(detrender);free(period);free(qu);free(iu);free(ji);free(jq);free(ib);free(qb);free(sib);free(re);free(im);free(sre);free(sim);free(speriod);free(smperiod);free(qc);free(ic);free(ric);free(intperiod);free(sig);free(nois);free(snr);
	cudaFreeHost(rets);cudaFreeHost(zscores);free(cumpnl);free(op);free(sharp);free(sumi);free(vari);free(stdevi);free(m_avei);//cudaFree(zscores);cudaFree(rets);cudaFree(pnl);cudaFree(pos);
	free(sumv);free(varv);free(stdevv);free(m_avev);free(dolls);free(cumdolls);free(sharpp);free(sumip);free(varip);free(stdevip);free(m_aveip);cudaFreeHost(pos);cudaFreeHost(pnl);
	cudaFree(zscores_d);cudaFree(pnl_d);cudaFree(pos_d);cudaFree(rets_d);

	}

	f_close(desta,ferri);						//close output file

}

	//cudaDeviceSynchronize();
	ss = dd;
	ss = clock();
	diff = (((float)ss - (float)ff) / 1000000.0F ) * 1;
	printf("\n\n%.3f min..Avg I-MaxSharpe is %.2f..+I-Sharpes are %d of %d combos or %.0f%%..\n+%d Win Markets out of %i..Avg All I-Sharpes is %.2f\n\n",diff/60.0,avv/NUMI,combo,counter,(((double)(combo)/(double)(counter))*100),wins,NUMI,sharpie/(double)(counter));
	fprintf(recon,"\n\n%.3f min..Avg I-MaxSharpe is %.2f..+I-Sharpes are %d of %d combos or %.0f%%..\n+%d Win Markets out of %i..Avg All I-Sharpes is %.2f\n\n",diff/60.0,avv/NUMI,combo,counter,(((double)(combo)/(double)(counter))*100),wins,NUMI,sharpie/(double)(counter));

	printf("\n\n%.3f min..+O-Sharpes are %d of %d combos or %.0f%%..\n+%d Win Markets out of %i..Avg All O-Sharpes is %.2f\n\n",diff/60.0,combos,counters,(((double)(combos)/(double)(counters))*100),winos,NUMI,sharpies/(double)(NUMI));//(double)(counters)); //NUMI
	fprintf(recon,"\n\n%.3f min..+O-Sharpes are %d of %d combos or %.0f%%..\n+%d Win Markets out of %i..Avg All O-Sharpes is %.2f\n\n",diff/60.0,combos,counters,(((double)(combos)/(double)(counters))*100),winos,NUMI,sharpies/(double)(NUMI)); //NUMI

	f_close(destr,recon); 
	//cudaDeviceReset();
	cudaSetDevice(dd);
       cudaDeviceReset();
	diff=0.00f;



}

	//**RESET GPU DEVICE**//	
	for (i = 0; i < GPUn; i++)
    	{
        cudaSetDevice(i);
        cudaDeviceReset();
    	}


}



long f_line(FILE *f)  {               /*count the lines*/
	char ch;
	long count;
	count=1;
	while( (ch = getc(f)) != EOF)
	  	if(ch == '\n')
			count++;
    fseek(f,0L,SEEK_SET);
	return(count);	}

FILE * f_openr( char *filer)   {
	FILE *f;
	f=fopen(filer,"r");
	if(f == NULL)	{
		perror("Error");
		fprintf(stderr," file %s cannot be open under r mode \n",filer);
		exit(1);	}
	return f;	}


FILE * f_openw( char *filer)      {
	FILE *f;
	f=fopen(filer,"w");
	if(f == NULL)	{
		perror("Error");
		fprintf(stderr," file %s cannot be open under w mode \n",filer);
		exit(1);	}
	return f;	}



void f_close(char *filer,FILE *f){
	if(fclose(f) !=0)	{
		printf("error in closing file %s \n",filer);
		exit(2);	}	}



void snf(double *op, double *lo, double *hi, long end, double *price, double *smooth, double *detrender, double *period, double *qu, double *iu, double *ji, double *jq, double *ib, double *qb, double *sib, double *sqb, double *re, double *im, double *sre, double *sim, double *speriod, double *smperiod, double *qc, double *ic, double *ric, int *intperiod, double *sig, double *nois, double *snr){

	int i,count;

	for(i=1;i<end;i++){
		if(i>5) price[i] = ((op[i]+op[i])/2);
		if(i>5) smooth[i] = (4*price[i] + 3*price[i-1] + 2*price[i-2] + price[i-3]) / 10;
		if(i>5) detrender[i] = (.0962*smooth[i] + .5769*smooth[i-2] - .5769*smooth[i-4] - .0962*smooth[i-6])*(.075*period[i-1] + .54);
		if(i>5) iu[i] = detrender[i-3];
		if(i>5) qu[i] = (.0962*detrender[i] + .5769*detrender[i-2] - .5769*detrender[i-4] - .0962*detrender[i-6])*(.075*period[i-1] + .54);
		if(i>5) ji[i] = (.0962*iu[i] + .5769*iu[i-2] - .5769*iu[i-4] - .0962*iu[i-6])*(.075*period[i-1] + .54);
		if(i>5) jq[i] = (.0962*qu[i] + .5769*qu[i-2] - .5769*qu[i-4] - .0962*qu[i-6])*(.075*period[i-1] + .54);
		if(i>5) ib[i] = iu[i] - jq[i];
		if(i>5) qb[i] = qu[i] + ji[i];
		if(i>5) sib[i] = .2*ib[i] + .8*ib[i-1];
		if(i>5) sqb[i] = .2*qb[i] + .8*qb[i-1];
		if(i>5) re[i] = sib[i]*sib[i-1]+sqb[i]*sqb[i-1];
		if(i>5) im[i] = sib[i]*sqb[i-1]-sqb[i]*sib[i-1];
		if(i>5) sre[i] = .2*re[i] + .8*re[i-1];
		if(i>5) sim[i] = .2*im[i] + .8*im[i-1];
		if(sim[i]!=0&&sre[i]!=0&&i>5) period[i] = 360/atan(sim[i]/sre[i]);
		if(i>5&&period[i]>period[i-1]*1.5) period[i]=1.5*period[i-1];
		if(i>5&&period[i]<period[i-1]*.67) period[i]=.67*period[i-1];
		if(i>5&&period[i]<6) period[i]=6;
		if(i>5&&period[i]>50) period[i]=50;
		if(i>5) speriod[i] = .2*period[i] + .8*period[i-1];
		if(i>5) smperiod[i] = .33*speriod[i] + .67*smperiod[i-1];
		if(i>5) intperiod[i] = (int)(smperiod[i]*.5);
		if(i>5) qc[i] = .5*(smooth[i]-smooth[i-2])*(.1759*smperiod[i]+.4607);
			ic[0]=0;
		for(count=0;count<=intperiod[i]-1;count++){
			ic[i] = qc[count]+ic[i];}
		if(i>5&&(intperiod[i])!=0) ic[i]=1.57*ic[i]/(intperiod[i]);
		if(i>5) sig[i] = ic[i]*ic[i] + qc[i]*qc[i];
		if(i>5) nois[i] = .1*(hi[i]-lo[i])*(hi[i]-lo[i])*.25+.9*nois[i-1];
		if(i>5&&nois[i]!=0&&sig[i]!=0) snr[i] = .33*(10*log(sig[i]/nois[i])/log(10.00))+.67*snr[i-1];  // REF LEVEL is 8 by 13 //
		//snr[i] = snr[i]*.5;

	}
	return;
}


//Zscore FUNC//
void zscore(int lens, double *op, double *sumv, double *varv, long end, float *zscores, double *stdevv, double *m_avev){


	int i,h;
	varv[0] = 0;
	sumv[0] = 0;
	m_avev[0] = 0;
	stdevv[0] = 0;
	for(i=1;i<end;i++) sumv[i]=sumv[i-1]+op[i];
	for(i=1;i<end;i++){
		if(i>=lens) m_avev[i]=(sumv[i]-sumv[i-lens])/lens;
		if(i>=lens) for(h=0;h<lens;h++) varv[i]+=(((op[i-h]-m_avev[i])*(op[i-h]-m_avev[i]))/(lens-1));
		if(i>=lens) stdevv[i]=sqrt(varv[i]);	//using square root function from math library here//
	    if(i>=lens) zscores[i]=(op[i] - m_avev[i]) / stdevv[i];	}

	return;
}


//Get RETURNS function from market closing price//
void ret(double *p, long end, float *rets){


	int i=0;
	rets[0] = 0;
	//if(i>0) {
		for(i=1;i<end;i++) rets[i]=(p[i] - p[i-1]) / p[i-1];
	//else {rets[i] = 0.00; }

	return;
}

//Calc SHARPE RATIO FUNC of PNL returns//
void sharpe(float *pnl, double *sumi, double *vari, long end, double *stdevi, double *m_avei, double *sharp){


	int i,h;
	vari[0] = 0;
	sumi[0] = 0;
	m_avei[0] = 0;
	stdevi[0] = 0;
	for(i=1;i<end;i++) sumi[i]=sumi[i-1]+pnl[i];
	for(i=1;i<end;i++){
		if(i>1) m_avei[i]=((sumi[i])/i);
		if(i>1) for(h=0;h<i;h++) vari[i]+=(((pnl[i-h]-m_avei[i])*(pnl[i-h]-m_avei[i]))/(i-1));
		if(i>1) stdevi[i]=sqrt(vari[i]);	//using square root function from math library here//
	    if(i>1) sharp[i]=((m_avei[i]*260) / (stdevi[i]*sqrt(260.00)));	}

	return;
}

void sharpep(float *pnl, double *sumip, double *varip, int start, int stop, double *stdevip, double *m_aveip, double *sharpp){


	int i,h;
	varip[0] = 0;
	sumip[0] = 0;
	m_aveip[0] = 0;
	stdevip[0] = 0;
	for(i=start;i<stop;i++) sumip[i]=sumip[i-1]+pnl[i];
	for(i=start;i<stop;i++){
		if(i>start) m_aveip[i]=((sumip[i])/i);
		if(i>start) for(h=0;h<i;h++) varip[i]+=(((pnl[i-h]-m_aveip[i])*(pnl[i-h]-m_aveip[i]))/(i-1));
		if(i>start) stdevip[i]=sqrt((varip[i]));	//using square root function from math library here//
	    if(i>start) sharpp[i]=((m_aveip[i]*260) / (stdevip[i]*sqrt(260.00)));	}

	return;
}


__global__ void kernelSim(float *zscores_d,float *rets_d,float *pnl_d,float *pos_d,int start,int stop,float zcut,int lens){



	//float zcut = *a_d;
	//int starty = *start_d;
	//int stoppy = *stop_d;
	//int opt = 0;
	const float buy =1.00f;
	const float sell=-1.00f;
	const float flat=0.00f;
	//const float trans=0.0002f;

	//const float scut = zcut;
	//const float scut = floorf(zcut * 1000) / 1000;   /* Result: 37.77 */



	//Thread index
    unsigned int      tid = blockDim.x * blockIdx.x + threadIdx.x;
    //Total number of threads in execution grid
    //const int THREAD_N = blockDim.x * gridDim.x;
	unsigned int THREAD_N = blockDim.x * gridDim.x;

	// __syncthreads();

    //No matter how small is execution grid or how large OptN is,
    //exactly OptN indices will be processed with perfect memory coalescing


	for(int opt = tid+start; opt < stop; opt += THREAD_N){
	//if(tid < stoppy){
		if(zscores_d[opt] > zcut && opt >=lens) pos_d[opt] = buy;
		if(zscores_d[opt] < -zcut && opt >=lens) pos_d[opt] = sell;
		if(opt >=lens && (pos_d[opt]==buy || pos_d[opt]==sell)) pnl_d[opt] = __fmul_rn(pos_d[opt],rets_d[opt]);
		else {pnl_d[opt] = flat; pos_d[opt] = flat; }

	}


}
