using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace Data
{
    /*
     * class for data download from the web
     */
    public class DataDownload
    {
        /*
         * static method that connects to and downloads the data from Yahoo Finance.
         * The file is saved to the specified directory according to the App.config key
         * *****only change the App.config "value" for wherever the file should go*****
         */
        public static void DownloadData(string ticker, string directory)
        {
            try
            {
                using (WebClient web = new WebClient())
                {
                    web.DownloadFile("http://ichart.finance.yahoo.com/table.csv?s=" + @ticker + "&ignore=.csv", directory + @ticker + ".csv");
                }
            }
            catch
            {
               
                MessageBox.Show("Ticker does not exist - Please enter another ticker");
                //Application.Run(new Test.Form1());
                Application.ExitThread();
                Application.Exit();
                Application.Restart();
                Environment.Exit(0);
              
            }
            
        }        
    }
}
