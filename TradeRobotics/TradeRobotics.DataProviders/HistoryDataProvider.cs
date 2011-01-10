using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradeRobotics.Model;
using System.IO;
using TradeRobotics.TradeLibrary;
using System.Text.RegularExpressions;
using System.Xml.Serialization;
using TradeRobotics.Model.Depth;


namespace TradeRobotics.DataProviders
{
    /// <summary>
    /// Provider historical data from file
    /// </summary>
    public class HistoryDataProvider : IDataProvider
    {
        /// <summary>
        /// Data series
        /// </summary>
        public StockDataSeries DataSeries { get; set; }


        /// <summary>
        /// Get info from string
        /// </summary>
        public Tuple<string, int, bool> GetDataFileInfo(string fileName)
        {
            Tuple<string, int, bool> result = new Tuple<string, int, bool>(string.Empty, 0, false);
            
            fileName = Path.GetFileName(fileName);
            //Regex regex = new Regex(@"(?<name>\w+)_(?<periodName>[m,d])(?<periodValue>\d+)(?<quotes>_quotes)*\.csv$", RegexOptions.IgnoreCase);

            // Try bars
            Regex barsFileRegex = new Regex(@"(?<name>\w+)_(?<periodName>[m,d])(?<periodValue>\d+)\.csv$", RegexOptions.IgnoreCase);
            Match match = barsFileRegex.Match(fileName);
            if (match.Success)
            {
                // Prepare bars file info
                string name = match.Groups["name"].Value;
                string periodName = match.Groups["periodName"].Value;
                int periodValue = Convert.ToInt32(match.Groups["periodValue"].Value);
                if (periodName == "H")
                    periodValue = 60 * periodValue;
                if (periodName == "D")
                    periodValue = 60 * 24 * periodValue;
                result = new Tuple<string, int, bool>(name, periodValue, false);

            }
            else
            {
                // Try quotes
                Regex quotesFileRegex = new Regex(@"^(?<name>.+)_(?<date>\d+\-\d+\-\d+)(?<quotes>_quotes)\.csv$",RegexOptions.IgnoreCase);
                match = quotesFileRegex.Match(fileName);
                if(match.Success)
                {
                    // Prepare quotes file info
                // Prepare bars file info
                string name = match.Groups["name"].Value;
                result = new Tuple<string, int, bool>(name, 0, true);
                }
            }

            return result;

            bool isQuotes = (match.Groups["quotes"].Value == "_quotes");
    
        }

        //public const string historyFileName = @"{0}_M{1}.csv";
        public const string quotesHistoryFileName = @"{0}_{1}_quotes.csv";
        private static string depthHistoryFileName = @"Depth\{0}\{1}.xml";
        private const string depthHistorySymbolDir = @"Depth\{0}";

        /// <summary>
        /// Load history data from file
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public StockDataSeries LoadFromFile(string filePath)
        {
            DataSeries = new StockDataSeries();
            var dataFileInfo = GetDataFileInfo(filePath);
            bool isQuotes = dataFileInfo.Item3;
            DataSeries.Symbol = dataFileInfo.Item1;
            // Load as bars
            if (!isQuotes)
            {
                DataSeries = LoadBars(filePath);
            }
            // Load as quotes
            else
            {
                DataSeries.Quotes = LoadQuotesFromFile(filePath);
                LoadDepth(this.DataSeries.Quotes.Last().Time);
            }
            return DataSeries;
        }

        /// <summary>
        /// Get bars from file
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="period"></param>
        public StockDataSeries LoadBars(string filePath)
        {
            // Load from file
            //string filePath = string.Format(historyFileName, symbol, period);
            Tuple<string, int, bool> dataInfo = GetDataFileInfo(filePath);
            filePath = string.Concat(DataContext.DataDirectory, filePath);

            string[] lines = File.ReadAllLines(filePath);
            int i = 0;
            // Init quik bar
            DataSeries = new StockDataSeries();
            DataSeries.Symbol = dataInfo.Item1;
            DataSeries.Period = dataInfo.Item2; ;
            // Add bars
            foreach (string line in lines)
            {
                // First line is a header, no parse
                if (i++ == 0)
                    continue;
                Bar bar = ConverterHelper.LoadBar(line);

                DataSeries.Bars.Add(bar);
                DataSeries.Times.Add(bar.Time);
                DataSeries.Open.Add(bar.Open);
                DataSeries.Low.Add(bar.Low);
                DataSeries.High.Add(bar.High);
                DataSeries.Close.Add(bar.Close);
                DataSeries.Volume.Add(bar.Volume);
            }

            return DataSeries;
        }

        #region Quotes
        /// <summary>
        /// Load quotes for one day
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        public List<Quote> LoadQuotes(string symbol, DateTime date)
        {
            string fileName = string.Format(quotesHistoryFileName, symbol, date.ToString("yyyy-MM-dd"));
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
        /// Get quotes portion from file
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


        #region Depth
        /// <summary>
        /// Save level2 snapshot
        /// </summary>
        /// <param name="level2"></param>
        public void SaveDepth()
        {
            if (this.DataSeries.Depth.Count == 0)
                return;
            // Form file path
            string filePath = GetDepthFilePath(this.DataSeries.Depth[0].Time);
            CreateDirectoriesIfNotExist(filePath);

            using (StreamWriter stream = File.CreateText(filePath))
            {
                // Serialize
                XmlSerializer serializer = new XmlSerializer(this.DataSeries.Depth.GetType());
                serializer.Serialize(stream, this);
                stream.Close();
            }
        }

        /// <summary>
        /// If no data for the date in history, load history
        /// </summary>
        /// <param name="date"></param>
        public void LoadDepthIfNotLoaded(DateTime date)
        {

            if (this.DataSeries.Depth.Count == 0
                ||
                (this.DataSeries.Depth.First().Time <= date
                    && this.DataSeries.Depth.Last().Time >= date))
            {
                return;
            }
            else
            {
                LoadDepth(date);
            }
        }


        /// <summary>
        /// Load history for special date
        /// </summary>
        /// <param name="date"></param>
        public void LoadDepth(DateTime date)
        {
            string filePath = GetDepthFilePath(date);

            this.DataSeries.Depth.Clear();
            if (!File.Exists(filePath))
                return;
            using (StreamReader stream = File.OpenText(filePath))
            {
                XmlSerializer serializer = new XmlSerializer(this.DataSeries.Depth.GetType());
                List<OrderBook> tmpDepth = serializer.Deserialize(stream) as List<OrderBook>;
                this.DataSeries.Depth.AddRange(tmpDepth);
            }
        }

        /// <summary>
        /// Get history xml file path for special time
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public string GetDepthFilePath(DateTime time)
        {
            string filePath = string.Format(depthHistoryFileName, this.DataSeries.Symbol, time.ToString("yyyy-MM-dd"));
            filePath = Path.Combine(Path.GetDirectoryName(DataContext.DataDirectory), filePath);
            return filePath;
        }

        /// <summary>
        /// Create directories for file path
        /// </summary>
        /// <param name="filePath"></param>
        private void CreateDirectoriesIfNotExist(string filePath)
        {
            // Sy
            DirectoryInfo symbolDirectory = new DirectoryInfo(Path.GetDirectoryName(filePath));
            DirectoryInfo domDirectory = symbolDirectory.Parent;
            if (!domDirectory.Exists)
                domDirectory.Create();
            if (!symbolDirectory.Exists)
                symbolDirectory.Create();
        }
        #endregion


        /// <summary>
        /// New history data
        /// </summary>
        public event EventHandler<TickEventArgs> Tick;
    }
}
