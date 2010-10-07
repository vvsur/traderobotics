using System;
using System.Collections.Generic;
using System.Net;
using System.IO;
using System.Globalization;             // CultureInfo
using System.Text.RegularExpressions;
using System.Threading;
using System.Linq;
using FTFHelper;
using System.Diagnostics;	// regular expressions for company name string formatting
using WealthLab;
using TradeRobotics.DataProviders.Quik.Dom;

namespace TradeRobotics.DataProviders.Quik
{
	/// <summary>
	/// Load data from quik 111
	/// </summary>
	public static class QuikQuotesLoader
	{
        private static string historyFilePath = @".\Data\DataSets\{0}.csv";
        private static string historyDirectory = @".\Data\DataSets\";
        private static string historyFileMask = "*.csv";




        public static QuikStreamingProvider StreamingProvider ;

        /// <summary>
        /// Add data to data store
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="data"></param>
        public static void AddQuotesFromDde(object[][] table)
        {
            //object[][] table = XLTable.Cells(data);

            // Parse rows to bars
            foreach (object[] row in table)
            {
                try
                {
                    // Write to WL streaming provider
                    if (StreamingProvider == null)
                        return;

                    // Load row
                    QuikQuote quikQuote = new QuikQuote(row);
                    // Maybe first ticks are zero ticks
                    if (quikQuote.Close == 0)
                        continue;

                    // Update streaming provider
                    Quote q = new Quote();
                    q.Symbol = quikQuote.Symbol;
                    q.TimeStamp = quikQuote.Time;
                    q.Ask = quikQuote.Close;
                    q.Bid = quikQuote.Close ;
                    q.Price = quikQuote.Close;
                    q.Size = quikQuote.Volume;

                    q.Open = q.Price;
                    q.PreviousClose = q.Price;
                    
                    
                    //StreamingProvider.UpdateQuote(q);

                    StreamingProvider.UpdateMiniBar(q,
                        q.Open,
                        (q.Ask != 0) ? q.Ask : q.Price,
                        (q.Bid != 0) ? q.Bid : q.Price);
                    StreamingProvider.Hearbeat(quikQuote.Time);
                }
                // Header rows contains titles, ignore them
                catch (Exception ex)
                {
                    continue;
                }
            }
        }

        public static Dictionary<string, BarsAndDom> HistoryBars = new Dictionary<string, BarsAndDom>();

        /// <summary>
        /// Get Wealth Lab bars from QuikBars
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        public static BarsAndDom GetHistoryBars(string symbol, DataSource ds, DateTime startTime, DateTime endTime)
        {
            // Return bars in ticks data. Will be recalculated by Wealth-lab
//            Bars bars = new Bars(symbol.Trim(new char[] { ' ', '"' }), ds.Scale, ds.BarInterval);
            if (HistoryBars.ContainsKey(symbol))
            {
                HistoryBars.Remove(symbol);
                //return HistoryBars[symbol];

            }
            BarsAndDom bars = new BarsAndDom(symbol.Trim(new char[] { ' ', '"' }), ds.Scale, ds.BarInterval);
            HistoryBars.Add(bars.Symbol, bars);

            // Load from file
            string[] lines = File.ReadAllLines(string.Format(historyFilePath, bars.Symbol));
            int i = 0;
            // Init quik bar
            QuikBar quikBar = null;
            if (lines.Length > 2)
            {
                if (ds.Scale == BarScale.Tick)
                {
                    QuikQuote quikQuote = new QuikQuote(lines[1]);
                    quikBar = new QuikBar(quikQuote);
                }
                else
                {
                    quikBar = new QuikBar(lines[1]);
                }
            }
            // Add bars
            foreach (string line in lines)
            {
                // First line is a header, no parse
                if (i++ == 0)
                    continue;
                QuikQuote quikQuote = new QuikQuote(line);

                // Add bar to wealth-lab if quote is a quote of new bar
                if(IsNewBarQuote(quikBar, quikQuote, ds.BarDataScale))
                {
                    bars.Add(quikBar.OpenTime, quikBar.Open, quikBar.High, quikBar.Low, quikBar.Close, quikBar.Volume);
                    quikBar = new QuikBar();
                }
                quikBar.Update(quikQuote);

                // Start a new bar or not?
                //TimeSpan timeFromOpen = quikBar.OpenTime - quikQuote.Time;
                
//                bars.Add(quikQuote.Time, quikQuote.Close, quikQuote.Close, quikQuote.Close, quikQuote.Close, quikQuote.Volume);
                
            }

            // Load history
            if (bars.Count != 0)
            {

                bars.Level2History= new Level2History(bars.Symbol);
                bars.Level2History.Load(bars.Date.Last());
            }
            return bars;
        }

        /// <summary>
        /// True if QuikQuote is inside QuikBar time interval
        /// </summary>
        /// <param name="bar">QuikBar</param>
        /// <param name="quote">QuikQuote</param>
        /// <param name="dataScale">Bar data scale</param>
        /// <returns></returns>
        private static bool IsNewBarQuote(QuikBar bar, QuikQuote quote, BarDataScale dataScale)
        {

            switch (dataScale.Scale)
            {
                case BarScale.Tick:
                    return true;
                    break;
                case BarScale.Second:
                    return (quote.Time.Second != bar.OpenTime.Second
                        && quote.Time.Second % dataScale.BarInterval == 0);
                    break;
                case BarScale.Minute:
                    return ( quote.Time.Minute != bar.OpenTime.Minute
                        && quote.Time.Minute % dataScale.BarInterval == 0);
                    break;
                case BarScale.Daily:
                case BarScale.Weekly:
                default:
                    return (quote.Time.Day != bar.OpenTime.Day
                        && quote.Time.Day % dataScale.BarInterval == 0);
                    break;
                case BarScale.Monthly:
                case BarScale.Quarterly:
                    return (quote.Time.Month != bar.OpenTime.Month
                        && quote.Time.Month % dataScale.BarInterval == 0);
                    break;
                case BarScale.Yearly:
                    return (quote.Time.Year != bar.OpenTime.Year
                        && quote.Time.Year % dataScale.BarInterval == 0);


            }
        }

        /// <summary>
        /// Get css file names
        /// </summary>
        /// <returns></returns>
        public static List<string> GetHistorySymbols()
        {
            List<string> symbols = new List<string>();
            string[] searchResult = Directory.GetFiles(historyDirectory, historyFileMask,SearchOption.TopDirectoryOnly);
            foreach(string filePath in searchResult)
            {
                symbols.Add(Path.GetFileNameWithoutExtension(filePath));
            }
            return symbols;
        }
    }
}