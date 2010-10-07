using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.IO;
using System.Xml;
using System.Reflection;

namespace TradeRobotics.DataProviders.Quik.Dom
{
    /// <summary>
    /// Loads Depth Of Market data from DDE
    /// </summary>
    public static class DomLoader
    {
        // History file path \{symbol}\{day}
        //private static string historyPath = @".\Data\DataSets\Level2\{0}\{1}.xml";

//        private static string historyPath = @"Data\DataSets\Level2\{0}\{1}";

        /// <summary>
        /// Histories for different symbols
        /// </summary>
        private static Dictionary<string, Level2History> Level2Histories = new Dictionary<string, Level2History>();
        
        /// <summary>
        /// Add Dom snapshot from dde server data
        /// </summary>
        /// <param name="table"></param>
        public static void AddSnapShotFromDde(string symbol, object[][] table)
        {
            // Create level 2 shapshot from table
            Level2 snapShot = new Level2(symbol, table, DateTime.Now);

            // Get level2 history for symbol
            Level2History history;
            if (Level2Histories.ContainsKey(symbol))
            {
                // Add snapshot to existing 
                history = Level2Histories[symbol];
            }
            else
            {
                history = new Level2History(symbol) { SaveOnExit = true };
                Level2Histories.Add(symbol, history);
                // Add this history to bars
                if(QuikQuotesLoader.HistoryBars.ContainsKey(symbol))
                    QuikQuotesLoader.HistoryBars[symbol].Level2History = history;
            }

            if (history.Count != 0
                && snapShot.Time.Minute % 5 == 1
                && history.Last().Time.Minute != snapShot.Time.Minute)
            {
                // Save history
                history.Save();
            }

            // Add to history
            history.Add(snapShot);
            //history.Save();
            //history.Load(snapShot.Time);
            
        }


        ///// <summary>
        ///// Save level2 snapshot
        ///// </summary>
        ///// <param name="level2"></param>
        //public static void SaveHistory(Level2History history)
        //{
        //    if(history.Count == 0)
        //        return;
        //    // Form file path
  
        //    string filePath = string.Format(historyPath, history.Symbol, history[0].Time.ToString("yyyy-MM-dd"));
        //    filePath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), filePath);
        //    CreateDirectoriesIfNotExist(filePath);

        //    // Serialize
        //    XmlSerializer serializer = new XmlSerializer(typeof(Level2History));
        //    StreamWriter stream = File.CreateText(filePath);
        //    serializer.Serialize(stream, history);
        //    stream.Close();
        //}

        ///// <summary>
        ///// Create directories for file path
        ///// </summary>
        ///// <param name="filePath"></param>
        //private static void CreateDirectoriesIfNotExist(string filePath)
        //{
        //    // Sy
        //    DirectoryInfo symbolDirectory = new DirectoryInfo(Path.GetDirectoryName(filePath));
        //    DirectoryInfo domDirectory = symbolDirectory.Parent;
        //    if (!domDirectory.Exists)
        //        domDirectory.Create();
        //    if (!symbolDirectory.Exists)
        //        symbolDirectory.Create();
        //}
    }
}
