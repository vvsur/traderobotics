using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradeRobotics.TradeAdapters;
using TradeRobotics.TradeLibrary;
using TradeRobotics.Model;

namespace TradeRobotics.TradeAdapters.Test
{
    /// <summary>
    /// Trade adapter for robot testing
    /// </summary>
    public class TestTradeAdapter
    {
        public List<Order> Orders;
        
        
        public void Buy(string symbol, double price, double volume)
        {
        }
        public void BuyAtMarket(string symbol, double volume)
        {
        }
        public void Sell(string symbol, double price, double volume)
        {
        }
        public void SellAtMarket(string symbol, double volume)
        {
        }
    }
}
