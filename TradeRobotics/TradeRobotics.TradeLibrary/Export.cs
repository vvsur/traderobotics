using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WealthLab;
using Microsoft.Office.Interop.Excel;
using System.Reflection;
using TradeRobotics.DataProviders.Quik.Dom;
using TradeRobotics.DataProviders.Quik;

namespace TradeRobotics.TradeLibrary
{
    /// <summary>
    /// Sends data to Excel
    /// </summary>
    public class Export
    {
        // Excel limitation - maximum bars on graph
        protected const int maxBars = 32000;

        protected static class Constants
        {
            public static int TimeColumnNumber = 1;
            public static string TimeColumn = "A";

            public static int OpenColumnNumber = 2;
            public static string OpenColumn = "B";

            public static int LowColumnNumber = 3;
            public static string LowColumn = "C";

            public static int HighColumnNumber = 4;
            public static string HighColumn = "D";

            public static int CloseColumnNumber = 5;
            public static string CloseColumn = "E";

            public static int VolumeColumnNumber = 6;
            public static string VolumeColumn = "F";

            public static int EquityColumnNumber = 7;
            public static string EquityColumn = "G";

            public static int DrawdownColumnNumber = 8;
            public static string DrawdownColumn = "H";

            public static int StartRow = 2;



            public static class GraphInfo
            {
                public static int Top = 0;
                public static int Left = 0;
                public static int Width = 1024;
                public static int Height = 768;
            }
        
        }

        protected Bars bars;
        protected IList<Position> positions;
        protected Worksheet workSheet;
        protected double startMoney = 1000;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="bars"></param>
        /// <param name="positions"></param>
        public Export(Bars bars, IList<Position> positions, double startMoney)
        {
            this.positions = positions;
            this.bars = bars;
            this.startMoney = startMoney;
        }

        /// <summary>
        /// Rescale bars to not overflow Excel limit
        /// </summary>
        /// <param name="bars"></param>
        /// <returns></returns>
        protected static int GetScale(Bars bars)
        {
            int interval = 0;
            TimeSpan barsLength = bars.Date[bars.Count - 1] - bars.Date[0];
            if(bars.Count < maxBars)
                interval = 0;
            // 1 minute scale will not overflow Excel
            else if (barsLength.Minutes < maxBars)
                interval = 1;
            // 5 minutes
            else if(barsLength.Minutes / 5 < maxBars)
                interval = 5;
            // 15 minutes
            else if(barsLength.Minutes / 15 < maxBars)
                interval = 15;
            // 30 minutes
            else if(barsLength.Minutes / 30 < maxBars)
                interval = 30;
            // An hour
            else if (barsLength.Minutes / 60 < maxBars)
                interval = 60;
            // 4 hours
            else if (barsLength.Minutes / 240 < maxBars)
                interval = 240;
            // A day
            else if (barsLength.Minutes / 1440 < maxBars)
                interval = 1440;
            return interval;

        }

        /// <summary>
        /// Open excel window
        /// </summary>
        /// <param name="bars"></param>
        /// <param name="positions"></param>
        public void OpenExcel()
        {
            if (bars.Count == 0)
                return;
            // Get graph x unit in minutes to not overflow Excel limitation
            int interval = GetScale(bars);


            Application app = new Application();
            app.Visible = true;
            Workbook workBook = app.Workbooks.Add(Missing.Value);
            workSheet = (Worksheet)workBook.Worksheets.get_Item(1);

            // Export table headers
            ExportHeaders();

            // Prepare compressed bars
            QuikQuote quote = new QuikQuote() { Time = bars.Date[0], Close = bars.Close[0], Symbol = bars.Symbol, Volume = bars.Volume[0] };
            QuikBar currentBar = new QuikBar(quote);

            // Go through all bars
            int count = bars.Close.Count;
            int newBarsCount = 0;
            double equity = startMoney;
            double drawdown = 0;
            for (int i = 0; i < count; i++)
            {
                // Quote from current tick
                quote = new QuikQuote() { Time = bars.Date[i], Close = bars.Close[i], Symbol = bars.Symbol, Volume = bars.Volume[i] };
                
                // If new bar formed
                if ((bars.Date[i] - currentBar.OpenTime).Minutes >= interval)
                {
                    // Start new bar, export old one
                    newBarsCount++;
                    int rowNumber = Constants.StartRow + newBarsCount - 1;
                    ExportBar(currentBar, rowNumber);
                    // Export Equity and Drawdown
                    ExportEquity(currentBar, rowNumber, ref equity, ref drawdown);

                    // Create new currentBar
                    currentBar = new QuikBar(quote);
                }
                // Update current bar
                currentBar.Update(quote);
            }

            // Draw charts in excel
            DrawCharts(newBarsCount);
        }

