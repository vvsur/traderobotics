using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradeRobotics.Model;
using System.IO;

namespace TradeRobotics.DataProviders.History
{
    /// <summary>
    /// Provider historical data from file
    /// </summary>
    public class HistoryDataProvider
    {

        #region Load data from file
        public const string historyFileName = @"{0}_M{1}.csv";
        public const string quotesHistoryFileName = @"{0}_{1}_quotes.csv";
        
        /// <summary>
        /// Get bars from file
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="period"></param>
        public BarCollection LoadBars(string symbol, int period)
        {
            // Load from file
            string filePath = string.Format(historyFileName, symbol, period);
            filePath = string.Concat(DataContext.DataDirectory, filePath);
            
            string[] lines = File.ReadAllLines(filePath);
            int i = 0;
            // Init quik bar
            BarCollection bars = new BarCollection();
            bars.Period = period;
            bars.Symbol = symbol;
            // Add bars
            foreach (string line in lines)
            {
                // First line is a header, no parse
                if (i++ == 0)
                    continue;
                Bar bar = ConverterHelper.LoadBar(line);
                bars.Bars.Add(bar);

            }
            return bars;
        }


        /// <summary>
        /// Load quotes for one day
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        public List<Quote> LoadQuotes(string symbol, DateTime date)
        {
            string fileName = string.Format(quotesHistoryFileName, symbol, date.ToString("yyyy.MM.dd"));
            string filePath = string.Concat(DataContext.DataDirectory, fileName);
            return LoadQuotesFromFile(filePath);

        }
        
        /// <summary>
        /// Load all quotes for symbol
        /// </summary>
        /// <param name="symbol"></param>
        /// <returns></returns>
        public List<Quote> LoadQuotes(string symbol)
        {
            string searchPattern = string.Format(quotesHistoryFileName, symbol, "*");

            string[] fileNames = Directory.GetFiles(DataContext.DataDirectory, searchPattern);
            List<Quote> allQuotes = new List<Quote>();
            foreach (string file in fileNames)
            {
                allQuotes.AddRange(LoadQuotesFromFile(file));
            }
            return allQuotes;
        }
        
        /// <summary>
        /// Get quotes from file
        /// </summary>
        /// <param name="symbol"></param>
        /// <returns></returns>
        protected List<Quote> LoadQuotesFromFile(string filePath)
        {
            /*string filePath = string.Format(quotesHistoryFileName, symbol);
            filePath = string.Concat(DataContext.DataDirectory, filePath);*/
            
            string[] lines = File.ReadAllLines(filePath);
            int i = 0;
            List<Quote> quotes = new List<Quote>();
            // Add bars
            foreach (string line in lines)
            {
                // First line is a header, no parse
                if (i++ == 0)
                    continue;
                Quote quote = ConverterHelper.LoadQuote(line);
                quotes.Add(quote);

            }
            return quotes;

        }
        #endregion

    }
}
