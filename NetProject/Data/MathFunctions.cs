using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Windows.Forms.DataVisualization.Charting;
using System.Web.UI.DataVisualization.Charting;



namespace Data
{
    //class for math and statistical computations 
    public class MathFunctions
    {
        //method that calculates the average close across entire history
        public static double AverageAllPNLrollHold(DataTable dt)
        {
            //local double variable for the average of entire close population
            double average = 0.00;

            //use LINQ to query the market table for the adjusted close            
            var query = from data in dt.AsEnumerable()
                        select data.Field<Double?>("RealPNLrollHold");

            //average of all the adjusted closes 
            average = (double)query.Average();

            //return Convert.ToDouble(average);
            return average;
                        
        }

        public static double ZscoreStaticCount(DataTable dt)
        {
            //local double variable for the average of entire close population
            double sums = 0.00;

            //use LINQ to query the market table for the adjusted close            
            var query = from data in dt.AsEnumerable()
                        select data.Field<Double?>("PositionsStatic");

            //average of all the adjusted closes 
            sums = Math.Abs((double)query.Sum());

            //return Convert.ToDouble(average);
            return sums;

        }

        public static double ZscoreRollCount(DataTable dt)
        {
            //local double variable for the average of entire close population
            double sums = 0.00;

            //use LINQ to query the market table for the adjusted close            
            var query = from data in dt.AsEnumerable()
                        select data.Field<Double?>("PositionsRoll");

            //average of all the adjusted closes 
            sums = Math.Abs((double)query.Sum());

            //return Convert.ToDouble(average);
            return sums;

        }

        public static double SharpePNLrollHold(DataTable dt)
        {
            //local double variable for the average of entire close population
            double average = 0.00;
            double vol = 0.00;
            double sharpe = 0.00;
            average = AverageAllPNLrollHold(dt) * 260;
            vol = StdDevAllPNLrollHold(dt) * Math.Sqrt(260);
                        
            //average of all the adjusted closes 
            sharpe = (average - .01) / vol;

            return sharpe;
        }

        public static double SumAllPNLrollHold(DataTable dt)
        {
            //local double variable for the average of entire close population
            double cum = 0.00;

            //use LINQ to query the market table for the adjusted close            
            var query = from data in dt.AsEnumerable()
                        select data.Field<Double?>("PNLrollHold");

            //average of all the adjusted closes 
            cum = (double)query.Sum();

            return cum;

        }

        public static double CountPNLrollHold(DataTable dt)
        {
            //local double variable for the average of entire close population
            
            //use LINQ to query the market table for the adjusted close            
            var query = from data in dt.AsEnumerable()
                        select data.Field<Double?>("RealPNLrollHold");

            //get mean of all the adjusted closes and get variance and stdev
            int n = 0;
            foreach (var value in query)
            {
                if (value > 0)
                {
                    n++;
                }

            }
           
            return n;

        }

        public static double StdDevAllPNLrollHold(DataTable dt)
        {
            //local double variable for the average of entire close population
            double mean = 0.00;
            double stdev = 0.00;
            double sum = 0.00;

            //use LINQ to query the market table for the adjusted close            
            var query = from data in dt.AsEnumerable()
                        select data.Field<Double?>("RealPNLrollHold");

            //get mean of all the adjusted closes and get variance and stdev
            int n = 0;
            foreach (var value in query)
            {
                n++;
                double delta = value == null ? 0.00 : (double)value - mean;
                mean += delta / n;
                sum += value == null ? 0.00 : delta * ((double)value - mean);
            }
            if (1 < n)
                stdev = Math.Sqrt(sum / (n - 1));            

            return stdev;  
           
        }

        public static double StdDevAllCloseA(double[] calcs)
        {
            //local double variable for the average of entire close population
            double mean = 0.00;
            double stdev = 0.00;
            double sum = 0.00;

            //use LINQ to query the market table for the adjusted close            
            //var query = from data in dt.AsEnumerable()
            //            select data.Field<double>("AdjustedClose");

            //get mean of all the adjusted closes and get variance and stdev
            int n = 0;
            foreach (double value in calcs)
            {
                n++;
                double delta = value - mean;
                mean += delta / n;
                sum += delta * (value - mean);
            }
            if (1 < n)
                stdev = Math.Sqrt(sum / (n - 1));
            //stdev = Math.Sqrt(StatisticFormula.Variance("timeseries",true));

            return stdev;
        }

