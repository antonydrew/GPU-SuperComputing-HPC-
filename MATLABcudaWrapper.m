cc=date;
disp(cc);
disp('                                                                                ');

%clear workspace and matrices and suppress warning messages
clear all;
clc;
warning off;

%start clock timer (wall clock)
tic

%Check/set GPU device
g = gpuDevice(1);

%Number of optims
Cut = 1810;

%Get market data from flat files and store in matrix
setdbprefs({'DataReturnFormat','ErrorHandling','NullNumberRead','NullNumberWrite','NullStringRead','NullStringWrite','JDBCDataSourceFile'},{'numeric','empty','NaN','NaN','null','null',''});
conn = database('BLP32','','');
e = exec(conn,'SELECT ALL "pcrhc$"."pcr date","pcrhc$"."pcr price","spxhc$"."spx price" FROM "pcrhc$","spxhc$" WHERE "pcrhc$"."pcr date" = "spxhc$"."spx date"');
e = fetch(e);
BLP = e.Data;
close(e)
close(conn)


Blotter = BLP;
Blotter(:,4:5) = single(zscore(BLP(:,2:3)));
Zpcr =single(zscore(Blotter(:,4)));
Zspx = single(zscore(Blotter(:,5)));
dates = Blotter(:,1);
lens = length(dates);


 %Set parameters for z-score cutoff, moving-average length and rolling
 %period
assignin('base', 'zii', '1');
assignin('base', 'fii', '21');
assignin('base', 'pii', '377');

%start outer loop for optimizations
for fors=1:Cut;

 %assign space in memory and set initial values to zero for matrices 
ret =single( zeros(lens,1));
fp= single(zeros(lens,2));
fproll=single(zeros(lens,5));
ZpcrRoll=single(zeros(lens,2));
ZspxRoll=single(zeros(lens,2));
lookback=str2double(pii);
period =str2double(fii);
zcut=str2double(zii);
dates1=zeros(lens,2);
dates1f=zeros(lens,2);
dirf=zeros(lens,2);
dir=zeros(lens,2);
dirup=zeros(lens,1);
dirupf=zeros(lens,1);
dirdn=zeros(lens,1);
dirdnf=zeros(lens,1);

for lp=1:lens;
    if lp == 1
            ret(lp,1)=0;
    else ret(lp,1)=((Blotter(lp,3) - Blotter(lp-1,3)) / Blotter(lp-1,3));
    end;
end;

Blotter(:,6)= ret;

for lp=1:lens;
    if lp >= lookback
    ZpcrRoll(lp-lookback+1:lp,1) = zscore(Blotter(lp-lookback+1:lp,2));
    ZpcrRoll(lp,2)=ZpcrRoll(lp,1);
    ZspxRoll(lp-lookback+1:lp,1) = zscore(Blotter(lp-lookback+1:lp,3));
    ZspxRoll(lp,2)=ZspxRoll(lp,1);
    else
        ZpcrRoll(1:lookback,2)=zscore(Blotter(1:lookback,2));
        ZspxRoll(1:lookback,2)=zscore(Blotter(1:lookback,3));
    end;
end;

Blotter(:,7)= single(ZpcrRoll(:,2));
Blotter(:,12)=single( ZspxRoll(:,2));

%copy data from CPU to GPU
Bl7   = gpuArray( single(Blotter(:,7)) );
Bl6   = gpuArray( single(Blotter(:,6) ));
Fpr   = gpuArray( single(fproll) );
Fpr2   = gpuArray( single(fproll) );
Fpr4   = gpuArray( single(fproll) );

%run main calculations on GPU
for lp=1:lens-period+1;
    if Bl7(lp) > zcut && zcut > 0
            Fpr(lp)=Bl6(lp);
            for lp2=0:period-1;
                Fpr2(lp+lp2)=Bl6(lp+lp2);
            end;
            Fpr4(lp)=sum(Bl6(lp:lp+period-1));
    elseif Bl7(lp) < zcut && zcut < 0
             Fpr(lp)=Bl6(lp);
            for lp2=0:period-1;
                Fpr2(lp+lp2)=Bl6(lp+lp2);
            end;
            Fpr4(lp)=sum(Bl6(lp:lp+period-1));
    else Fpr(lp)=0;
    end;
