using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using Data;
using System.Diagnostics;
using System.IO;
using System.Web.UI.DataVisualization.Charting;
using System.Data.Odbc;
using System.Data.SqlClient;
using Microsoft.Office.Core;
using System.Runtime.InteropServices;



namespace Test
{
    public partial class Form1 : Form
    {        
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string sourcePath = System.Configuration.ConfigurationManager.AppSettings["SourceDirectory"];                    
            string destPath = System.Configuration.ConfigurationManager.AppSettings["DestDirectory"];                                            
            string ticker = textBox1.Text;
            int holdDays = 0;
            double cutoff = 0.00;
            int rollingDays = 0;
            try
            {
                holdDays = int.Parse(textBox3.Text);
                cutoff = double.Parse(textBox4.Text);
                rollingDays = int.Parse(textBox5.Text);
            }
            catch
            {

                MessageBox.Show("Inputs are in the wrong format - Try again!");
                //Application.Run(new Test.Form1());
                Application.ExitThread();
                Application.Exit();
                Application.Restart();
                Environment.Exit(0);
            }
                        
            Cursor = Cursors.WaitCursor;
            //instantiate a new back test for the symbol
            BackTest test = new BackTest(ticker, sourcePath, cutoff);
            test.BackTestLongShort(holdDays, rollingDays);            

            //bind data to the data grid
            BindingSource bSource = new BindingSource();
            bSource.DataSource = test.DT;
            dataGridView1.DataSource = bSource;

            DataRow last = test.DT.Rows[test.DT.Rows.Count - 1];
            double zroll = Convert.ToDouble(last["ZscoreRolling"]);
            textBox6.Text = Convert.ToString(Math.Round((zroll * 1), 4));
            double zstatic = Convert.ToDouble(last["ZscoreStatic"]);
            textBox2.Text = Convert.ToString(Math.Round((zstatic * 1), 4));
            textBox7.Text = Convert.ToString(Math.Round((cutoff * 1), 4));
            textBox8.Text = Convert.ToString(Math.Round((MathFunctions.ZscoreRollCount(test.DT) * 1), 4));
            textBox9.Text = Convert.ToString(Math.Round((MathFunctions.ZscoreStaticCount(test.DT) * 1), 4));
            textBox10.Text = Convert.ToString(Math.Round((MathFunctions.AverageAllPNLrollHold(test.DT) * 26000), 4));
            double RealCum = Convert.ToDouble(last["CumRealPNLrollHold"]);
            textBox11.Text = Convert.ToString(Math.Round((RealCum * 100), 4));
            textBox12.Text = Convert.ToString(Math.Round((MathFunctions.ZscoreRollCount(test.DT) / holdDays), 0));
            if (Math.Round((MathFunctions.AverageAllPNLrollHold(test.DT) * 26000), 4) > 0) textBox13.Text = Convert.ToString(Math.Abs(Math.Round((((MathFunctions.CountPNLrollHold(test.DT) / holdDays) / (MathFunctions.ZscoreRollCount(test.DT) / holdDays))) * 100, 4)));
            if (Math.Round((MathFunctions.AverageAllPNLrollHold(test.DT) * 26000), 4) < 0) textBox13.Text = Convert.ToString(-1 * Math.Abs(Math.Round((((MathFunctions.CountPNLrollHold(test.DT) / holdDays) / (MathFunctions.ZscoreRollCount(test.DT) / holdDays))) * 100, 4)));
            textBox14.Text = Convert.ToString(Math.Round((MathFunctions.SharpePNLrollHold(test.DT)), 4));
            textBox15.Text = Convert.ToString(Math.Round((MathFunctions.StdDevAllPNLrollHold(test.DT) * Math.Sqrt(260) * 100), 4));
            double aclose = Convert.ToDouble(last["AdjustedClose"]);
            textBox16.Text = Convert.ToString(Math.Round((aclose * 1), 2));

