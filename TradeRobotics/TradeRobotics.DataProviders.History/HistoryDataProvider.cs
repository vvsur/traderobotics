using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradeRobotics.Model;
using System.IO;
using TradeRobotics.TradeLibrary;
using System.Text.RegularExpressions;

namespace TradeRobotics.DataProviders.History
{
    /// <summary>
    /// Provider historical data from file
    /// </summary>
    public class HistoryDataProvider:IDataProvider
    {

        #region Load data from file
        /// <summary>
        /// 
        /// </summary>
        public Tuple<string, int, bool> GetDataFileInfo(string fileName)
        {
            Regex regex = new Regex(@"(?<name>\w+)_(?<periodName>[m,d])(?<periodValue>\d+)(?<quotes>_quotes)*\.csv$", RegexOptions.IgnoreCase);

            Match match = regex.Match(fileName);
            if(!match.Success)
                return new Tuple<string, int, bool>(null,0,false);

            string name = match.Groups["name"].Value;
            string periodName = match.Groups["periodName"].Value;
            int periodValue = Convert.ToInt32(match.Groups["periodValue"].Value);
            if(periodName == "H") 
                periodValue = 60*periodValue;
            if(periodName == "D")
                periodValue = 60*24*periodValue;
            bool isQuotes = (match.Groups["quotes"].Value == "_quotes");

            return new Tuple<string,int,bool>(name, periodValue, isQuotes);
    
        }
        
        //public const string historyFileName = @"{0}_M{1}.csv";
        public const string quotesHistoryFileName = @"{0}_{1}_quotes.csv";
        
        /// <summary>
        /// Get bars from file
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="period"></param>
//        public BarCollection LoadBars(string symbol, int period)
        public StockDataSeries LoadBars(string filePath)
        {
            // Load from file
            //string filePath = string.Format(historyFileName, symbol, period);
            Tuple<string, int, bool> dataInfo = GetDataFileInfo(filePath);
            filePath = string.Concat(DataContext.DataDirectory, filePath);

            string[] lines = File.ReadAllLines(filePath);
            int i = 0;
            // Init quik bar
            StockDataSeries dataSeries = new StockDataSeries();
            dataSeries.Symbol = dataInfo.Item1;
            dataSeries.Period = dataInfo.Item2; ;
            // Add bars
            foreach (string line in lines)
            {
                // First line is a header, no parse
                if (i++ == 0)
                    continue;
                Bar bar = ConverterHelper.LoadBar(line);

                dataSeries.Times.Add(bar.Time);
                dataSeries.Open.Add(bar.Open);
                dataSeries.Low.Add(bar.Low);
                dataSeries.High.Add(bar.High);
                dataSeries.Close.Add(bar.Close);
                dataSeries.Volume.Add(bar.Volume);

            }
            return dataSeries;
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

        /// <summary>
        /// New history data
        /// </summary>
        public event EventHandler<TickEventArgs> Tick;
    }
}