        public static double ZscoreAllClose(DataTable dt)
        {
            //local double variable for the average of entire close population
            double mean = 0.00;
            double stdev = 0.00;
            double sum = 0.00;
            double zscore = 0.00;

            //use LINQ to query the market table for the adjusted close            
            var query = from data in dt.AsEnumerable()
                        select data.Field<double>("AdjustedClose");

            //get mean of all the adjusted closes and get variance and stdev
            int n = 0;
            foreach (var value in query)
            {
                n++;
                double delta = value - mean;
                mean += delta / n;
                sum += delta * (value - mean);
            }
            if (1 < n)
                stdev = Math.Sqrt(sum / (n - 1));
            //stdev = Math.Sqrt(StatisticFormula.Variance("timeseries",true));

            int x = 0;
            foreach (var value in query)
            {
                x++;
                zscore = (value - mean) / stdev;

            }

            return zscore;
        }

        public static double ZscoreAllCloseA(double[] calcs)
        {
            //local double variable for the average of entire close population
            double mean = 0.00;
            double stdev = 0.00;
            double sum = 0.00;
            double zscore = 0.00;

            //get mean of all the adjusted closes and get variance and stdev
            int n = 0;
            foreach (double value in calcs)
            {
                n++;
                double delta = value - mean;
                mean += delta / n;
                sum += delta * (value - mean);
            }
            if (1 < n)
                stdev = Math.Sqrt(sum / (n - 1));
            //stdev = Math.Sqrt(StatisticFormula.Variance("timeseries",true));

            int x = 0;
            foreach (double value in calcs)
            {
                x++;
                zscore = (value - mean) / stdev;

            }

            return zscore;
        }

        //method that returns a SL of adjusted close rolling averages based on specified days                   
        public static SortedList<DateTime, double> AverageRolling(DataTable dt, int numDays)
        {
            numDays = 100;
            //instantiate SL for holding the rolling average of adjusted closes of N days
            SortedList<DateTime, double> rollingaverage = new SortedList<DateTime, double>();
            SortedList<DateTime, double> calculation = new SortedList<DateTime, double>();

            //query the market table with LINQ for Date and Adjusted Close
            var query = from data in dt.AsEnumerable()
                        select new
                        {
                            Date = data.Field<DateTime>("Date"),
                            AdjClose = data.Field<double>("AdjustedClose")
                        };

            //load the calculation SL for computations
            foreach (var item in query)
                calculation.Add(item.Date, item.AdjClose);

            //calculate and add rolling average of adjusted closes
            for (int i = 0; i < calculation.Count(); i++)
            {
                if (i >= numDays - 1)
                {
                    //array for numDays of adjusted closes to average
                    double[] prices = new double[numDays];

                    //load adjusted closes into arrays
                    for (int j = numDays; j > 0; j--)
                    {
                        prices[j - 1] = calculation.Values[(i + 1) - j];
                    }
                    rollingaverage.Add(calculation.Keys[i], prices.Average());
                }
            }
            return rollingaverage;
        }

        //method that returns a SL of adjusted close rolling averages based on specified days                   
        public static SortedList<DateTime, double> StdDevRolling(DataTable dt, int numDays)
        {
            //instantiate SL for holding the rolling average of adjusted closes of N days
            SortedList<DateTime, double> rollingstd = new SortedList<DateTime, double>();
            SortedList<DateTime, double> calculation = new SortedList<DateTime, double>();

            //query the market table with LINQ for Date and Adjusted Close
            var query = from data in dt.AsEnumerable()
                        select new
                        {
                            Date = data.Field<DateTime>("Date"),
                            AdjClose = data.Field<double>("AdjustedClose")
                        };

            //load the calculation SL for computations
            foreach (var item in query)
                calculation.Add(item.Date, item.AdjClose);

            //calculate and add rolling average of adjusted closes
            for (int i = 0; i < calculation.Count(); i++)
            {
                if (i >= numDays - 1)
                {
                    //array for numDays of adjusted closes to average
                    double[] prices = new double[numDays];

                    //load adjusted closes into arrays
                    for (int j = numDays; j > 0; j--)
                    {
                        prices[j - 1] = calculation.Values[(i + 1) - j];
                    }

                    rollingstd.Add(calculation.Keys[i], StdDevAllCloseA(prices));
                }
            }
            return rollingstd;
        }