        /// <summary>
        /// Write table headers
        /// </summary>
        /// <param name="workSheet"></param>
        protected void ExportHeaders()
        {
            workSheet.Cells[1, Constants.TimeColumnNumber] = "Time";
            workSheet.Cells[1, Constants.OpenColumnNumber] = "Open";
            workSheet.Cells[1, Constants.LowColumnNumber] = "Low";
            workSheet.Cells[1, Constants.HighColumnNumber] = "High";
            workSheet.Cells[1, Constants.CloseColumnNumber] = "Close";
            workSheet.Cells[1, Constants.VolumeColumnNumber] = "Volume";

        }
        
        /// <summary>
        /// Write one bar values to excel
        /// </summary>
        /// <param name="bar"></param>
        protected void ExportBar(QuikBar bar, int rowNumber)
        {
            const string timeFormat = "dd/MM/yyyy hh:mm:ss:fff";
            //Price and volume columns
            workSheet.Cells[rowNumber, Constants.TimeColumnNumber] = bar.CloseTime.ToString(timeFormat);
            workSheet.Cells[rowNumber, Constants.OpenColumnNumber] = bar.Open;
            workSheet.Cells[rowNumber, Constants.LowColumnNumber] = bar.Low;
            workSheet.Cells[rowNumber, Constants.HighColumnNumber] = bar.High;
            workSheet.Cells[rowNumber, Constants.CloseColumnNumber] = bar.Close;
            workSheet.Cells[rowNumber, Constants.VolumeColumnNumber] = bar.Volume;

        }

        /// <summary>
        /// Write equity and drawdown to Excel
        /// </summary>
        /// <param name="bar"></param>
        /// <param name="rowNumber"></param>
        /// <param name="profit"></param>
        /// <param name="equity"></param>
        protected void ExportEquity(QuikBar bar, int rowNumber, ref double equity, ref double drawdown)
        {
            // Calculate
            CalculateEquity(bar, rowNumber, ref equity, ref drawdown);
            // Export to Excel row
            workSheet.Cells[rowNumber, Constants.EquityColumnNumber] = equity;
            workSheet.Cells[rowNumber, Constants.EquityColumnNumber] = drawdown;
        }