end;

%Gather results on GPU and send back to CPU
f1=gather(Fpr);
f2=gather(Fpr2);
f4=gather(Fpr4);

%assign results to main data matrix
fproll(:,1)= (f1(:,1));
fproll(:,2)= (f2(:,1));
fproll(:,4)= (f4(:,1));

for lp=1:lens;
    if lp > 1
            fproll(lp,5)=fproll(lp,4)+fproll(lp-1,5);            
    else fproll(lp,5)=0;
    end;
end;

fproll(:,3)=Blotter(:,1);
Blotter(:,9)=fproll(:,2);

for lp=1:lens;
    if fproll(lp,1) ~=0;
    dates1f(lp,1) = fproll(lp,3);
    dates1f(lp,2)=fproll(lp,4);
        else 
        dates1f(lp,1)=nan;
        dates1f(lp,2)=nan;
            end;
end;

sort_rf=dates1f(:,2);
sort_df=dates1f(:,1);
sorted_datesf = sort_df( ~isnan(sort_df) );
sorted_dates_rf=sort_rf( ~isnan(sort_rf) );
sorted_datesf(:,2)=sorted_dates_rf;
lensdf = length(sorted_datesf);


for lp=1:lens;
    if Blotter(lp,1)== dates1f(lp,1);
            Blotter(lp,11)=dates1f(lp,2);
    end;
end;

sumdays = sum(dir(:,2));

for lp=1:lens;
    if fproll(lp,1) ~=0;
    dirf(lp,1) = fp(lp,1);
    dirf(lp,2) = 1;
        else 
        dirf(lp,1)=0;
        dirf(lp,2)=0;
            end;
end;

sumdaysf = sum(dirf(:,2));


for lp=1:lens;
    if fproll(lp,1) >0;
    dirupf(lp,1) = 1;
            else 
        dirupf(lp,1)=0;
   end;
end;


for lp=1:lens;
    if fproll(lp,1) <0;
    dirdnf(lp,1) = 1;
            else 
        dirdnf(lp,1)=0;
   end;
end;

uppercf = sum(dirupf)./sum(dirf);
downpercf = sum(dirdnf)./sum(dirf);
sumretroll=sum(fproll(:,4));
meanretroll=sumretroll./sum(dirf);
 
%print results log to screen
 str2=sprintf('%s  %g %s','CBOE PCR OUTPUT OVER ROLLING PERIOD',lookback,'DAYS');
 disp(str2);
 disp('-----------------------------------------------------------------------------------------------------------------');
disp('SPX ZSCORE   PCR ZSCORE    PCR ZCUT      #INSTANCES     AVG SPX RET%      TOTAL SPX RET%      %UP DAYS     %DN DAYS')  ;
str3 = sprintf('%7.4g        %7.4g                %g                     %g                     %7.4g                    %7.4g                      %g                  %g',Blotter(lens,12),Blotter(lens,7),zcut,sum(dirf(:,2)),meanretroll(1,2)*100,sumretroll*100,uppercf(1,2)*100,downpercf(1,2)*100);
disp(str3);
disp('                                                                                ');

datef=datestr(Blotter(:,1),'mm/dd/yy');

DATE = [datef];
PCR = [Blotter(:,2)];
SPX = [Blotter(:,3)];
PCRstaticZscore = [Blotter(:,4)];
PCRrollingZscore = [Blotter(:,7)];
StaticFwdRet = [Blotter(:,10)*100];
RollFwdRet = [Blotter(:,11)*100];
format bank;
Pres = dataset(DATE,PCR,SPX,PCRstaticZscore,PCRrollingZscore,StaticFwdRet,RollFwdRet);

%export fomratted data to .dat file in local directory
export(Pres,'file','PCRdata.dat');

end;

toc

%reset GPU device
reset(g);

