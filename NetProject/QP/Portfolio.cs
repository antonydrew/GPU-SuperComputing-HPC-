/*==============================================================================
// Copyright © Microsoft Corporation.  All Rights Reserved.
// This code released under the terms of the 
// Microsoft Public License (MS-PL, http://opensource.org/licenses/ms-pl.html.)
==============================================================================*/

// -------------------------------------------------------------------------------------------------------------
// Problem Type: Quadratic programming
//
// Problem Description:
//
// This file implements a Markowitz portfolio optimization sample.
// -------------------------------------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.IO;
using System.Windows.Forms;
using Microsoft.SolverFoundation.Services;
using System.Threading;

namespace Microsoft.SolverFoundation.Samples
{
    class StockPerformance
    {
        public string Stock { get; set; }
        public const int NumMonths = 12;
        public double Jan { get; set; }
        public double Feb { get; set; }
        public double Mar { get; set; }
        public double Apr { get; set; }
        public double May { get; set; }
        public double Jun { get; set; }
        public double Jul { get; set; }
        public double Aug { get; set; }
        public double Sep { get; set; }
        public double Oct { get; set; }
        public double Nov { get; set; }
        public double Dec { get; set; }
        private double m_dMean;

        public double Mean
        {
            get
            {
                return m_dMean;
            }
        }

        public Double SumOfSquares
        {
            get
            {
                return Jan * Jan + Feb * Feb + Mar * Mar + Apr * Apr + May * May + Jun * Jun + Jul * Jul + Aug * Aug + Sep * Sep + Oct * Oct + Nov * Nov + Dec * Dec;
            }
        }

        public double Allocation { get; set; }

        protected StockPerformance()
        {

        }

        public StockPerformance(string strName, double dJan, double dFeb, double dMar, double dApr, double dMay, double dJun, double dJul, double dAug, double dSep, double dOct, double dNov, double dDec)
        {
            Stock = strName;
            Jan = dJan;
            Feb = dFeb;
            Mar = dMar;
            Apr = dApr;
            May = dMay;
            Jun = dJun;
            Jul = dJul;
            Aug = dAug;
            Sep = dSep;
            Oct = dOct;
            Nov = dNov;
            Dec = dDec;
            m_dMean = (Jan + Feb + Mar + Apr + May + Jun + Jul + Aug + Sep + Oct + Nov + Dec) / NumMonths;
        }
    }

    class Variant
    {
        public string StockI { get; set; }
        public string StockJ { get; set; }
        public double Variance { get; set; }
    }

    class Portfolio
    {
        /// <summary> Historical data for the 6 stocks in the sample
        /// </summary>
        private StockPerformance[] _historicalData = new StockPerformance[] {
            new StockPerformance("AlecCorp", 0.2224,0.1616,0.0527,0.1546,0.2062,-0.0042,-0.0034,0.1779,0.0580,0.1237,0.1131,0.1650),
            new StockPerformance("BobInc", 0.0964,0.0706,0.0768,0.0826,0.0855,0.0826,0.0661,0.0771,0.0845,0.0661,0.0494,0.0684),
            new StockPerformance("CugarCo", 0.1008,0.0816,0.0846,0.0918,0.0926,0.0906,0.0725,0.0806,0.0931,0.0734,0.0571,0.0741),
            new StockPerformance("DoubleC", 0.0620,-0.0350,-0.0250,0.3210,0.1530,0.0478,0.0382,0.0496,-0.0275,0.2568,-0.0245,0.1224),
            new StockPerformance("EmergingInc", 0.0475,0.0563,0.0835,0.0921,0.0610,0.0370,0.0296,0.0380,0.0919,0.0737,0.0394,0.0488),
            new StockPerformance("Fabrikam", 0.1058,0.0670,0.0545,0.1484,0.1197,0.0508,0.0406,0.0847,0.0600,0.1187,0.0469,0.0957)
        };

        public IQueryable<StockPerformance> StocksHistory
        {
            get
            {
                return _historicalData.AsQueryable();
            }
        }

        //Covariant matrix
        private List<Variant> Variants;

        public IQueryable<Variant> Covariants
        {
            get
            {
                return Variants.AsQueryable<Variant>();
            }
        }

