using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WealthLab;
using TradeRobotics.DataProviders.Quik.Dom;

namespace TradeRobotics.DataProviders.Quik.Dom
{
    /// <summary>
    /// Bars object with Dom history
    /// </summary>
    public class BarsAndDom:Bars
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="scale"></param>
        /// <param name="interval"></param>
        public BarsAndDom(string symbol, BarScale scale, int interval):base(symbol, scale, interval)
        {
            Level2History = new Level2History(Symbol);
        }

        /// <summary>
        /// Depth of market history
        /// </summary>
        public Level2History Level2History;
    }
}
