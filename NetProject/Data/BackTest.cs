using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.IO;

namespace Data
{
    public class BackTest
    {
        //fields
        private DataTable dt;
        private string ticker;
        private string directory;
        private double zcut;
        private double trans;
        
        public BackTest(string symbol, string fileDirectory, double z)
        {
            Ticker = symbol;
            Directory = fileDirectory;
            Zcut = z;
        }

        public DataTable BackTestLongShort(int holdDays, int rollingDays)
        {
            dt = new MarketTable(zcut, holdDays, rollingDays).CreateTable(ticker, directory);
                                    
            sortTable(); //sort the table according to date and in ascending order
            trans = .0008 / holdDays;
            CalcPositionsZscoreStatic();
            CalcPNLstatic();
            CalcRealPNLstatic();
            if (zcut > 0)
                CalcPositionsZscoreStaticHoldShort(holdDays);
            if (zcut < 0)
                CalcPositionsZscoreStaticHoldLong(holdDays);
            CalcPNLstaticHold();
            CalcRealPNLstaticHold();
            CalcPositionsZscoreRoll();
            CalcPNLroll(rollingDays);
            CalcRealPNLroll(rollingDays);
            if (zcut > 0)
                CalcPositionsZscoreRollHoldShort(holdDays);
            if (zcut < 0)
                CalcPositionsZscoreRollHoldLong(holdDays);
            CalcPNLrollHold(rollingDays);
            CalcRealPNLrollHold(rollingDays);

            WriteCSV();

            return dt;
        }

        //method that sorts the table in ascending date order
        private void sortTable()
        {
            DataView view = dt.DefaultView;
            view.Sort = "Date ASC";
            dt = view.ToTable();
        }

        private void CalcPNLstatic()  //Theoretical PnL where we simply take return on every occurence of Z-score breach; used more for indicator pattern-matching stats
        {

            for (int i = 0; i < dt.Rows.Count; i++)
                if (i > 0)
                {
                    dt.Rows[i]["PNLstatic"] = (double)dt.Rows[i]["Returns"] * (double)dt.Rows[i]["PositionsStatic"];

                    if (i == 1)
                        dt.Rows[i]["CumPNLstatic"] = (double)dt.Rows[i]["PNLstatic"];

                    if (!(dt.Rows[i - 1].IsNull("PNLstatic")) && !(dt.Rows[i-1].IsNull("CumPNLstatic")))
                        dt.Rows[i]["CumPNLstatic"] = (double)dt.Rows[i]["PNLstatic"] + (double)dt.Rows[i-1]["CumPNLstatic"];
                }
        }

       
        private void CalcRealPNLstatic()  //Real PnL w/1 day offset so if one were to really trade off of signal
        {

            for (int i = 0; i < dt.Rows.Count; i++)
                if (i > 0)
                {
                    dt.Rows[i]["RealPNLstatic"] = (double)dt.Rows[i]["Returns"] * (double)dt.Rows[i-1]["PositionsStatic"];
                    if ((double)dt.Rows[i]["RealPNLstatic"] != 0 && (double)dt.Rows[i]["PositionsStatic"] == 0) dt.Rows[i]["RealPNLstatic"] = 0;

                    if (i == 1)
                        dt.Rows[i]["CumRealPNLstatic"] = (double)dt.Rows[i]["RealPNLstatic"];

                    if (!(dt.Rows[i - 1].IsNull("RealPNLstatic")) && !(dt.Rows[i - 1].IsNull("CumRealPNLstatic")))
                        dt.Rows[i]["CumRealPNLstatic"] = (double)dt.Rows[i]["RealPNLstatic"] + (double)dt.Rows[i - 1]["CumRealPNLstatic"];
                }
        }

        private void CalcPositionsZscoreRoll()
        {          
            double sell = 1.00;
            double buy = -1.00;
            double flat = 0.00;

            for(int i = 0; i < dt.Rows.Count; i++)
            {
                if (!(dt.Rows[i].IsNull("ZscoreRolling")))
                {
                    if (i > 0 && (double)dt.Rows[i]["ZscoreRolling"] > zcut && zcut > 0)
                    {
                        dt.Rows[i]["PositionsRoll"] = sell;
                    }
                    else if (i > 0 && (double)dt.Rows[i]["ZscoreRolling"] < zcut && zcut < 0) 
                    {
                        dt.Rows[i]["PositionsRoll"] = buy;

                    }
                    else 
                    {
                        dt.Rows[i]["PositionsRoll"] = flat;
                    }                    
                }
            }           
        }

