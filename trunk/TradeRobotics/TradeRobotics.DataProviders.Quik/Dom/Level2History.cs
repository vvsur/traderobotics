using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.IO;
using System.Reflection;

namespace TradeRobotics.DataProviders.Quik.Dom
{
    /// <summary>
    /// Manage Dom history data in files
    /// </summary>
    public class Level2History:List<Level2>
    {
        private static string historyPath = @".\Data\DataSets\Level2\{0}\{1}.xml";

        /// <summary>
        /// Symbol
        /// </summary>
        public string Symbol { get; set; }
        public bool SaveOnExit = false;

        /// <summary>
        /// Constructor of level2 history for symbol
        /// </summary>
        /// <param name="symbol"></param>
        public Level2History(string symbol):this()
        {
            Symbol = symbol;

        }

        public Level2History():base() { }

        #region Load, Save
        /// <summary>
        /// Save level2 snapshot
        /// </summary>
        /// <param name="level2"></param>
        public void Save()
        {
            if (this.Count == 0)
                return;
            // Form file path

            string filePath = GetFilePath(this[0].Time);
            CreateDirectoriesIfNotExist(filePath);

            using (StreamWriter stream = File.CreateText(filePath))
            {
                // Serialize
                XmlSerializer serializer = new XmlSerializer(typeof(Level2History));
                serializer.Serialize(stream, this);
                stream.Close();
            }
        }

        /// <summary>
        /// If no data for the date in history, load history
        /// </summary>
        /// <param name="date"></param>
        public void LoadIfNotLoaded(DateTime date)
        {

            if (this.Count  == 0
                ||
                (   this.First().Time <= date
                    && this.Last().Time >= date))
            {
                return;
            }
            else
            {
                Load(date);
            }
        }

        /// <summary>
        /// Load history for special date
        /// </summary>
        /// <param name="date"></param>
        public void Load(DateTime date)
        {
            string filePath = GetFilePath(date);
            this.Clear();
            if (!File.Exists(filePath))
                return;
            using (StreamReader stream = File.OpenText(filePath))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(Level2History));
                Level2History tmpHistory= serializer.Deserialize(stream) as Level2History;
                this.Symbol = tmpHistory.Symbol;
                this.AddRange(tmpHistory);
            }
        }

        /// <summary>
        /// Get history xml file path for special time
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public string GetFilePath(DateTime time)
        {
            string filePath = string.Format(historyPath, this.Symbol, time.ToString("yyyy-MM-dd"));
            filePath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), filePath);
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

        #region IDisposable Members

         ~Level2History()
        {
            if(SaveOnExit)
             Save();
        }
        #endregion
    }
}
