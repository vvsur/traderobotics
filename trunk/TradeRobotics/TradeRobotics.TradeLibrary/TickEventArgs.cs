using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradeRobotics.Model;

namespace TradeRobotics.TradeLibrary
{
    /// <summary>
    /// New tick event
    /// </summary>
    public class TickEventArgs:EventArgs
    {
        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="dataSeries">updated data series</param>
        /// <param name="i">bar index</param>
        public TickEventArgs(StockDataSeries dataSeries, int i)
        {
            DataSeries = dataSeries;
            LastBarIndex = i;
        }

        /// <summary>
        /// Last bar in data series index
        /// </summary>
        public int LastBarIndex { get; set; }

        /// <summary>
        /// Data series received new data
        /// </summary>
        public StockDataSeries DataSeries { get; set; }
    }
}