        private void CalcPositionsZscoreStatic()
        {
            double sell = 1.00;
            double buy = -1.00;
            double flat = 0.00;

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                if (!(dt.Rows[i].IsNull("ZscoreStatic")))
                {
                    if (i > 0 && (double)dt.Rows[i]["ZscoreStatic"] > zcut && zcut > 0)
                    {
                        dt.Rows[i]["PositionsStatic"] = sell;
                    }
                    else if (i > 0 && (double)dt.Rows[i]["ZscoreStatic"] < zcut && zcut < 0)
                    {
                        dt.Rows[i]["PositionsStatic"] = buy;

                    }
                    else
                    {
                        dt.Rows[i]["PositionsStatic"] = flat;
                    }
                }
            }
        }

        //short roll hold
        private void CalcPositionsZscoreRollHoldShort(int holdDays)
        {
            double sell = 1.00;
            int flat = 0;
            int rollHoldCount = holdDays;
            bool shortHoldOn = false;


            for (int i = 0; i < dt.Rows.Count; i++)
            {
                if (!(dt.Rows[i].IsNull("ZscoreRolling")))
                {
                    if ((double)dt.Rows[i]["ZscoreRolling"] > zcut)
                    {
                        shortHoldOn = true;
                        rollHoldCount = holdDays;
                    }
                    if (shortHoldOn && rollHoldCount > 0)
                    {
                        dt.Rows[i]["PositionsRollHold"] = sell;
                        rollHoldCount -= 1;
                    }
                    if (rollHoldCount == 0)
                    {
                        rollHoldCount = holdDays;
                        shortHoldOn = false;
                    }
                    if (dt.Rows[i].IsNull("PositionsRollHold"))
                        dt.Rows[i]["PositionsRollHold"] = flat;
                }
            }
        }

        
        //long roll hold
        private void CalcPositionsZscoreRollHoldLong(int holdDays)
        {
            double buy = -1.00;
            int flat = 0;
            int rollHoldCount = holdDays;
            bool longHoldOn = false;


            for (int i = 0; i < dt.Rows.Count; i++)
            {
                if (!(dt.Rows[i].IsNull("ZscoreRolling")))
                {
                    if ((double)dt.Rows[i]["ZscoreRolling"] < zcut)
                    {
                        longHoldOn = true;
                        rollHoldCount = holdDays;
                    }
                    if (longHoldOn && rollHoldCount > 0)
                    {
                        dt.Rows[i]["PositionsRollHold"] = buy;
                        rollHoldCount -= 1;
                    }
                    if (rollHoldCount == 0)
                    {
                        rollHoldCount = holdDays;
                        longHoldOn = false;
                    }
                    if (dt.Rows[i].IsNull("PositionsRollHold"))
                        dt.Rows[i]["PositionsRollHold"] = flat;
                }
            }
        }


        //short roll hold
        private void CalcPositionsZscoreStaticHoldShort(int holdDays)
        {
            double sell = 1.00;
            int flat = 0;
            int rollHoldCount = holdDays;
            bool shortHoldOn = false;


            for (int i = 0; i < dt.Rows.Count; i++)
            {
                if (!(dt.Rows[i].IsNull("ZscoreStatic")))
                {
                    if ((double)dt.Rows[i]["ZscoreStatic"] > zcut)
                    {
                        shortHoldOn = true;
                        rollHoldCount = holdDays;
                    }
                    if (shortHoldOn && rollHoldCount > 0)
                    {
                        dt.Rows[i]["PositionsStaticHold"] = sell;
                        rollHoldCount -= 1;
                    }
                    if (rollHoldCount == 0)
                    {
                        rollHoldCount = holdDays;
                        shortHoldOn = false;
                    }
                    if (dt.Rows[i].IsNull("PositionsStaticHold"))
                        dt.Rows[i]["PositionsStaticHold"] = flat;
                }
            }
        }