        /// <summary>
        /// Calculates profit and equity in ref variables
        /// </summary>
        /// <param name="bar"></param>
        /// <param name="rowNumber"></param>
        /// <param name="profit"></param>
        /// <param name="equity"></param>
        protected void CalculateEquity(QuikBar bar, int rowNumber, ref double equity, ref double drawdown)
        {
            
            // List of moments where account changed
            List<DateTime> changeTimes = new List<DateTime>();

            
            // Positions, being opened during bar
            List<Position> openingPositions = (from curPos in positions
                                             where curPos.EntryDate >= bar.OpenTime && curPos.EntryDate <= bar.CloseTime
                                             select curPos).ToList();

            // Positions, beding closed during bar
            List<Position> closingPositions = (from curPos in positions
                                             where curPos.ExitDate >= bar.OpenTime && curPos.ExitDate <= bar.CloseTime
                                             select curPos).ToList();
            // Positions, opened before and closed after bar
            List<Position> activePositions =  (from curPos in positions
                                             where curPos.ExitDate > bar.CloseTime && curPos.ExitDate < bar.OpenTime
                                             select curPos).ToList();

            
            // Calculate profit from active positions
            CalculateEquity(bar, activePositions, ref equity, ref drawdown);


            // Calculate opened positions
            changeTimes.Add(bar.OpenTime);
            openingPositions.ForEach(curPos => { changeTimes.Add(curPos.EntryDate); });
            closingPositions.ForEach(curPos => { changeTimes.Add(curPos.EntryDate);});
            changeTimes.Add(bar.CloseTime);
            foreach (DateTime changeTime in changeTimes)
            {
                // Add position, opened at the moment
                //var curPositions = openingPositions.Select(curPos => (curPos.EntryDate >= changeTime)).ToList();
                List<Position> curPositions1 = (from curPos1 in openingPositions
                              where curPos1.EntryDate >= changeTime
                              select curPos1).ToList();
                List<Position> curPositions2 = (from curPos1 in openingPositions
                                                where curPos1.ExitDate < changeTime
                                                select curPos1).ToList();
                List<Position> curPositions = new List<Position>(curPositions1);
                curPositions.AddRange(curPositions);
                // Equity calculation
                CalculateEquity(bar, curPositions, ref equity, ref drawdown);
            }
        }

        /// <summary>
        /// Calculate portfolio parameters for bar from positions
        /// </summary>
        /// <param name="bar"></param>
        /// <param name="positions"></param>
        /// <param name="profit"></param>
        /// <param name="drawdown"></param>
        protected void CalculateEquity(QuikBar bar, List<Position> positions, ref double equity, ref double drawdown)
        {
            // Calculate profit
            foreach (Position pos in positions)
            {
                equity += (pos.PositionType == PositionType.Long) ? bar.High - pos.EntryPrice : pos.EntryPrice - bar.Low;
                drawdown += (pos.PositionType == PositionType.Long) ? pos.EntryPrice - bar.Low : bar.High - pos.EntryPrice;
            }
        }

        /// <summary>
        /// Draw Excel graph
        /// </summary>
        /// <param name="workSheet"></param>
        protected void DrawCharts(int rowCount)
        {
            //Add a chart
            ChartObjects xlCharts = (ChartObjects)workSheet.ChartObjects(Type.Missing);
            ChartObject xlChart = (ChartObject)xlCharts.Add(Constants.GraphInfo.Left, Constants.GraphInfo.Top, Constants.GraphInfo.Width, Constants.GraphInfo.Height);
            Chart chartPage = xlChart.Chart;
            chartPage.ChartType = XlChartType.xlLine;

            //// Price series
            //Series priceSeries = ((SeriesCollection)chartPage.SeriesCollection(Type.Missing)).NewSeries();
            //priceSeries.XValues = workSheet.get_Range(Constants.TimeColumn + Constants.StartRow, Constants.TimeColumn + rowCount);
            //priceSeries.Values = workSheet.get_Range(Constants.CloseColumn + Constants.StartRow, Constants.CloseColumn + rowCount);
            //priceSeries.Name = "Price";

            // Drawdown series
            Series drawdownSeries = ((SeriesCollection)chartPage.SeriesCollection(Type.Missing)).NewSeries();
            drawdownSeries.XValues = workSheet.get_Range(Constants.TimeColumn + Constants.StartRow, Constants.TimeColumn + rowCount);
            drawdownSeries.Values = workSheet.get_Range(Constants.DrawdownColumn + Constants.StartRow, Constants.DrawdownColumn + rowCount);
            drawdownSeries.Name = "Drawdown";

            // Equity series
            Series equitySeries = ((SeriesCollection)chartPage.SeriesCollection(Type.Missing)).NewSeries();
            equitySeries.XValues = workSheet.get_Range(Constants.TimeColumn + Constants.StartRow, Constants.TimeColumn + rowCount);
            equitySeries.Values = workSheet.get_Range(Constants.EquityColumn + Constants.StartRow, Constants.EquityColumn + rowCount);
            equitySeries.Name = "Equity";

        }

    }
}
