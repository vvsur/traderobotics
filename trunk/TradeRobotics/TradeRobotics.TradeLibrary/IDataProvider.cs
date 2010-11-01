using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TradeRobotics.TradeLibrary
{
    public interface IDataProvider
    {
        event EventHandler<TickEventArgs> Tick;
    }
}