        //long roll hold
        private void CalcPositionsZscoreStaticHoldLong(int holdDays)
        {
            double buy = -1.00;
            int flat = 0;
            int rollHoldCount = holdDays;
            bool longHoldOn = false;


            for (int i = 0; i < dt.Rows.Count; i++)
            {
                if (!(dt.Rows[i].IsNull("ZscoreStatic")))
                {
                    if ((double)dt.Rows[i]["ZscoreStatic"] < zcut)
                    {
                        longHoldOn = true;
                        rollHoldCount = holdDays;
                    }
                    if (longHoldOn && rollHoldCount > 0)
                    {
                        dt.Rows[i]["PositionsStaticHold"] = buy;
                        rollHoldCount -= 1;
                    }
                    if (rollHoldCount == 0)
                    {
                        rollHoldCount = holdDays;
                        longHoldOn = false;
                    }
                    if (dt.Rows[i].IsNull("PositionsStaticHold"))
                        dt.Rows[i]["PositionsStaticHold"] = flat;
                }
            }
        }

    
        private void CalcPNLstaticHold() //Theoretical PnL where we simply take return on every occurence of Z-score breach followed by X day holding period; used more for indicator pattern-matching stats
        {

            for (int i = 0; i < dt.Rows.Count; i++)
                if (i > 0)
                {
                    dt.Rows[i]["PNLstaticHold"] = (double)dt.Rows[i]["Returns"] * (double)dt.Rows[i]["PositionsStaticHold"];

                    if (i == 1)
                        dt.Rows[i]["CumPNLstaticHold"] = (double)dt.Rows[i]["PNLstaticHold"];

                    if (!(dt.Rows[i - 1].IsNull("PNLstaticHold")) && !(dt.Rows[i - 1].IsNull("CumPNLstaticHold")))
                        dt.Rows[i]["CumPNLstaticHold"] = (double)dt.Rows[i]["PNLstaticHold"] + (double)dt.Rows[i - 1]["CumPNLstaticHold"];
                }
        }

        private void CalcRealPNLstaticHold() //Real PnL w/1 day offset so if one were to really trade off of signal - assumes we hold trade for X num of days after zscore breach
        {

            for (int i = 0; i < dt.Rows.Count; i++)
                if (i > 0)
                {
                    dt.Rows[i]["RealPNLstaticHold"] = (double)dt.Rows[i]["Returns"] * (double)dt.Rows[i-1]["PositionsStaticHold"];
                    if ((double)dt.Rows[i]["RealPNLstaticHold"] != 0 && (double)dt.Rows[i]["PositionsStaticHold"] == 0) dt.Rows[i]["RealPNLstaticHold"] = 0;

                    if (i == 1)
                        dt.Rows[i]["CumRealPNLstaticHold"] = (double)dt.Rows[i]["RealPNLstaticHold"];

                    if (!(dt.Rows[i - 1].IsNull("RealPNLstaticHold")) && !(dt.Rows[i - 1].IsNull("CumRealPNLstaticHold")))
                        dt.Rows[i]["CumRealPNLstaticHold"] = (double)dt.Rows[i]["RealPNLstaticHold"] + (double)dt.Rows[i - 1]["CumRealPNLstaticHold"];
                }
        }

        private void CalcPNLroll(int rollingDays) //not fully working yet due to null value issue of positions calced from rolling zscore
        {

            for (int i = 0; i < dt.Rows.Count; i++)
                
                    if (i>0 && !(dt.Rows[i].IsNull("PositionsRoll")))
                    {
                        dt.Rows[i]["PNLroll"] = (double)dt.Rows[i]["Returns"] * (double)dt.Rows[i]["PositionsRoll"];

                        if (i == rollingDays-1)
                            dt.Rows[i]["CumPNLroll"] = (double)dt.Rows[i]["PNLroll"];

                        if (!(dt.Rows[i - 1].IsNull("PNLroll")) && !(dt.Rows[i - 1].IsNull("CumPNLroll")))
                            dt.Rows[i]["CumPNLroll"] = (double)dt.Rows[i]["PNLroll"] + (double)dt.Rows[i - 1]["CumPNLroll"];
                    }
        }

        private void CalcRealPNLroll(int rollingDays) //not fully working yet due to null value issue of positions calced from rolling zscore
        {

            for (int i = 0; i < dt.Rows.Count; i++)

                if (i > rollingDays - 1 && !(dt.Rows[i].IsNull("PositionsRoll")))
                    {
                        dt.Rows[i]["RealPNLroll"] = (double)dt.Rows[i]["Returns"] * (double)dt.Rows[i-1]["PositionsRoll"];
                        if ((double)dt.Rows[i]["RealPNLroll"] != 0 && (double)dt.Rows[i]["PositionsRoll"] == 0) dt.Rows[i]["RealPNLroll"] = 0;

                        if (i == rollingDays)
                            dt.Rows[i]["CumRealPNLroll"] = (double)dt.Rows[i]["RealPNLroll"];

                        if (!(dt.Rows[i - 1].IsNull("RealPNLroll")) && !(dt.Rows[i - 1].IsNull("CumRealPNLroll")))
                            dt.Rows[i]["CumRealPNLroll"] = (double)dt.Rows[i]["RealPNLroll"] + (double)dt.Rows[i - 1]["CumRealPNLroll"];
                    }
        }

