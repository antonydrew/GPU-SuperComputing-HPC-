#include <stdio.h>
#include <math.h>
#include <io.h>
#include <stdlib.h>
#include <string.h>
#include <float.h>
#include "utility.h"

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
void zscore(int lens, double *op, double *sumv, double *varv, long end, double *zscores, double *stdevv, double *m_avev){   
 		
 	
	int i,h;
	varv[0] = 0;
	sumv[0] = 0;
	m_avev[0] = 0;
	stdevv[0] = 0;
	for(i=1;i<end;i++) sumv[i]=sumv[i-1]+op[i];	
	for(i=1;i<end;i++){
		if(i>lens) m_avev[i]=(sumv[i]-sumv[i-lens])/lens;	
		if(i>lens) for(h=0;h<lens;h++) varv[i]+=(((op[i-h]-m_avev[i])*(op[i-h]-m_avev[i]))/(lens-1));	
		if(i>lens) stdevv[i]=sqrt(varv[i]);	//using square root function from math library here//
	    if(i>lens) zscores[i]=(op[i] - m_avev[i]) / stdevv[i];	}
		
	return;
}


//Get RETURNS function from market closing price//
void ret(double *p, long end, double *rets){   
 		
 	
	int i=0;
	rets[0] = 0;
	//if(i>0) {
		for(i=1;i<end;i++) rets[i]=(p[i] - p[i-1]) / p[i-1];	
	//else {rets[i] = 0.00; }
	
	return;
}

//Calc SHARPE RATIO FUNC of PNL returns//
void sharpe(double *pnl, double *sumi, double *vari, long end, double *stdevi, double *m_avei, double *sharp){   
 		
 	
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

void sharpep(double *pnl, double *sumip, double *varip, int start, int stop, double *stdevip, double *m_aveip, double *sharpp){   
 		
 	
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










