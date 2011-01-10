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
        public DateTime Time {get;set;}

        
        public OrderType OrderType {get;set;}

        public string Symbol {get;set;}
        public double Price {get;set;}
        public double Volume {get;set;}

        public bool IsMarket { get; set; }

    }
}