        private void CalcPNLrollHold(int rollingDays) //not fully working yet due to null value issue of positions calced from rolling zscore
        {

            for (int i = 0; i < dt.Rows.Count; i++)

                if (i > 0 && !(dt.Rows[i].IsNull("PositionsRollHold")))
                {
                    dt.Rows[i]["PNLrollHold"] = (double)dt.Rows[i]["Returns"] * (double)dt.Rows[i]["PositionsRollHold"];

                    if (i == rollingDays - 1)
                        dt.Rows[i]["CumPNLrollHold"] = (double)dt.Rows[i]["PNLrollHold"];

                    if (!(dt.Rows[i - 1].IsNull("PNLrollHold")) && !(dt.Rows[i - 1].IsNull("CumPNLrollHold")))
                        dt.Rows[i]["CumPNLrollHold"] = (double)dt.Rows[i]["PNLrollHold"] + (double)dt.Rows[i - 1]["CumPNLrollHold"];
                }
        }

        private void CalcRealPNLrollHold(int rollingDays) //not fully working yet due to null value issue of positions calced from rolling zscore
        {

            for (int i = 0; i < dt.Rows.Count; i++)

                if (i > rollingDays - 1 && !(dt.Rows[i].IsNull("PositionsRollHold")))
                {
                    dt.Rows[i]["RealPNLrollHold"] = ((double)dt.Rows[i]["Returns"] * (double)dt.Rows[i - 1]["PositionsRollHold"]) - (Math.Abs((double)dt.Rows[i - 1]["PositionsRollHold"] * trans));
                    if ((double)dt.Rows[i]["RealPNLrollHold"] != 0 && (double)dt.Rows[i]["PositionsRollHold"] == 0) dt.Rows[i]["RealPNLrollHold"] = 0;
                       
                    if (i == rollingDays)
                        dt.Rows[i]["CumRealPNLrollHold"] = (double)dt.Rows[i]["RealPNLrollHold"];

                    if (!(dt.Rows[i - 1].IsNull("RealPNLrollHold")) && !(dt.Rows[i - 1].IsNull("CumRealPNLrollHold")))
                        dt.Rows[i]["CumRealPNLrollHold"] = (double)dt.Rows[i]["RealPNLrollHold"] + (double)dt.Rows[i - 1]["CumRealPNLrollHold"];
                }
        }