        public double MinROI
        {
            get;
            set;
        }

        public Portfolio()
        {
            BuildCovariants();
        }

        /// <summary> 
        /// The Markowitz model uses a table of regression coefficients
        /// which are the quadratic section of the model
        /// </summary>
        private void BuildCovariants()
        {
            int iNumStocks = _historicalData.Length;
            Variants = new List<Variant>();
            for (int i = 0; i < iNumStocks; i++)
            {
                for (int j = 0; j < iNumStocks; j++)
                {
                    Variant v = new Variant();
                    v.StockI = _historicalData[i].Stock;
                    v.StockJ = _historicalData[j].Stock;
                    v.Variance = SumOfSquares(_historicalData[i], _historicalData[j]) / StockPerformance.NumMonths - _historicalData[i].Mean * _historicalData[j].Mean;
                    Variants.Add(v);
                }
            }
        }

        public double SumOfSquares(StockPerformance stock1, StockPerformance stock2)
        {
            return stock1.Jan * stock2.Jan + stock1.Feb * stock2.Feb + stock1.Mar * stock2.Mar + stock1.Apr * stock2.Apr
                + stock1.May * stock2.May + stock1.Jun * stock2.Jun + stock1.Jul * stock2.Jul + stock1.Aug * stock2.Aug
                + stock1.Sep * stock2.Sep + stock1.Oct * stock2.Oct + stock1.Nov * stock2.Nov + stock1.Dec * stock2.Dec;
        }

        //This is only for databinding to grid control
        public DataTable BuildTabularData()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Stock");
            dt.Columns.Add("Mean");
            dt.Columns.Add("Allocation");
            dt.Columns.Add("Jan");
            dt.Columns.Add("Feb");
            dt.Columns.Add("Mar");
            dt.Columns.Add("apr");
            dt.Columns.Add("May");
            dt.Columns.Add("Jun");
            dt.Columns.Add("Jul");
            dt.Columns.Add("Aug");
            dt.Columns.Add("Sep");
            dt.Columns.Add("Oct");
            dt.Columns.Add("Nov");
            dt.Columns.Add("Dec");

            for (int i = 0; i < _historicalData.Length; i++)
            {
                DataRow dr = dt.NewRow();
                dr["Stock"] = _historicalData[i].Stock;
                dr["Mean"] = _historicalData[i].Mean;
                dr["Allocation"] = _historicalData[i].Allocation;
                dr["Jan"] = _historicalData[i].Jan;
                dr["Feb"] = _historicalData[i].Feb;
                dr["Mar"] = _historicalData[i].Mar;
                dr["Apr"] = _historicalData[i].Apr;
                dr["May"] = _historicalData[i].May;
                dr["Jun"] = _historicalData[i].Jun;
                dr["Jul"] = _historicalData[i].Jul;
                dr["Aug"] = _historicalData[i].Aug;
                dr["Sep"] = _historicalData[i].Sep;
                dr["Oct"] = _historicalData[i].Oct;
                dr["Nov"] = _historicalData[i].Nov;
                dr["Dec"] = _historicalData[i].Dec;

                dt.Rows.Add(dr);
            }

            return dt;
        }