            //double zroll = Convert.ToDouble(last["ZscoreRolling"]);
            textBox28.Text = Convert.ToString(Math.Round((zroll * 1), 4));
            //double zstatic = Convert.ToDouble(last["ZscoreStatic"]);
            textBox27.Text = Convert.ToString(Math.Round((zstatic * 1), 4));
            textBox26.Text = Convert.ToString(Math.Round((cutoff * 1), 4));
            textBox25.Text = Convert.ToString(Math.Round((MathFunctions.ZscoreRollCount(test.DT) * 1), 4));
            textBox24.Text = Convert.ToString(Math.Round((MathFunctions.ZscoreStaticCount(test.DT) * 1), 4));
            textBox23.Text = Convert.ToString(Math.Round((MathFunctions.AverageAllPNLrollHold(test.DT) * 26000 * -1), 4));
            //double RealCum = Convert.ToDouble(last["CumRealPNLrollHold"]);
            textBox22.Text = Convert.ToString(Math.Round((RealCum * -100), 4));
            textBox21.Text = Convert.ToString(Math.Round((MathFunctions.ZscoreRollCount(test.DT) / holdDays), 0));
            if (Math.Round((MathFunctions.AverageAllPNLrollHold(test.DT) * 26000 * -1), 4) > 0) textBox20.Text = Convert.ToString(Math.Abs(Math.Round((((MathFunctions.CountPNLrollHold(test.DT) / holdDays) / (MathFunctions.ZscoreRollCount(test.DT) / holdDays))) * 100, 4)));
            if (Math.Round((MathFunctions.AverageAllPNLrollHold(test.DT) * 26000 * -1), 4) < 0) textBox20.Text = Convert.ToString(-1 * Math.Abs(Math.Round((((MathFunctions.CountPNLrollHold(test.DT) / holdDays) / (MathFunctions.ZscoreRollCount(test.DT) / holdDays))) * 100, 4)));
            textBox19.Text = Convert.ToString(Math.Round((MathFunctions.SharpePNLrollHold(test.DT) * -1), 4));
            textBox18.Text = Convert.ToString(Math.Round((MathFunctions.StdDevAllPNLrollHold(test.DT) * Math.Sqrt(260) * 100), 4));
            //double aclose = Convert.ToDouble(last["AdjustedClose"]);
            textBox17.Text = Convert.ToString(Math.Round((aclose * 1), 2));
                                                            
            /*
             * bind data to a chart using LINQ and the
             * data table property for the BackTest data table instance
             */
            chart1.Series[0].Points.Clear();
            var d = from data in test.DT.AsEnumerable()
                    select new
                    {
                        Date = data.Field<DateTime>("Date"),
                        ZscoreRolling = data.Field<Object>("ZscoreRolling")
                    };

            foreach (var item in d)
            {
                if (item.Date != null && item.ZscoreRolling != null)
                    chart1.Series["Series1"].Points.AddXY(item.Date.Date.ToString("d"), item.ZscoreRolling);
            }
            chart1.DataBind();

            chart2.Series[0].Points.Clear();
            chart2.Series[1].Points.Clear();
            var dd = from data in test.DT.AsEnumerable()
                    select new
                    {
                        Date = data.Field<DateTime>("Date"),
                        AdjustedClose = data.Field<Object>("AdjustedClose"),
                        RollingAverage = data.Field<Double?>("RollingAverage")
                    };

            foreach (var item in dd)
            {
                if (item.Date != null && item.AdjustedClose != null /*&& item.RollingAverage != null*/)
                {
                    chart2.Series["Series1"].Points.AddXY(item.Date.Date.ToString("d"), item.AdjustedClose);
                    chart2.Series["Series2"].Points.AddXY(item.Date.Date.ToString("d"), item.RollingAverage);
                }
            }
            chart2.DataBind();
            
            chart3.Series[0].Points.Clear();
            var ddd = from data in test.DT.AsEnumerable()
                     select new
                     {
                         Date = data.Field<DateTime>("Date"),
                         CumRealPNLrollHold = data.Field<Object>("CumRealPNLrollHold")
                     };

            foreach (var item in ddd)
            {
                if (item.Date != null && item.CumRealPNLrollHold != null)
                    chart3.Series["Series1"].Points.AddXY(item.Date.Date.ToString("d"), item.CumRealPNLrollHold);
            }
            chart3.DataBind();
            //chart3.ChartAreas[0].AxisY.LabelStyle.Format = "{0.00}" + "%"; 
            Cursor = Cursors.Default;

        }
        
        private void button2_Click(object sender, EventArgs e)
        {
            //close the application
            this.Close();
        }



        private void textBox1_TextChanged(object sender, EventArgs e) { }

        private void pictureBox1_Click(object sender, EventArgs e) { }

        private void textBox2_TextChanged(object sender, EventArgs e) { }

        private void textBox3_TextChanged(object sender, EventArgs e) { }
        
        private void marketTableBindingSource_CurrentChanged(object sender, EventArgs e) { }
        
        private void bindingSource1_CurrentChanged(object sender, EventArgs e) { }
        