        //method to write the portfolio table to a CSV file
        private void WriteCSV()
        {
            try
            {                             
                var test = from data in dt.AsEnumerable()                                                  
                           select new
                           {
                               Date = data.Field<DateTime>("Date"),
                               Symbol = data.Field<string>("Symbol"),
                               Open = data.Field<double>("Open"),
                               High = data.Field<double>("High"),
                               Low = data.Field<double>("Low"),
                               Close = data.Field<double>("Close"),
                               Volume = data.Field<double>("Volume"),
                               AdjustedClose = data.Field<double>("AdjustedClose"),
                               Returns = data.Field<Object>("Returns"),
                               RollingAverage = data.Field<Object>("RollingAverage"),
                               RollingStd = data.Field<Object>("RollingStd"),
                               ZscoreStatic = data.Field<Object>("ZscoreStatic"),
                               ZscoreRolling = data.Field<Object>("ZscoreRolling"),
                               PositionsStatic = data.Field<Object>("PositionsStatic"),
                               PNLstatic = data.Field<Object>("PNLstatic"),
                               CumPNLstatic = data.Field<Object>("CumPNLstatic"),
                               RealPNLstatic = data.Field<Object>("RealPNLstatic"),
                               CumRealPNLstatic = data.Field<Object>("CumRealPNLstatic"),
                               PositionsStaticHold = data.Field<Object>("PositionsStaticHold"),
                               PNLstaticHold = data.Field<Object>("PNLstaticHold"),
                               CumPNLstaticHold = data.Field<Object>("CumPNLstaticHold"),
                               RealPNLstaticHold = data.Field<Object>("RealPNLstaticHold"),
                               CumRealPNLstaticHold = data.Field<Object>("CumRealPNLstaticHold"),
                               PositionsRoll = data.Field<Object>("PositionsRoll"),
                               PNLroll = data.Field<Object>("PNLroll"),
                               CumPNLroll = data.Field<Object>("CumPNLroll"),
                               RealPNLroll = data.Field<Object>("RealPNLroll"),
                               CumRealPNLroll = data.Field<Object>("CumRealPNLroll"),
                               PositionsRollHold = data.Field<Object>("PositionsRollHold"),
                               PNLrollHold = data.Field<Object>("PNLrollHold"),
                               CumPNLrollHold = data.Field<Object>("CumPNLrollHold"),
                               RealPNLrollHold = data.Field<Object>("RealPNLrollHold"),
                               CumRealPNLrollHold = data.Field<Object>("CumRealPNLrollHold")
                               //PNLroll = data.Field<Object>("PNLroll") 
                           };
                                
                //write parent table to a file in the destination directory
                string destination = directory + "PortfolioTable" + ".csv";
                using (StreamWriter writer = new StreamWriter(destination, false))
                {
                    int count = 0;
                    foreach (var value in test)
                    {
                        if (count == 0)
                            writer.WriteLine(                                
                            "Date" + "," +
                            "Symbol" + "," +
                            "Open" + "," +
                            "High" + "," +
                            "Low" + "," +
                            "Close" + "," +
                            "Volume" + "," +
                            "AdjustedClose" + "," +
                            "Returns" + "," +
                            "RollingAverage" + "," +
                            "RollingStd" + "," +
                            "ZscoreStatic" + "," +
                            "ZscoreRolling" + "," +
                            "PositionsStatic" + "," +
                            "PNLstatic" + "," +
                            "CumPNLstatic" + "," +
                            "RealPNLstatic" + "," +
                            "CumRealPNLstatic" + "," +
                            "PositionsStaticHold" + "," +
                            "PNLstaticHold" + "," +
                            "CumPNLstaticHold" + "," +
                            "RealPNLstaticHold" + "," +
                            "CumRealPNLstaticHold" + "," +
                            "PositionsRoll" + "," +
                            "PNLroll" + "," +
                            "CumPNLroll" + "," +
                            "RealPNLroll" + "," +
                            "CumRealPNLroll" + "," +
                            "PositionsRollHold" + "," +
                            "PNLrollHold" + "," +
                            "CumPNLrollHold" + "," +
                            "RealPNLrollHold" + "," +
                            "CumRealPNLrollHold"
                                );
                        writer.WriteLine(
                            value.Date + "," +
                            value.Symbol + "," +
                            value.Open + "," +
                            value.High + "," +
                            value.Low + "," +
                            value.Close + "," +
                            value.Volume + "," +
                            value.AdjustedClose + "," +
                            value.Returns + "," +
                            value.RollingAverage + "," +
                            value.RollingStd + "," +
                            value.ZscoreStatic + "," +
                            value.ZscoreRolling + "," +
                            value.PositionsStatic + "," +
                            value.PNLstatic + "," +
                            value.CumPNLstatic + "," +
                            value.RealPNLstatic + "," +
                            value.CumRealPNLstatic + "," +
                            value.PositionsStaticHold + "," +
                            value.PNLstaticHold + "," +
                            value.CumPNLstaticHold + "," +
                            value.RealPNLstaticHold + "," +
                            value.CumRealPNLstaticHold + "," +
                            value.PositionsRoll + "," +
                            value.PNLroll + "," +
                            value.CumPNLroll + "," +
                            value.RealPNLroll + "," +
                            value.CumRealPNLroll + "," +
                            value.PositionsRollHold + "," +
                            value.PNLrollHold + "," +
                            value.CumPNLrollHold + "," +
                            value.RealPNLrollHold + "," +
                            value.CumRealPNLrollHold);
                        count++;
                    }
                }
            }
            catch (Exception)
            {
                Console.WriteLine("Exception source: ");
                throw;
            }
        }

        //properties
        public DataTable DT
        {
            get { return dt; }
        }

        public string Ticker
        {
            set { ticker = value; }
        }

        public string Directory
        {
            set { directory = value; }
        }

        public double Zcut
        {
            set { zcut = value; }            
        }
    }
}