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
//#include "helper_cuda.h"
//#include "helper_functions.h"

#pragma warning( disable : 4996 )

//Declaring macros and constants in pre-processor - STEP A//INSERT NEW STUFF DOWN HERE EACH TIME START SUB-FUNCTION******PRE-PROCESSOR AREA*******//
#define ABS(X) (X>=0?X:-X)
#define MAX(X,Y) (X>=Y?X:Y)
#define MIN(X,Y) (X<=Y?X:Y)
#define SIGN(X) (X>=0?(X==0?0:1):-1)
#define ROUND(X,Y) ((X>=0?(X<<1)+Y:(X<<1)-Y)/(Y<<1))*Y 

//Change path below for UNIX "c://usr"
#define PATH "F:\\"			
#define LOOKBACK 1597		// 1597-987-610-377-144-89 fibos rolling optimization historical period
#define STEP 377			// or 89 fibos step forward in time period for next rolling optimization
#define NUMI 27				//up to 27 number of markets 
   


//INT MAIN//INSERT NEW STUFF HERE EACH TIME START SUB-FUNCTION*******MAIN AREA****//Declare each new variable here - initializing and declaring space/memory for return arrays of variables or output we want****STEP B//
int main(int argc, char **argv){
//void main (int argc, char *argv[]){  
	
	FILE *recon, *fin, *ferr, *fins,*ferri;
	int *intperiod,*start_h,*stop_h,c, lens=0, combos=0,counters=0,starto=0,tachy=0,lenny=0;
	long *dt; 
	float *zscores_d, *pnl_d, *pos_d, *rets_d,*a_d,*a_h,*pos,*pnl,*zscores,*rets; 
	double *op,*hi,*lo,*p,*price, *smooth, *detrender, *period, *qu, *iu, *ji, *jq, *ib, *qb, *sib, *sqb,*re, *im,*sre,*sim,*speriod,*smperiod,*qc,*ic,*ric,*sig,*nois,*snr, *cumpnl, *sharp;
	double *sumi, *vari, *stdevi, *m_avei,*sumv, *varv, *stdevv, *m_avev, *dolls, *cumdolls, *sumip, *varip, *stdevip, *m_aveip, *sharpp;
	int i=0,combo=0,ii=0,zz=0,wins=0,counter=0,start=1,stop=0,*start_d,*stop_d,startf=0,stopf=0,beg=1,high=0,m=0,mm=0,gg=0; char desty[50],dest[50],desta[50],tmp[50],strs[50],foldr[50],fnum[50],fnums[50],dir[50]; int peri[100] = { { 0 } };
	
	double pp[] = { 42000.00, 42000.00, 50.00, 20.00, 100.00, 100.00, 10.00, 25.00, 5.00, 1000.00, 1000.00, 2000.00, 1000.00, 2500.00, 100.00, 25000.00, 5000.00, 50.00, 100000.00, 125000.00, 125000.00, 125000.00, 62500.00, 50.00, 1000.00, 10000.00, 50.00 };
	char *marks[] = {"RBOB","HO","SP", "ND", "EMD", "TF", "FESX", "FDAX", "NK", "US", "TY", "TU", "FGBL", "ED", "GC", "HG", "SI", "PL", "AD", "EC", "SF", "JY", "BP", "S", "CL", "NG", "C"};
	char *dfiles[] = {PATH"data0.dat",PATH"data1.dat",PATH"data2.dat",PATH"data3.dat",PATH"data4.dat",PATH"data5.dat","c:\\data6.dat",PATH"data7.dat",PATH"data8.dat",PATH"data9.dat",PATH"data10.dat",PATH"data11.dat",PATH"data12.dat",PATH"data13.dat",
		PATH"data14.dat",PATH"data15.dat",PATH"data16.dat",PATH"data17.dat",PATH"\\data18.dat",PATH"data19.dat",PATH"data20.dat",PATH"data21.dat",PATH"data22.dat",PATH"data23.dat",PATH"data24.dat",PATH"data25.dat",PATH"data26.dat"}; 
	
	clock_t t1, t2; float diff; char sources[60],source[60],line[100]; long end, endf;
	double a[] = { 1.25, 1.50 };		//array holder for parameter combinations later on aka "parameter sweeps" which GPU can greatly speed up// a[] is # standard deviations//
	double b[] = { 21.00, 34.00 };	
	//double b[] = { 3.0, 5.0, 8.0, 10.0 };			
	double lensa=sizeof(a)/sizeof(double); double maxi=0.00,mat=0.00;
	double lensb=sizeof(b)/sizeof(double);
	double lensc=lensa * lensb;				//number of parameter combinations
	double sharplist[100][7][100] = { { 0 } };
	double table[100][5]= { { 0 } }; double ocum[100][3][100]= { { 0 } };
	int z=0, j=0, winos=0,lensz=0; double sumss[100] = { { 0 } }; double avv=0.00, sharpie=0.00, sharpies=0.00;
	t1 = clock();
	recon=f_openw(PATH"recon.dat");	
		
for (gg = 1; gg <= NUMI; gg++) {		// top loop for number of market data files passed thru dfiles[] // must change NUMI in #def as add number of markets // should I use STRUCT instead to store file names?
	
	sprintf(sources,dfiles[gg-1]);			//find and open price data files to get lengths for periodicities//
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
	strcat(desta,marks[gg-1]);  
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
	strcat(desty,marks[gg-1]);  
	strcat(desty,foldr); 
	ferr=f_openw(desty);				//find and open output file//	
	sprintf(source,dfiles[gg-1]);			//find and open price data file//
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


	lenny=stop-start;
	dim3 threads; threads.x = 896;		//use 896 threads as per specific GPU device for higher OCCUPANCY/USE OF CARD - trial-and-error via PROFILING
    //dim3 blocks; blocks.x = ((int)(end)/threads.x) + 1;
	//kernelSim<<<threads,blocks>>>(zscores_d,rets_d,pnl_d,pos_d,start,stop,a[z]);

	//** CALL GPU FUNCTION/KERNEL HERE FOR MODEL PARAMETER SWEEP TO GENERATE IN_SAMPLE RESULTS**
    kernelSim<<<threads,112>>>(zscores_d,rets_d,pnl_d,pos_d,start,stop,(float)(a[z]),lens);

	//** COPY CUDA VARIABLES/RESULTS FROM GPU (DEVICE) BACK TO CPU (HOST) - MUST WAIT FOR GPU OPERATION/FUNCTION TO FINISH HERE SINCE LOW ASYNC/CONCURRENCY ON NON_TESLA GPU DEVICES**
	cudaMemcpy(pos, pos_d, (int)(end)*sizeof(float)/*stop-start*/, cudaMemcpyDeviceToHost);
	cudaMemcpy(pnl, pnl_d, (int)(end)*sizeof(float), cudaMemcpyDeviceToHost);
	
	

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
		printf("\nIS Test%.0f Market%d-%s Period%d", table[m][0],gg,marks[gg-1],ii);		
		printf("\nSharpe\t%.2f", table[m][3]);
		printf("\nParam1\t%.2f", table[m][1]);
		printf("\nParam2\t%.0f", table[m][2]);
		//printf("\nCum Ret\t%.2f%%", table[m][4]*100);
		//printf("\nAnn Ret\t%.2f%%", (table[m][4]*100)/(LOOKBACK/260));
		//printf("\nAnn Vol\t%.2f%%", ABS(((table[m][4]*100)/(LOOKBACK/260))/table[m][3]));
		printf("\nNum of Years: %.2f thru %.2f of %.2f total\n", ((((ii-1)*(double)(STEP)))/260),(((double)(LOOKBACK) + (ii*(double)(STEP)))/260)-(double)(STEP)/260,((double)(end)/260));
		fprintf(recon,"\nIS Test%.0f Market%d-%s Period%d", table[m][0],gg,marks[gg-1],ii);		
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
	printf("\n%.2f Max Sharpe of Market%d-%s Period%d is Test %.0f with STD %.2f and SNR %.0f\n", sharplist[gg][6][ii],gg,marks[gg-1],ii,sharplist[gg][0][ii],sharplist[gg][1][ii],sharplist[gg][2][ii]);	
	fprintf(recon,"\n%.2f Max Sharpe of Market%d-%s Period%d is Test %.0f with STD %.2f and SNR %.0f\n", sharplist[gg][6][ii],gg,marks[gg-1],ii,sharplist[gg][0][ii],sharplist[gg][1][ii],sharplist[gg][2][ii]);	

	
	sprintf(fnum, "%d", gg);
	sprintf(tmp, "%d", ii);		
	strcpy(dest, PATH"OSrun");		//output directory for out-of-sample tests
	strcpy(foldr, ".dat"); 
	strcat(dest,"-");  
	strcat(dest,fnum);  
	strcat(dest,"-");  
	strcat(dest,tmp); 
	strcat(dest,"-");  
	strcat(dest,marks[gg-1]); 
	strcat(dest,foldr); 
	ferr=f_openw(dest);				//find and open output file//	
	sprintf(source,dfiles[gg-1]);	//find and open price data file//
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

	lenny=stopf-starto;
	dim3 threads; threads.x = 896;  //use 896 threads as per specific GPU device for higher OCCUPANCY/USE OF CARD - trial-and-error via PROFILING
    //dim3 blocks; blocks.x = ((int)(end)/threads.x) + 1;
	//kernelSim<<<threads,blocks>>>(zscores_d,rets_d,pnl_d,pos_d,start,stop,a[z]);

	//** CALL GPU FUNCTION/KERNEL HERE FOR MODEL PARAMETER SWEEP TO GENERATE IN_SAMPLE RESULTS**
    kernelSim<<<threads,112>>>(zscores_d,rets_d,pnl_d,pos_d,starto,stopf,(float)(sharplist[gg][1][ii-tachy]),lensz);
	
	//** COPY CUDA VARIABLES/RESULTS FROM GPU (DEVICE) BACK TO CPU (HOST) - MUST WAIT FOR GPU OPERATION/FUNCTION TO FINISH HERE SINCE LOW ASYNC/CONCURRENCY ON NON_TESLA GPU DEVICES**
	cudaMemcpy(pos, pos_d, (int)(end)*sizeof(float)/*stop-start*/, cudaMemcpyDeviceToHost);
	cudaMemcpy(pnl, pnl_d, (int)(end)*sizeof(float), cudaMemcpyDeviceToHost);
	

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
		printf("\nOS PNL: Market%d-%s Period%d", gg,marks[gg-1],ii);		
		printf("\nOS-Sharpe\t%.2f", sharpp[stopf-1]);
		printf("\nOS-Param1\t%.2f", sharplist[gg][1][ii-tachy]);
		printf("\nOS-Param2\t%.0f", sharplist[gg][2][ii-tachy]);
		//printf("\nOS-Cum Ret\t%.2f%%", cumpnl[stopf-1]*100);
		//printf("\nOS-Ann Ret\t%.2f%%", (cumpnl[stopf-1]*100)/(LOOKBACK/260));
		//printf("\nOS-Ann Vol\t%.2f%%", ABS(((cumpnl[stopf-1]*100)/(LOOKBACK/260))/sharpp[stopf-1]));
		printf("\nNum of Years: %.2f thru %.2f of %.2f total\n", (((((ii-1)*(double)(STEP)))+LOOKBACK)/260),mat,((double)(end)/260));
		fprintf(recon,"\nOS PNL: Market%d-%s Period%d", gg,marks[gg-1],ii);		
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
	
	t2 = clock();   
	diff = (((float)t2 - (float)t1) / 1000000.0F ) * 1000;   
	printf("\n\n%.2f min..Avg I-MaxSharpe is %.2f..+I-Sharpes are %d of %d combos or %.0f%%..\n+%d Win Markets out of %i..Avg All I-Sharpes is %.2f\n\n",diff/60.0,avv/NUMI,combo,counter,(((double)(combo)/(double)(counter))*100),wins,NUMI,sharpie/(double)(counter)); 
	fprintf(recon,"\n\n%.2f min..Avg I-MaxSharpe is %.2f..+I-Sharpes are %d of %d combos or %.0f%%..\n+%d Win Markets out of %i..Avg All I-Sharpes is %.2f\n\n",diff/60.0,avv/NUMI,combo,counter,(((double)(combo)/(double)(counter))*100),wins,NUMI,sharpie/(double)(counter)); 
	
	printf("\n\n%.2f min..+O-Sharpes are %d of %d combos or %.0f%%..\n+%d Win Markets out of %i..Avg All O-Sharpes is %.2f\n\n",diff/60.0,combos,counters,(((double)(combos)/(double)(counters))*100),winos,NUMI,sharpies/(double)(NUMI));//(double)(counters)); //NUMI
	fprintf(recon,"\n\n%.2f sec..+O-Sharpes are %d of %d combos or %.0f%%..\n+%d Win Markets out of %i..Avg All O-Sharpes is %.2f\n\n",diff/60.0,combos,counters,(((double)(combos)/(double)(counters))*100),winos,NUMI,sharpies/(double)(NUMI)); //NUMI
	
	f_close(PATH"recon.dat",recon); 
	
	//**RESET GPU DEVICE**//
	cudaDeviceReset();
	system("pause");
	
}




















