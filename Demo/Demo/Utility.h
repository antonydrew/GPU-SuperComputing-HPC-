#pragma warning( disable : 4996 )

long f_line(FILE *f) ;
FILE * f_openr( char *filer); 		      	      
FILE * f_openw( char *filer);
void f_close(char *filer,FILE *f); 
void snf(double *op, double *lo, double *hi, long end, double *price, double *smooth, double *detrender, double *period, double *qu, double *iu, double *ji, double *jq, double *ib, double *qb, double *sib, double *sqb, double *re, double *im, double *sre, double *sim, double *speriod, double *smperiod, double *qc, double *ic, double *ric, int *intperiod, double *sig, double *nois, double *snr);  
void zscore(int lens, double *op, double *sumv, double *varv, long end, double *zscores, double *stdevv, double *m_avev);
void ret(double *p, long end, double *rets);
void sharpe(double *pnl, double *sumi, double *vari, long end, double *stdevi, double *m_avei, double *sharp);
void sharpep(double *pnl, double *sumip, double *varip, int start, int stop, double *stdevip, double *m_aveip, double *sharpp);
__global__ void kernelSim(float *zscores_d,float *rets_d,float *pnl_d,float *pos_d,int start,int stop,double zcut);