        private void chart1_Click(object sender, EventArgs e) { }

        private void textBox4_TextChanged(object sender, EventArgs e) { }

        public void textBox5_TextChanged(object sender, EventArgs e) { }

        private void dataGridView1_CellContentClick_1(object sender, DataGridViewCellEventArgs e) { }



        private void button3_Click(object sender, EventArgs e)
        {

            // BRUTE FORCE OPTIMIZE - LOOP THRU ALL COMBINATIONS OF PARAMETERS IN BACKTEST
            string sourcePath = System.Configuration.ConfigurationManager.AppSettings["SourceDirectory"];                    
            string destPath = System.Configuration.ConfigurationManager.AppSettings["DestDirectory"];                                            
            string ticker = textBox1.Text;
            //int[] holdDays = { 3, 5, 8 };
            double[] cutoff = { 1, 2, 3, 4};
            //int[] rollingDays = { 30, 60, 90};
            double lens = Math.Pow(3,(cutoff.Length));
            int len = Convert.ToInt32(lens);
            double[,] table = new double[len, 4];
            int count = 0;
            Cursor = Cursors.WaitCursor;

            Console.WriteLine(lens);
            var geneticsArray = new[] { new[] { 2, 3, 5, 8}, new[] { 1, 2, -1, -2 }, new[] { 10, 20, 40, 60 } };
            var perms = from a in geneticsArray[0]
                        from b in geneticsArray[1]
                        from c in geneticsArray[2]
                        select new { a, b, c };
            try
            {
                foreach (var value in perms)
                {
                    //double result;

                    BackTest test = new BackTest(ticker, sourcePath, value.b);

                    test.BackTestLongShort(value.a, value.c);

                    double sharpe = MathFunctions.SharpePNLrollHold(test.DT);


                    table[count, 0] = sharpe;
                    table[count, 1] = value.a;
                    table[count, 2] = value.b;
                    table[count, 3] = value.c;

                    count = count + 1;

                }
            }
            catch
            {
                MessageBox.Show("Optimization Failed - Check parameters and try again!");
            }

            double max = 0.00;
            int z = 0;
            for (int i = 0; i < len; i++)
            {
                if (table[i, 0] > max) max = table[i, 0];
                if (max == table[i, 0]) z=i;
            }

            BackTest test1 = new BackTest(ticker, sourcePath, table[z,2]);
            test1.BackTestLongShort(Convert.ToInt32(table[z, 1]), Convert.ToInt32(table[z, 3]));

            textBox3.Text = Convert.ToString(table[z, 1]);
            textBox4.Text = Convert.ToString(table[z, 2]);
            textBox5.Text = Convert.ToString(table[z, 3]);

            BindingSource bSource = new BindingSource();
            bSource.DataSource = test1.DT;
            dataGridView1.DataSource = bSource;

            DataRow last = test1.DT.Rows[test1.DT.Rows.Count - 1];
            double zroll = Convert.ToDouble(last["ZscoreRolling"]);
            textBox6.Text = Convert.ToString(Math.Round((zroll * 1), 4));
            double zstatic = Convert.ToDouble(last["ZscoreStatic"]);
            textBox2.Text = Convert.ToString(Math.Round((zstatic * 1), 4));
            textBox7.Text = Convert.ToString(Math.Round((table[z, 2] * 1), 4));
            textBox8.Text = Convert.ToString(Math.Round((MathFunctions.ZscoreRollCount(test1.DT) * 1), 4));
            textBox9.Text = Convert.ToString(Math.Round((MathFunctions.ZscoreStaticCount(test1.DT) * 1), 4));
            textBox10.Text = Convert.ToString(Math.Round((MathFunctions.AverageAllPNLrollHold(test1.DT) * 26000), 4));
            double RealCum = Convert.ToDouble(last["CumRealPNLrollHold"]);
            textBox11.Text = Convert.ToString(Math.Round((RealCum * 100), 4));
            textBox12.Text = Convert.ToString(Math.Round((MathFunctions.ZscoreRollCount(test1.DT) / table[z, 1]), 0));
            if (Math.Round((MathFunctions.AverageAllPNLrollHold(test1.DT) * 26000), 4) > 0) textBox13.Text = Convert.ToString(Math.Abs(Math.Round((((MathFunctions.CountPNLrollHold(test1.DT) / table[z, 1]) / (MathFunctions.ZscoreRollCount(test1.DT) / table[z, 1]))) * 100, 4)));
            if (Math.Round((MathFunctions.AverageAllPNLrollHold(test1.DT) * 26000), 4) < 0) textBox13.Text = Convert.ToString(-1 * Math.Abs(Math.Round((((MathFunctions.CountPNLrollHold(test1.DT) / table[z, 1]) / (MathFunctions.ZscoreRollCount(test1.DT) / table[z, 1]))) * 100, 4)));
            textBox14.Text = Convert.ToString(Math.Round((MathFunctions.SharpePNLrollHold(test1.DT)), 4));
            textBox15.Text = Convert.ToString(Math.Round((MathFunctions.StdDevAllPNLrollHold(test1.DT) * Math.Sqrt(260) * 100), 4));
            double aclose = Convert.ToDouble(last["AdjustedClose"]);
            textBox16.Text = Convert.ToString(Math.Round((aclose * 1), 2));

            //double zroll = Convert.ToDouble(last["ZscoreRolling"]);
            textBox28.Text = Convert.ToString(Math.Round((zroll * 1), 4));
            //double zstatic = Convert.ToDouble(last["ZscoreStatic"]);
            textBox27.Text = Convert.ToString(Math.Round((zstatic * 1), 4));
            textBox26.Text = Convert.ToString(Math.Round((table[z, 2] * 1), 4));
            textBox25.Text = Convert.ToString(Math.Round((MathFunctions.ZscoreRollCount(test1.DT) * 1), 4));
            textBox24.Text = Convert.ToString(Math.Round((MathFunctions.ZscoreStaticCount(test1.DT) * 1), 4));
            textBox23.Text = Convert.ToString(Math.Round((MathFunctions.AverageAllPNLrollHold(test1.DT) * 26000 * -1), 4));
            //double RealCum = Convert.ToDouble(last["CumRealPNLrollHold"]);
            textBox22.Text = Convert.ToString(Math.Round((RealCum * -100), 4));
            textBox21.Text = Convert.ToString(Math.Round((MathFunctions.ZscoreRollCount(test1.DT) / table[z, 1]), 0));
            if (Math.Round((MathFunctions.AverageAllPNLrollHold(test1.DT) * 26000 * -1), 4) > 0) textBox20.Text = Convert.ToString(Math.Abs(Math.Round((((MathFunctions.CountPNLrollHold(test1.DT) / table[z, 1]) / (MathFunctions.ZscoreRollCount(test1.DT) / table[z, 1]))) * 100, 4)));
            if (Math.Round((MathFunctions.AverageAllPNLrollHold(test1.DT) * 26000 * -1), 4) < 0) textBox20.Text = Convert.ToString(-1 * Math.Abs(Math.Round((((MathFunctions.CountPNLrollHold(test1.DT) / table[z, 1]) / (MathFunctions.ZscoreRollCount(test1.DT) / table[z, 1]))) * 100, 4)));
            textBox19.Text = Convert.ToString(Math.Round((MathFunctions.SharpePNLrollHold(test1.DT) * -1), 4));
            textBox18.Text = Convert.ToString(Math.Round((MathFunctions.StdDevAllPNLrollHold(test1.DT) * Math.Sqrt(260) * 100), 4));
            //double aclose = Convert.ToDouble(last["AdjustedClose"]);
            textBox17.Text = Convert.ToString(Math.Round((aclose * 1), 2));
            
            /*
             * bind data to a chart using LINQ and the
             * data table property for the BackTest data table instance
             */
            chart1.Series[0].Points.Clear();
            var d = from data in test1.DT.AsEnumerable()
                    select new
                    {
                        Date = data.Field<DateTime>("Date"),
                        ZscoreRolling = data.Field<Object>("ZscoreRolling")
                    };

            foreach (var item in d)
            {
                if (item.Date != null && item.ZscoreRolling != null)
                    chart1.Series["Series1"].Points.AddXY(item.Date.Date.ToString("d"), item.ZscoreRolling);
            }
            chart1.DataBind();

            chart2.Series[0].Points.Clear();
            var dd = from data in test1.DT.AsEnumerable()
                     select new
                     {
                         Date = data.Field<DateTime>("Date"),
                         AdjustedClose = data.Field<Object>("AdjustedClose")
                     };

            foreach (var item in dd)
            {
                if (item.Date != null && item.AdjustedClose != null)
                    chart2.Series["Series1"].Points.AddXY(item.Date.Date.ToString("d"), item.AdjustedClose);
            }
            chart2.DataBind();

            chart3.Series[0].Points.Clear();
            var ddd = from data in test1.DT.AsEnumerable()
                      select new
                      {
                          Date = data.Field<DateTime>("Date"),
                          CumRealPNLrollHold = data.Field<Object>("CumRealPNLrollHold")
                      };

            foreach (var item in ddd)
            {
                if (item.Date != null && item.CumRealPNLrollHold != null)
                    chart3.Series["Series1"].Points.AddXY(item.Date.Date.ToString("d"), item.CumRealPNLrollHold);
            }
            chart3.DataBind();
            //chart3.ChartAreas[0].AxisY.LabelStyle.Format = "{0.00}" + "%"; 

            Cursor = Cursors.Default;   
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CopyClipboard();
        }

