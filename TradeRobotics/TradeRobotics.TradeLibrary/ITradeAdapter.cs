using System;
namespace TradeRobotics.TradeLibrary
{
    public interface ITradeAdapter
    {
        void Buy(string symbol, double price, double volume);
        void BuyAtMarket(string symbol, double volume);
        void Sell(string symbol, double price, double volume);
        void SellAtMarket(string symbol, double volume);
    }
}
