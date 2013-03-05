using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Microsoft.SolverFoundation.Samples
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Data;
    using System.Drawing;
    using System.Text;
    using System.Windows.Forms;

    public partial class Form1 : Form
    {
        Portfolio portfolio;        

        public Form1()
        {
            InitializeComponent();
            Control.CheckForIllegalCrossThreadCalls = false;
            
            portfolio = new Portfolio();
            portfolio.SolveLogUpdated += new EventHandler<SolveLogUpdatedEventArgs>(portfolio_SolveLogUpdated);
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            
            portfolio.MinROI = 0.03;
            gridSourceData.DefaultCellStyle.Format = "F4";
            this.gridSourceData.DataSource = portfolio.BuildTabularData();
            btnSolve.Enabled = true;

            this.textBox2.Text = "";
            this.textBox3.Text = "";
        }        

        void portfolio_SolveLogUpdated(object sender, SolveLogUpdatedEventArgs e)
        {   
           textBox2.AppendText(e.NewLog);            
        }

        private void btnSolve_Click(object sender, EventArgs e)
        {
            try
            {
              IFormatProvider provider = System.Globalization.CultureInfo.CurrentCulture;
                if (textBox1.Text != String.Empty)
                    portfolio.MinROI = Convert.ToDouble(textBox1.Text, provider) / 100;
            }
            catch (FormatException)
            {
                portfolio.MinROI = 0.03;
            }
            this.textBox2.Text = "";
            this.textBox3.Text = "";

            string report = portfolio.Solve();
            this.gridSourceData.Refresh();
            this.gridSourceData.DataSource = portfolio.BuildTabularData();
            
            this.textBox3.Text = report;
        }




        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }

        private void gridSourceData_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
        
    }
}
