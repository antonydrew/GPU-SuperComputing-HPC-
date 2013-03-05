using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.IO;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace Data
{
    /*
     * class that utilizes ADO.NET DataTable functionality
     * to represent each market's persistent CSV file on 
     * the user's local drive
     */
    public class MarketTable
    {
        //private member instance field
        private DataTable datatable;
        int holdDays;
        int rollingDays;
        double zcut;

        public MarketTable(double z, int hd, int rd)
        {
            Zcut = z;
            HoldDays = hd;
            RollingDays = rd;
        }

        //create a data table for a market
        public DataTable CreateTable(string ticker, string sourceDirectory)
        {
            //create the datatable            
            datatable = new DataTable();

            //create the columns
            DataColumn dateColumn = new DataColumn("Date", typeof(DateTime));
            DataColumn symbolColumn = new DataColumn("Symbol", typeof(string));
            DataColumn openColumn = new DataColumn("Open", typeof(double));
            DataColumn highColumn = new DataColumn("High", typeof(double));
            DataColumn lowColumn = new DataColumn("Low", typeof(double));
            DataColumn closeColumn = new DataColumn("Close", typeof(double));
            DataColumn volumeColumn = new DataColumn("Volume", typeof(double));
            DataColumn adjCloseColumn = new DataColumn("AdjustedClose", typeof(double));
            DataColumn Returns = new DataColumn("Returns", typeof(double));
            DataColumn rollingAverageColumn = new DataColumn("RollingAverage", typeof(double));
            DataColumn rollingStdColumn = new DataColumn("RollingStd", typeof(double));
            DataColumn ZscoreStaticColumn = new DataColumn("ZscoreStatic", typeof(double));
            DataColumn ZscoreRollingColumn = new DataColumn("ZscoreRolling", typeof(double));
            DataColumn PositionsStaticColumn = new DataColumn("PositionsStatic", typeof(double));
            DataColumn PNLstaticColumn = new DataColumn("PNLstatic", typeof(double));
            DataColumn CumPNLstaticColumn = new DataColumn("CumPNLstatic", typeof(double));
            DataColumn RealPNLstaticColumn = new DataColumn("RealPNLstatic", typeof(double));
            DataColumn CumRealPNLstaticColumn = new DataColumn("CumRealPNLstatic", typeof(double));
            DataColumn PositionsStaticHoldColumn = new DataColumn("PositionsStaticHold", typeof(double));
            DataColumn PNLstaticHoldColumn = new DataColumn("PNLstaticHold", typeof(double));
            DataColumn CumPNLstaticHoldColumn = new DataColumn("CumPNLstaticHold", typeof(double));
            DataColumn RealPNLstaticHoldColumn = new DataColumn("RealPNLstaticHold", typeof(double));
            DataColumn CumRealPNLstaticHoldColumn = new DataColumn("CumRealPNLstaticHold", typeof(double));
            DataColumn PositionsRollColumn = new DataColumn("PositionsRoll", typeof(double));
            DataColumn PNLrollColumn = new DataColumn("PNLroll", typeof(double));
            DataColumn CumPNLrollColumn = new DataColumn("CumPNLroll", typeof(double));
            DataColumn RealPNLrollColumn = new DataColumn("RealPNLroll", typeof(double));
            DataColumn CumRealPNLrollColumn = new DataColumn("CumRealPNLroll", typeof(double));
            DataColumn PositionsRollHoldColumn = new DataColumn("PositionsRollHold", typeof(double));
            DataColumn PNLrollHoldColumn = new DataColumn("PNLrollHold", typeof(double));
            DataColumn CumPNLrollHoldColumn = new DataColumn("CumPNLrollHold", typeof(double));
            DataColumn RealPNLrollHoldColumn = new DataColumn("RealPNLrollHold", typeof(double));
            DataColumn CumRealPNLrollHoldColumn = new DataColumn("CumRealPNLrollHold", typeof(double));

            //add the data column object to the data table
            datatable.Columns.AddRange(new DataColumn[] { dateColumn, symbolColumn, 
                openColumn, highColumn, lowColumn, closeColumn, volumeColumn, adjCloseColumn,
                Returns,rollingAverageColumn,rollingStdColumn,ZscoreStaticColumn,ZscoreRollingColumn,
                PositionsStaticColumn,PNLstaticColumn,CumPNLstaticColumn,RealPNLstaticColumn,CumRealPNLstaticColumn,PositionsStaticHoldColumn,
                PNLstaticHoldColumn,CumPNLstaticHoldColumn,RealPNLstaticHoldColumn,CumRealPNLstaticHoldColumn,PositionsRollColumn,PNLrollColumn,CumPNLrollColumn,RealPNLrollColumn,CumRealPNLrollColumn,PositionsRollHoldColumn,PNLrollHoldColumn,CumPNLrollHoldColumn,RealPNLrollHoldColumn,CumRealPNLrollHoldColumn}); /*PositionsRollColumn,PNLrollColumn});  */

            //add the CSV data files' contents to the data table
            AddMarket(ticker, sourceDirectory);
            //add rolling average to the data table
            AddReturns();
            AddRollingAverage();
            AddRollingStd();
            AddStaticZscore();
            AddRollingZscore();
            //AddPositionsStatic();
            //AddPositionsStaticHold();
            //AddPositionsRoll();
            return datatable;
        }

        //method to insert the data from the local directory
        private void AddMarket(string ticker, string sourceDirectory)
        {
            //location of CSV files
            //string fileDirectory = System.Configuration.ConfigurationManager.AppSettings["FileDirectory"];
            //get the ticker's data from the web
            DataDownload.DownloadData(ticker, sourceDirectory);
            //get the file directory path and set the file name
            string fileName = new DirectoryInfo(sourceDirectory) + ticker + ".csv";

            //in order to skip the first line of the header
            int count = 0;
            //read the current file name
            using (StreamReader reader = new StreamReader(fileName))
            {
                string input;
                //read each line of the file and add to the SortedList
                while ((input = reader.ReadLine()) != null)
                {
                    string[] line = input.Split(',');
                    //make sure to exclude the first line because of header
                    if (count > 0)
                    {
                        DataRow row = datatable.NewRow();
                        row["Date"] = DateTime.Parse(line[0]);
                        row["Symbol"] = ticker;
                        row["Open"] = double.Parse(line[1]);
                        row["High"] = double.Parse(line[2]);
                        row["Low"] = double.Parse(line[3]);
                        row["Close"] = double.Parse(line[4]);
                        row["Volume"] = double.Parse(line[5]);
                        row["AdjustedClose"] = double.Parse(line[6]);
                        datatable.Rows.Add(row);
                    }
                    count++;
                }
            }
        }

        //method to add the rolling average to the data table
        public void AddRollingAverage()
        {
            
            SortedList<DateTime, double> sl = MathFunctions.AverageRolling(datatable, rollingDays);

            foreach (DataRow dr in datatable.Rows)
            {
                if (sl.ContainsKey((DateTime)(dr["Date"])))
                    dr["RollingAverage"] = sl[(DateTime)(dr["Date"])];
            }
        }

        public void AddRollingStd()
        {
            
            SortedList<DateTime, double> sl = MathFunctions.StdDevRolling(datatable, rollingDays);

            foreach (DataRow dr in datatable.Rows)
            {
                if (sl.ContainsKey((DateTime)(dr["Date"])))
                    dr["RollingStd"] = sl[(DateTime)(dr["Date"])];
            }
        }

        private void AddStaticZscore()
        {

            SortedList<DateTime, double> sl = MathFunctions.ZscoreStatic(datatable);

            foreach (DataRow dr in datatable.Rows)
            {
                if (sl.ContainsKey((DateTime)(dr["Date"])))
                    dr["ZscoreStatic"] = sl[(DateTime)(dr["Date"])];
            }
        }

        private void AddReturns()
        {

            SortedList<DateTime, double> sl = MathFunctions.Returns(datatable);

            foreach (DataRow dr in datatable.Rows)
            {
                if (sl.ContainsKey((DateTime)(dr["Date"])))
                    dr["Returns"] = sl[(DateTime)(dr["Date"])];
            }
        }

        private void AddRollingZscore()
        {
            //number of days to calculate the rolling average across
            //int days = 30;
            SortedList<DateTime, double> sl = MathFunctions.ZscoreRolling(datatable, rollingDays);

            foreach (DataRow dr in datatable.Rows)
            {
                if (sl.ContainsKey((DateTime)(dr["Date"])))
                    dr["ZscoreRolling"] = sl[(DateTime)(dr["Date"])];
            }
        }

        //property for the data table
        public DataTable DataTable
        {
            get { return datatable; }
        }

        public int HoldDays
        {
            set { holdDays = value; }
        }

        public int RollingDays
        {
            set { rollingDays = value; }
        }

        public double Zcut
        {
            set { zcut = value; }
        }
    }
}
