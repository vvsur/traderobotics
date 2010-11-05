using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TradeRobotics.Model
{
    /// <summary>
    /// Buy or sell order to trade system
    /// </summary>
    public class Order
    {
        public DateTime Time;

        
        public OrderType OrderType;

        public string Symbol;
        public double Price;
        public double Volume;

        /// <summary>
        /// Buy or sell by market price
        /// </summary>
        public bool IsMarket = false;
    }
}
