using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradeRobotics.TradeAdapters;
using TradeRobotics.TradeLibrary;
using TradeRobotics.Model;

namespace TradeRobotics.TradeAdapters
{
    /// <summary>
    /// Trade adapter for robot testing
    /// </summary>
    public class TestTradeAdapter : ITradeAdapter
    {
        /// <summary>
        /// Emulated orders
        /// </summary>
        public List<Order> Orders = new List<Order>();

        #region Buy or sell orders
        public void Buy(string symbol, double price, double volume)
        {
            Order order = new Order() { OrderType = OrderType.Buy, Price = price, Volume = volume, Time = TestContext.CurrentTime };
            Orders.Add(order);
        }
        public void BuyAtMarket(string symbol, double volume)
        {
            Order order = new Order() { OrderType = OrderType.Buy, Volume = volume, IsMarket = true, Time = TestContext.CurrentTime };
            Orders.Add(order);
        }
        public void Sell(string symbol, double price, double volume)
        {
            Order order = new Order() { OrderType = OrderType.Sell, Price = price, Volume = volume, Time = TestContext.CurrentTime };
            Orders.Add(order);
        }
        public void SellAtMarket(string symbol, double volume)
        {
            Order order = new Order() { OrderType = OrderType.Sell, Volume = volume, IsMarket = true, Time = TestContext.CurrentTime };
            Orders.Add(order);
        }
        #endregion
    }
}
