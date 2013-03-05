using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using Data;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;


namespace Test
{
    public class Program
    {
        [STAThread]
        static void Main(string[] args)
        {

            //string sourcePath = System.Configuration.ConfigurationManager.AppSettings["SourceDirectory"];
            //MarketTable dt = new MarketTable(2, 1, 20);
            //dt.CreateTable("GOOG", sourcePath);
            //double sharpe = MathFunctions.AverageAllPNLrollHold(dt.DataTable);
            //Console.WriteLine(sharpe);


            //Console.ReadLine();



            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());

        }
    }
}