        public static SortedList<DateTime, double> ZscoreStatic(DataTable dt)
        {
            //instantiate SL for holding the rolling average of adjusted closes of N days
            SortedList<DateTime, double> zscorestatic = new SortedList<DateTime, double>();
            SortedList<DateTime, double> calculation = new SortedList<DateTime, double>();

            //query the market table with LINQ for Date and Adjusted Close
            var query = from data in dt.AsEnumerable()
                        select new
                        {
                            Date = data.Field<DateTime>("Date"),
                            AdjClose = data.Field<double>("Open")
                        };

            //load the calculation SL for computations
            foreach (var item in query)
                calculation.Add(item.Date, item.AdjClose);

            //calculate and add static average of adjusted closes
            double[] prices = new double[calculation.Count()];
            for (int i = 0; i < calculation.Count(); i++)
            {
                //load adjusted closes into arrays
                for (int k = 0; k < calculation.Count(); k++)
                {
                    prices[k] = calculation.Values[k];
                }

                double zscoreStatic = 0.00;
                zscoreStatic = (prices[i] - prices.Average()) / StdDevAllCloseA(prices);
                zscorestatic.Add(calculation.Keys[i], zscoreStatic);
            }

            return zscorestatic;
        }

        public static SortedList<DateTime, double> Returns(DataTable dt)
        {
            //instantiate SL for holding the rolling average of adjusted closes of N days
            SortedList<DateTime, double> returns = new SortedList<DateTime, double>();
            SortedList<DateTime, double> calculation = new SortedList<DateTime, double>();

            //query the market table with LINQ for Date and Adjusted Close
            var query = from data in dt.AsEnumerable()
                        select new
                        {
                            Date = data.Field<DateTime>("Date"),
                            AdjClose = data.Field<double>("AdjustedClose")
                        };

            //load the calculation SL for computations
            foreach (var item in query)
                calculation.Add(item.Date, item.AdjClose);

            //calculate and add static average of adjusted closes
            double[] prices = new double[calculation.Count()];
            for (int i = 0; i < calculation.Count(); i++)
            {
                //load adjusted closes into arrays
                for (int k = 0; k < calculation.Count(); k++)
                {
                    prices[k] = calculation.Values[k];
                }

                if (i > 0)
                {
                    double rets = 0.00;
                    rets = (prices[i] - prices[i - 1]) / prices[i - 1];
                    returns.Add(calculation.Keys[i], rets);
                }
            }

            return returns;
        }
               
        public static SortedList<DateTime, double> ZscoreRolling(DataTable dt, int numDays)
        {
            //instantiate SL for holding the rolling average of adjusted closes of N days
            SortedList<DateTime, double> rollingzscore = new SortedList<DateTime, double>();
            SortedList<DateTime, double> calculation = new SortedList<DateTime, double>();

            //query the market table with LINQ for Date and Adjusted Close
            var query = from data in dt.AsEnumerable()
                        select new
                        {
                            Date = data.Field<DateTime>("Date"),
                            AdjClose = data.Field<double>("Open")
                        };

            //load the calculation SL for computations
            foreach (var item in query)
                calculation.Add(item.Date, item.AdjClose);

            //calculate and add rolling average of adjusted closes
            for (int i = 0; i < calculation.Count(); i++)
            {

                if (i >= numDays - 1)
                {
                    //array for numDays of adjusted closes to average
                    double[] prices = new double[numDays];

                    //load adjusted closes into arrays
                    for (int j = numDays; j > 0; j--)
                    {
                        prices[j - 1] = calculation.Values[(i + 1) - j];
                    }

                    double zscoreRoll = 0.00;
                    for (int j = numDays; j > 0; j--)
                    {
                        zscoreRoll = (prices[j - 1] - prices.Average()) / StdDevAllCloseA(prices);
                    }

                    rollingzscore.Add(calculation.Keys[i], zscoreRoll);
                }

            }
            return rollingzscore;
        }



    }
}