        private void CopyClipboard()
        {
            DataObject d = dataGridView1.GetClipboardContent();
            Clipboard.SetDataObject(d);
        }

        private void pasteCtrlVToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PasteClipboard();
        }

        /// <summary>
        /// This will be moved to the util class so it can service any paste into a DGV
        /// </summary>
        private void PasteClipboard()
        {
            try
            {
                string s = Clipboard.GetText();
                string[] lines = s.Split('\n');
                int iFail = 0, iRow = dataGridView1.CurrentCell.RowIndex;
                int iCol = dataGridView1.CurrentCell.ColumnIndex;
                DataGridViewCell oCell;
                foreach (string line in lines)
                {
                    if (iRow < dataGridView1.RowCount && line.Length > 0)
                    {
                        string[] sCells = line.Split('\t');
                        for (int i = 0; i < sCells.GetLength(0); ++i)
                        {
                            if (iCol + i < this.dataGridView1.ColumnCount)
                            {
                                oCell = dataGridView1[iCol + i, iRow];
                                if (!oCell.ReadOnly)
                                {
                                    if (oCell.Value.ToString() != sCells[i])
                                    {
                                        oCell.Value = Convert.ChangeType(sCells[i], oCell.ValueType);
                                        oCell.Style.BackColor = Color.Tomato;
                                    }
                                    else
                                        iFail++;//only traps a fail if the data has changed and you are pasting into a read only cell
                                }
                            }
                            else
                            { break; }
                        }
                        iRow++;
                    }
                    else
                    { break; }
                    if (iFail > 0)
                        MessageBox.Show(string.Format("{0} updates failed due to read only column setting", iFail));
                }
            }
            catch (FormatException)
            {
                MessageBox.Show("The data you pasted is in the wrong format for the cell");
                return;
            }
        }

        private void dgData_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.Control && e.KeyCode == Keys.Delete) || (e.Shift && e.KeyCode == Keys.Delete))
            {
                CopyClipboard();
            }
            if ((e.Control && e.KeyCode == Keys.Insert) || (e.Shift && e.KeyCode == Keys.Insert))
            {
                PasteClipboard();
            }

        }

        private void button4_Click(object sender, EventArgs e)
        {
            //initialize the cursor to waiting while updating
            //Cursor = Cursors.WaitCursor;
            Cursor = Cursors.WaitCursor;
            string sourcePath = System.Configuration.ConfigurationManager.AppSettings["SourceDirectory"];
            string destPath = System.Configuration.ConfigurationManager.AppSettings["DestDirectory"];
            string ticker = textBox1.Text;
            int holdDays = int.Parse(textBox3.Text);
            double cutoff = double.Parse(textBox4.Text);
            int rollingDays = int.Parse(textBox5.Text);
            //instantiate a new back test for the symbol
            BackTest test = new BackTest(ticker, sourcePath, cutoff);
            test.BackTestLongShort(holdDays, rollingDays);

            //update the database for the test results via Entity Framework
            try
            {
                using (NetEntities db = new NetEntities())
                {
                    //use a stored procedure to delete all the data in the database that matches the symbol
                    db.DeleteData(ticker);
                }

                string sql = "Select * from Table5";

                SQLConnection Test1 = new SQLConnection();
                Test1.ExecuteSelect(sql, test.DT);
            }
            catch
            {
                Cursor = Cursors.Default;   
            }
            
            Cursor = Cursors.Default;   
                        
        }

        Form secondForm = new HTMLReportEngineTestApp.Form1();
        
        private void button5_Click(object sender, EventArgs e)
        {

            try
            {
                
                secondForm.Show();

                //Application.Run(new HTMLReportEngineTestApp.Form1());
            }
            catch
            {
                MessageBox.Show("Form is already open");

            }
        }

        private void textBox12_TextChanged(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button6_Click(object sender, EventArgs e)
        {
            this.Close();
        }


    }
}

