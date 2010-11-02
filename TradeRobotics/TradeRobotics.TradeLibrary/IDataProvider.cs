using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradeRobotics.Model;

namespace TradeRobotics.TradeLibrary
{
    public interface IDataProvider
    {
        StockDataSeries DataSeries {get;set;}
        event EventHandler<TickEventArgs> Tick;
    }
}
