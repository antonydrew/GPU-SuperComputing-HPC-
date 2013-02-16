#include <stdio.h>
#include <math.h>
#include <io.h>
#include <stdlib.h>
#include <string.h> 
#include <float.h>

//forward declare function up here before main program
void s_tr(double *tr,double *hi, double *lo, double *cl, int n);

//start main program
void main (int argc, char *argv[]){
	int i,n;
	long numlin;

	//declare pointer array variables
	double *hi,*lo,*cl,*tr;

	char source[60],line[100];
 	FILE *fin;
	sprintf(source,"c:\\data\\%s.dat",argv[1]);
	fin=f_openr(source);
	numlin=f_line(fin);

	//allocate memory for pointer array variables
	hi=(double*) calloc(numlin+1,sizeof(double));
	lo=(double*) calloc(numlin+1,sizeof(double));
	cl=(double*) calloc(numlin+1,sizeof(double));
	tr=(double*) calloc(numlin+1,sizeof(double));

	i=0;

	//open file and read in data
	while(fgets(line,100,fin)>0){
		sscanf(line,"%*s %*s %lf %lf %lf",&hi[i],&lo[i],&cl[i++]);}

	//close file
	f_close(source,fin);

	//call function - pass by reference
	s_tr(tr,hi,lo,cl,i);

	//free memory of pointer array variables
	free (hi); free (lo); free (cl); free (tr);


}

//define function down here
void s_tr(double *tr,double *hi, double *lo, double *cl, int nday){
 	double m1,m2,m3,m4;
	int i;

	for(i=1;i<nday;i++){
		m1=abs(hi[i]-lo[i]);
		m2=abs(hi[i]-cl[i-1]);
		m3=abs(cl[i-1]-lo[i]);
		m4=max(m1,m2);
		tr[i]=max(m4,m3);}
	return;
}

/*Dim MyNumber
MyNumber = Abs(50.3)  ' Returns 50.3.*/

	//MAX(ABS(hi - lo),ABS(hi - cl),ABS(cl - lo));
	/*int max_value(float *p_array,
    unsigned int values_in_array,
float *p_max_value)
{
int position;

position = 0;
*p_max_value = p_array[position];
for (position = 1; position < values_in_array; ++position)
{
if (p_array[position] > *p_max_value)
{
*p_max_value = p_array[position];
break;
}
}
return position;}*/

	/*int iarr[] = { -7, -3, -1, -10 };
	int siz = sizeof iarr / sizeof iarr[0];
	int m, i, x = 0;
	m = iarr[0];
	for (i = 1; i < siz; ++i)	
	if (iarr[i] > m) {
	m = iarr[i];
	x = i;
	}
	printf("The greatest of %d values is %d at index %d\n", siz, m, x);	
	return 0;}*/
// #define max(a, b)  (((a) > (b)) ? (a) : (b))