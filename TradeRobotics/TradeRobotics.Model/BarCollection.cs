using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TradeRobotics.Model
{
    /// <summary>
    /// M1, M15 etc. bars
    /// </summary>
    public class BarCollection
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
    }
}