        public string Solve()
        {
            /***************************
            /*Construction of the model*
            /***************************/
            SolverContext context = SolverContext.GetContext();
            //For repeating the solution with other minimum returns
            context.ClearModel();

            //Create an empty model from context
            Model portfolio = context.CreateModel();

            //Create a string set with stock names
            Set setStocks = new Set(Domain.Any, "Stocks");

            /****Decisions*****/

            //Create decisions bound to the set. There will be as many decisions as there are values in the set
            Decision allocations = new Decision(Domain.RealNonnegative, "Allocations", setStocks);
            allocations.SetBinding(StocksHistory, "Allocation", "Stock");
            portfolio.AddDecision(allocations);

            /***Parameters***/

            //Create parameters bound to Covariant matrix
            Parameter pCovariants = new Parameter(Domain.Real, "Covariants", setStocks, setStocks);
            pCovariants.SetBinding(Covariants, "Variance", "StockI", "StockJ");

            //Create parameters bound to mean performance of the stocks over 12 month period
            Parameter pMeans = new Parameter(Domain.Real, "Means", setStocks);
            pMeans.SetBinding(StocksHistory, "Mean", "Stock");

            portfolio.AddParameters(pCovariants, pMeans);

            /***Constraints***/

            //Portion of a stock should be between 0 and 1
            portfolio.AddConstraint("portion", Model.ForEach(setStocks, stock => 0 <= allocations[stock] <= 1));

            //Sum of all allocations should be equal to unity
            portfolio.AddConstraint("SumPortions", Model.Sum(Model.ForEach(setStocks, stock => allocations[stock])) == 1);

            //Expected minimum return
            portfolio.AddConstraint("ROI", Model.Sum(Model.ForEach(setStocks, stock => Model.Product(allocations[stock], pMeans[stock]))) >= MinROI);

            /***Goals***/

            portfolio.AddGoal("Variance", GoalKind.Minimize, Model.Sum
                                                             (
                                                                Model.ForEach
                                                                (
                                                                    setStocks, stockI =>
                                                                    Model.ForEach
                                                                    (
                                                                        setStocks, stockJ =>
                                                                       Model.Product(pCovariants[stockI, stockJ], allocations[stockI], allocations[stockJ])
                                                                    )
                                                                )
                                                            )
                             );
            /************************************************
           /*Add SolveEvent to watch the solving progress  *
           /************************************************/

            EventHandler<SolvingEventArgs> handler = new EventHandler<SolvingEventArgs>(SolvingEventHandler);            
            
            // ensure the handler is registered only once when hit the Solve button again.
            context.Solving -= handler;
            context.Solving += handler;           

            /*******************
            /*Solve the model  *
            /*******************/

            //Use IPM algorithm
            Solution solution = context.Solve(new InteriorPointMethodDirective());

            //Save the decisions back to the array
            if (solution.Quality == SolverQuality.Optimal)
                context.PropagateDecisions();

            using (TextWriter tw = new StreamWriter("portfolio.qps"))
            {
                context.SaveModel(FileFormat.MPS, tw);
            }

            Report report = solution.GetReport();
            return report.ToString();
        }

        /// <summary>
        /// This is the event handler for SFS Solving Event.
        /// It prints out the detailed information on the IPM solve process.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void SolvingEventHandler(object sender, SolvingEventArgs e)
        {
            if (e[SolverProperties.SolveState].ToString() == "Init")
                return;

            StringBuilder sb = new StringBuilder();
            if ((Int32)e[SolverProperties.IterationCount] == 1)
            {
                // print the header
                sb.AppendLine("Iteration PrimalObjective DualObjective AbsoluteGap");
                // Gap is null for the iteration 1
                sb.AppendFormat("{0,-9:G2} {1,15:F5} {2,15:F5}{3}", (Int32)e[SolverProperties.IterationCount],
                (double)e[InteriorPointProperties.PrimalObjectiveValue],
                (double)e[InteriorPointProperties.DualObjectiveValue],
                Environment.NewLine);
            }
            else
                sb.AppendFormat("{0,-9:G2} {1,15:F5} {2,15:F5} {3,15:F9}{4}", (Int32)e[SolverProperties.IterationCount],
                (double)e[InteriorPointProperties.PrimalObjectiveValue],
                (double)e[InteriorPointProperties.DualObjectiveValue],
                (double)e[InteriorPointProperties.AbsoluteGap],
                Environment.NewLine);


            // append the new solve log to the textbox
            OnSolveLogUpdated(new SolveLogUpdatedEventArgs(sb.ToString()));
            // this is added for demo purpose
            Thread.Sleep(50);            
        }

        public event EventHandler<SolveLogUpdatedEventArgs> SolveLogUpdated ;

        protected virtual void OnSolveLogUpdated(SolveLogUpdatedEventArgs e)
        {
            if (SolveLogUpdated != null) SolveLogUpdated(this, e);
        }

    }    

    public class SolveLogUpdatedEventArgs : System.EventArgs
    {
        public string NewLog { get; set; }
        public SolveLogUpdatedEventArgs(string text)
        {
            NewLog = text;
        }
    }
}
