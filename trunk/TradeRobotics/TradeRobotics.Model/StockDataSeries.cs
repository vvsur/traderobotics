using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TradeRobotics.Model
{
    /// <summary>
    /// M1, M15 etc. bars
    /// </summary>
    public class StockDataSeries
    {
        /// <summary>
        /// Symbol ticker
        /// </summary>
        public string Symbol;

        /// <summary>
        /// Period in minutes
        /// </summary>
        public int Period;

        /// <summary>
        /// Bars data
        /// </summary>
        public List<Bar> Bars = new List<Bar>();
        #region price data
        public List<DateTime> Times = new List<DateTime>();
        public List<double> Open = new List<double>();
        public List<double> Low = new List<double>();
        public List<double> High = new List<double>();
        public List<double> Close = new List<double>();
        public List<double> Volume = new List<double>();

        /// <summary>
        /// Bars count
        /// </summary>
        public int Count
        {
           get{ return Bars.Count;}
        }

        #endregion

    }
}
