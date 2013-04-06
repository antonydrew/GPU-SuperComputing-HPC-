void add(double *p,double *hi,double *lo,int begin,int end,struct t *tn)  //read data in here from S's price database//'int' is COUNTER//
{ 
int a1,a2,b1,b2,b3,b4,n1,n2,s1,s2,i,j,***score,tr_begin;//,c,n3;  //5000 calendar days plus 90 day lag//
//FILE *sco;

s1=sizeof score_lag/sizeof(int); //S is creating 3D array here for score as well as non-strict//
s2=sizeof score_strict/sizeof(int);

n1=sizeof add_entry/sizeof(int);
n2=sizeof add_exit/sizeof(int);
//n3=sizeof stop_pct/sizeof(int);

score=(int ***)malloc(s1*sizeof(int **));    //S is using allocate function for memory by number of parameters***score-length interger//
for(a1=0;a1<s1;a1++)						//now we have 3-D loop for score - notice idented brackets - always enclose FUNCTIONS in BRACKETS//
	{
	score[a1]=(int **)calloc(s2,sizeof(int *));
	for(a2=0;a2<s2;a2++) 
		{
		score[a1][a2]=(int *)calloc(end,sizeof(int));
		for(i=begin;i<end;i++)
			{
			if(p[i]==p[i-score_lag[a1]]) score[a1][a2][i]=0;     //Score being calced here and then recycled or called later on by signal buy/sell function//
			else if(score[a1][a2][i-1]>=0)
				{
				if((p[i]<p[i-score_lag[a1]] && score[a1][a2][i-1]<=score_strict[a2])	|| p[i]<lo[i-score_lag[a1]]) score[a1][a2][i]=score[a1][a2][i-1]+1;
				else
					{
					if(p[i]>p[i-score_lag[a1]]) score[a1][a2][i]=-1;
					else score[a1][a2][i]=0;
					}
				}
			else
				{
				if((p[i]>p[i-score_lag[a1]] && score[a1][a2][i-1]>=-score_strict[a2]) || p[i]>hi[i-score_lag[a1]]) score[a1][a2][i]=score[a1][a2][i-1]-1;
				else
					{
					if(p[i]<p[i-score_lag[a1]]) score[a1][a2][i]=1; 
					else score[a1][a2][i]=0;
					}
				}
			}
		}
	}

//now we start actual buy/sell business//"j" is number of parameters//
for(j=0,a1=0;a1<s1;a1++) for(a2=0;a2<s2;a2++) for(b1=0;b1<n1;b1++) for(b2=0;b2<n1;b2++) for(b3=0;b3<n2;b3++) for(b4=0;b4<n2;b4++,j++) //for(c=0;c<n3;c++,j++)
	{
	/*if(!(add_entry[b1]==7 && add_entry[b2]==7 && add_exit[b3]==3 && add_exit[b4]==2)) {j=0;continue;}
	if((sco=fopen("add_rule.csv","w"))==NULL) {printf("File add_rule.txt could not be opened\n");exit(1);}*/

	for(i=begin;i<end;i++)   //H likes for loops - notice "i++" which advances "i" by 1 with each loop//
		{
		if(score[a1][a2][i]==add_entry[b1])    //GOAL - ADD TRUE STOPS AND EXIT2 IN HERE!!!!!!!!!!!!1/////////
			{
			tn[j].tr[i]=1;
			tr_begin=i;
			}
		else if(score[a1][a2][i]==-add_entry[b2]) //S using negative sign of score in order to get Sells//
			{
			tn[j].tr[i]=-1;
			tr_begin=i;
			}
		else if((score[a1][a2][i]==-add_exit[b3] || score[a1][a2][i]==add_entry[b1]+score_lag[a1]) && tn[j].tr[i-1]==1)
			{
			tn[j].tr[i]=0;
			tr_begin=0;
			}
		else if((score[a1][a2][i]==add_exit[b4] || score[a1][a2][i]==-add_entry[b2]-score_lag[a1]) && tn[j].tr[i-1]==-1) 
			{
			tn[j].tr[i]=0;
			tr_begin=0;
			}
		/*else if(p[i]<p[tr_begin] && tn[j].tr[i-1]==1)    ///Leverage off of this ELSEIF to STACK MORE conditions in here//
			{
			tn[j].tr[i]=0;
			tr_begin=0;
			}*/
		else tn[j].tr[i]=tn[j].tr[i-1];
		//fprintf(sco,"%9.5lf,%9.5lf,%9.5lf,%3d,%2d\n",hi[i],lo[i],p[i],score[a1][a2][i],tn[j].tr[i]);
		}
	//fclose(sco);
	}
for(a1=0;a1<s1;a1++)
	{
	for(a2=0;a2<s2;a2++) free(score[a1][a2]);
	free(score[a1]);
	}
free(score);  //frees up memory and releases results of score//
}